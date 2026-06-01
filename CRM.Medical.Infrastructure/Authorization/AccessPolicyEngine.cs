using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using CRM.Medical.Application.Abstractions;
using CRM.Medical.Application.Authorization;
using CRM.Medical.Application.Exceptions;
using CRM.Medical.Domain.Entities;
using CRM.Medical.Domain.Entities.Accounting;
using CRM.Medical.Domain.Entities.ServiceRequests;
using CRM.Medical.Domain.Entities.Insurance;
using CRM.Medical.Domain.Entities.Store;
using CRM.Medical.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace CRM.Medical.Infrastructure.Authorization;

internal sealed class CurrentSubjectAccessor(
    ICurrentUserAccessor currentUser,
    MedicalDbContext db,
    UserManager<User> userManager) : ICurrentSubjectAccessor
{
    public async Task<CurrentSubjectContext> GetCurrentAsync(CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId;
        if (string.IsNullOrWhiteSpace(userId))
            return new CurrentSubjectContext(null, [], [], null, null, false);

        var user = await db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        var roles = currentUser.Roles.Count > 0
            ? currentUser.Roles
            : (await userManager.GetRolesAsync(new User { Id = userId })).ToArray();

        var roleIds = await (
            from ur in db.UserRoles.AsNoTracking()
            where ur.UserId == userId
            select ur.RoleId).ToListAsync(cancellationToken);

        return new CurrentSubjectContext(
            userId,
            roles,
            roleIds,
            user?.City,
            user?.CreatedByUserId,
            true,
            currentUser.Email ?? user?.Email,
            currentUser.TenantId);
    }
}

internal sealed class AccessPolicyRuleStore(MedicalDbContext db, IMemoryCache cache) : IAccessPolicyRuleStore
{
    private const string Wildcard = "*";

    public async Task<IReadOnlyList<AccessPolicy>> GetEnabledPoliciesAsync(string resource, string action, CancellationToken cancellationToken)
    {
        var cacheKey = $"access-policies:{resource}:{action}";
        if (cache.TryGetValue(cacheKey, out IReadOnlyList<AccessPolicy>? cached) && cached is not null)
            return cached;

        var utcNow = DateTime.UtcNow;
        var policies = await db.Set<AccessPolicy>()
            .AsNoTracking()
            .Where(x =>
                x.IsEnabled
                && x.DeletedAt == null
                && (x.Resource == resource || x.Resource == Wildcard)
                && (x.Action == action || x.Action == Wildcard)
                && (x.ValidFrom == null || x.ValidFrom <= utcNow)
                && (x.ValidTo == null || x.ValidTo >= utcNow))
            .OrderBy(x => x.Priority)
            .ThenBy(x => x.Resource == resource ? 0 : 1)
            .ThenBy(x => x.Action == action ? 0 : 1)
            .ThenBy(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

        cache.Set(cacheKey, policies, TimeSpan.FromMinutes(2));
        return policies;
    }
}

internal sealed class AccessPolicyRuntimeTokenResolver : IAccessPolicyRuntimeTokenResolver
{
    public object? Resolve(string token, CurrentSubjectContext subject) =>
        token switch
        {
            "@CurrentUserId" => subject.UserId,
            "@CurrentUserEmail" => subject.Email,
            "@CurrentUserRole" => subject.RoleNames.FirstOrDefault(),
            "@CurrentRoleIds" => subject.RoleIds,
            "@CurrentRoleNames" => subject.RoleNames,
            "@CurrentTenantId" => subject.TenantId,
            "@CurrentCity" => subject.City,
            "@CurrentUserCreatedById" => subject.CreatedByUserId,
            "@NowUtc" => DateTime.UtcNow,
            _ => null
        };
}

internal sealed class AccessPolicyConditionParser : IAccessPolicyConditionParser
{
    public AccessConditionNode? Parse(JsonDocument? condition)
    {
        if (condition is null)
            return null;
        return ParseElement(condition.RootElement);
    }

    private static AccessConditionNode ParseElement(JsonElement element)
    {
        if (element.TryGetProperty("collectionAny", out var anyNode))
        {
            var path = anyNode.GetProperty("path").GetString()
                ?? throw new ApplicationBadRequestException("collectionAny.path is required.");
            var child = ParseElement(anyNode.GetProperty("condition"));
            return new AccessConditionCollectionAny(path, child);
        }

        var hasField = element.TryGetProperty("field", out var field);
        var hasOperator = element.TryGetProperty("operator", out var op);
        if (hasField && hasOperator)
        {
            JsonElement? value = element.TryGetProperty("value", out var v) ? v : null;
            return new AccessConditionPredicate(
                field.GetString() ?? throw new ApplicationBadRequestException("field is required."),
                op.GetString() ?? throw new ApplicationBadRequestException("operator is required."),
                value);
        }

        var all = element.TryGetProperty("all", out var allNode)
            ? allNode.EnumerateArray().Select(ParseElement).ToList()
            : [];
        var any = element.TryGetProperty("any", out var anyItems)
            ? anyItems.EnumerateArray().Select(ParseElement).ToList()
            : [];

        return new AccessConditionGroup(all, any);
    }
}

internal sealed class AccessPolicyConditionValidator : IAccessPolicyConditionValidator
{
    private static readonly HashSet<string> AllowedOperators =
    [
        "eq", "neq", "gt", "gte", "lt", "lte", "in", "nin",
        "contains", "startswith", "endswith", "isnull", "notnull"
    ];
    private static readonly Dictionary<Type, HashSet<string>> RestrictedFieldsByType = new()
    {
        [typeof(User)] = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "id",
            "email",
            "fullName",
            "city",
            "phoneNumber",
            "isActive",
            "emailConfirmed",
            "createdAt",
            "createdByUserId"
        }
    };

    public AccessPolicyValidationResult Validate<TEntity>(AccessConditionNode? condition)
    {
        var result = new AccessPolicyValidationResult();
        ValidateNode(typeof(TEntity), condition, result, string.Empty);
        return result;
    }

    private static void ValidateNode(Type rootType, AccessConditionNode? node, AccessPolicyValidationResult result, string path)
    {
        if (node is null)
            return;

        switch (node)
        {
            case AccessConditionGroup group:
                foreach (var item in group.All)
                    ValidateNode(rootType, item, result, path);
                foreach (var item in group.Any)
                    ValidateNode(rootType, item, result, path);
                break;
            case AccessConditionCollectionAny collectionAny:
                if (!IsFieldAllowed(rootType, collectionAny.Path))
                {
                    result.Errors.Add($"Field path '{collectionAny.Path}' is not allowed for resource type '{rootType.Name}'.");
                    break;
                }
                var collectionProperty = ResolveProperty(rootType, collectionAny.Path);
                if (collectionProperty is null)
                {
                    result.Errors.Add($"Unknown collection path '{collectionAny.Path}'.");
                    break;
                }
                ValidateNode(collectionProperty.PropertyType.GetGenericArguments().FirstOrDefault() ?? collectionProperty.PropertyType, collectionAny.Condition, result, collectionAny.Path);
                break;
            case AccessConditionPredicate predicate:
                if (!AllowedOperators.Contains(predicate.Operator.ToLowerInvariant()))
                    result.Errors.Add($"Operator '{predicate.Operator}' is not allowed.");
                if (!IsFieldAllowed(rootType, predicate.Field))
                    result.Errors.Add($"Field path '{predicate.Field}' is not allowed for resource type '{rootType.Name}'.");
                if (ResolveProperty(rootType, predicate.Field) is null)
                    result.Errors.Add($"Unknown field path '{predicate.Field}'.");
                break;
        }
    }

    private static bool IsFieldAllowed(Type rootType, string path)
    {
        if (!RestrictedFieldsByType.TryGetValue(rootType, out var allowedFields))
            return true;

        var firstSegment = path.Split('.', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
        return firstSegment is not null && allowedFields.Contains(firstSegment);
    }

    private static PropertyInfo? ResolveProperty(Type type, string path)
    {
        var current = type;
        PropertyInfo? property = null;
        foreach (var segment in path.Split('.', StringSplitOptions.RemoveEmptyEntries))
        {
            property = current.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .FirstOrDefault(p => string.Equals(p.Name, segment, StringComparison.OrdinalIgnoreCase));
            if (property is null)
                return null;
            current = property.PropertyType;
            if (current != typeof(string) && typeof(System.Collections.IEnumerable).IsAssignableFrom(current) && current.IsGenericType)
                current = current.GetGenericArguments()[0];
        }
        return property;
    }
}

internal sealed class AccessPolicyMetadataProvider : IAccessPolicyMetadataProvider
{
    private static readonly Dictionary<Type, string> Map = new()
    {
        [typeof(TestRequest)] = "test_requests",
        [typeof(TestResult)] = "test_results",
        [typeof(MedicalTest)] = "medical_tests",
        [typeof(ExternalPatient)] = "external_patients",
        [typeof(SubscriptionPackage)] = "subscription_packages",
        [typeof(Conversation)] = "conversations",
        [typeof(ConversationParticipant)] = "conversation_participants",
        [typeof(Message)] = "messages",
        [typeof(User)] = "users",
        [typeof(Complaint)] = "complaints",
        [typeof(Banner)] = "banners",
        [typeof(Template)] = "templates",
        [typeof(ServiceRequestPageSetting)] = "service_request_page_settings",
        [typeof(VacantJob)] = "vacant_jobs",
        [typeof(EmploymentApplicationRequest)] = "employment_application_requests",
        [typeof(ClientJoinRequest)] = "client_join_requests",
        [typeof(ContractServiceRequest)] = "contract_service_requests",
        [typeof(InsuranceApprovalRequest)] = "insurance_approval_requests",
        [typeof(ProductCategory)] = "store_product_categories",
        [typeof(Product)] = "store_products",
        [typeof(StoreSetting)] = "store_settings",
        [typeof(StoreBanner)] = "store_banners",
        [typeof(StoreSlider)] = "store_sliders",
        [typeof(Coupon)] = "store_coupons",
        [typeof(StoreOrder)] = "store_orders",
        [typeof(AccountingPageSetting)] = "accounting_page_settings",
        [typeof(LabAccountPayment)] = "lab_account_payments",
        [typeof(LabAccountStatementFile)] = "lab_account_statement_files"
    };

    public string ResolveResource<TEntity>() =>
        Map.TryGetValue(typeof(TEntity), out var resource)
            ? resource
            : throw new ApplicationBadRequestException($"No access-policy resource mapping exists for {typeof(TEntity).Name}.");
}

internal sealed class AccessPolicyExpressionCompiler(IAccessPolicyRuntimeTokenResolver tokenResolver) : IAccessPolicyExpressionCompiler
{
    public Expression<Func<TEntity, bool>> Compile<TEntity>(AccessConditionNode? condition, CurrentSubjectContext subject)
    {
        var parameter = Expression.Parameter(typeof(TEntity), "e");
        var body = Build(condition, parameter, subject) ?? Expression.Constant(true);
        return Expression.Lambda<Func<TEntity, bool>>(body, parameter);
    }

    private Expression? Build(AccessConditionNode? node, Expression parameter, CurrentSubjectContext subject)
    {
        return node switch
        {
            null => Expression.Constant(true),
            AccessConditionGroup g => BuildGroup(g, parameter, subject),
            AccessConditionPredicate p => BuildPredicate(p, parameter, subject),
            AccessConditionCollectionAny c => BuildCollectionAny(c, parameter, subject),
            _ => throw new ApplicationBadRequestException("Unsupported condition node.")
        };
    }

    private Expression BuildGroup(AccessConditionGroup group, Expression parameter, CurrentSubjectContext subject)
    {
        Expression? allExpr = null;
        foreach (var node in group.All)
        {
            var expr = Build(node, parameter, subject);
            allExpr = allExpr is null ? expr : Expression.AndAlso(allExpr, expr!);
        }

        Expression? anyExpr = null;
        foreach (var node in group.Any)
        {
            var expr = Build(node, parameter, subject);
            anyExpr = anyExpr is null ? expr : Expression.OrElse(anyExpr, expr!);
        }

        if (allExpr is null && anyExpr is null)
            return Expression.Constant(true);
        if (allExpr is null)
            return anyExpr!;
        if (anyExpr is null)
            return allExpr;
        return Expression.AndAlso(allExpr, anyExpr);
    }

    private Expression BuildCollectionAny(AccessConditionCollectionAny condition, Expression parameter, CurrentSubjectContext subject)
    {
        var collectionExpr = ResolveMember(parameter, condition.Path);
        var elementType = collectionExpr.Type.GetGenericArguments().First();
        var elementParam = Expression.Parameter(elementType, "x");
        var nestedExpr = Build(condition.Condition, elementParam, subject) ?? Expression.Constant(true);
        var lambda = Expression.Lambda(nestedExpr, elementParam);
        return Expression.Call(
            typeof(Enumerable),
            nameof(Enumerable.Any),
            [elementType],
            collectionExpr,
            lambda);
    }

    private Expression BuildPredicate(AccessConditionPredicate predicate, Expression parameter, CurrentSubjectContext subject)
    {
        var left = ResolveMember(parameter, predicate.Field);
        var op = predicate.Operator.ToLowerInvariant();
        var rawValue = ResolveValue(predicate.Value, subject);

        if (op is "isnull")
            return Expression.Equal(left, Expression.Constant(null, left.Type));
        if (op is "notnull")
            return Expression.NotEqual(left, Expression.Constant(null, left.Type));

        return op switch
        {
            "eq" => Expression.Equal(left, BuildConstant(left.Type, rawValue)),
            "neq" => Expression.NotEqual(left, BuildConstant(left.Type, rawValue)),
            "gt" => Expression.GreaterThan(left, BuildConstant(left.Type, rawValue)),
            "gte" => Expression.GreaterThanOrEqual(left, BuildConstant(left.Type, rawValue)),
            "lt" => Expression.LessThan(left, BuildConstant(left.Type, rawValue)),
            "lte" => Expression.LessThanOrEqual(left, BuildConstant(left.Type, rawValue)),
            "in" => BuildIn(left, rawValue, negate: false),
            "nin" => BuildIn(left, rawValue, negate: true),
            "contains" => BuildContains(left, rawValue),
            "startswith" => Expression.Call(left, typeof(string).GetMethod(nameof(string.StartsWith), [typeof(string)])!, BuildConstant(left.Type, rawValue)),
            "endswith" => Expression.Call(left, typeof(string).GetMethod(nameof(string.EndsWith), [typeof(string)])!, BuildConstant(left.Type, rawValue)),
            _ => throw new ApplicationBadRequestException($"Operator '{predicate.Operator}' is not supported for expression compilation.")
        };
    }

    private static ConstantExpression BuildConstant(Type targetType, object? rawValue) =>
        Expression.Constant(ChangeType(rawValue, Nullable.GetUnderlyingType(targetType) ?? targetType), targetType);

    private object? ResolveValue(JsonElement? element, CurrentSubjectContext subject)
    {
        if (element is null)
            return null;

        return ResolveElementValue(element.Value, subject);
    }

    private object? ResolveElementValue(JsonElement element, CurrentSubjectContext subject)
    {
        if (element.ValueKind == JsonValueKind.String)
        {
            var value = element.GetString();
            if (!string.IsNullOrWhiteSpace(value) && value.StartsWith('@'))
                return tokenResolver.Resolve(value, subject);
            return value;
        }
        return element.ValueKind switch
        {
            JsonValueKind.Number when element.TryGetInt64(out var i) => i,
            JsonValueKind.Number => element.GetDouble(),
            JsonValueKind.Array => element.EnumerateArray().Select(x => ResolveElementValue(x, subject)).ToArray(),
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            _ => null
        };
    }

    private static Expression BuildContains(Expression left, object? rawValue)
    {
        if (left.Type == typeof(string))
        {
            var constant = Expression.Constant(rawValue?.ToString() ?? string.Empty, typeof(string));
            return Expression.Call(left, typeof(string).GetMethod(nameof(string.Contains), [typeof(string)])!, constant);
        }

        var elementType = left.Type.IsGenericType ? left.Type.GetGenericArguments()[0] : typeof(object);
        var value = Expression.Constant(ChangeType(rawValue, elementType), elementType);
        return Expression.Call(typeof(Enumerable), nameof(Enumerable.Contains), [elementType], left, value);
    }

    private static Expression BuildIn(Expression left, object? rawValue, bool negate)
    {
        var values = rawValue switch
        {
            null => [],
            IEnumerable<string> s => s.ToArray(),
            IEnumerable<object?> o => o.Select(x => x?.ToString() ?? string.Empty).ToArray(),
            _ => [rawValue.ToString() ?? string.Empty]
        };

        var elementType = Nullable.GetUnderlyingType(left.Type) ?? left.Type;
        var casted = values.Select(x => ChangeType(x, elementType)).ToArray();
        var list = Array.CreateInstance(elementType, casted.Length);
        for (var i = 0; i < casted.Length; i++)
            list.SetValue(casted[i], i);
        var contains = Expression.Call(typeof(Enumerable), nameof(Enumerable.Contains), [elementType], Expression.Constant(list), left);
        return negate ? Expression.Not(contains) : contains;
    }

    private static object? ChangeType(object? value, Type targetType)
    {
        if (value is null)
            return null;
        if (targetType.IsAssignableFrom(value.GetType()))
            return value;
        if (targetType.IsEnum)
            return Enum.Parse(targetType, value.ToString()!, true);
        return Convert.ChangeType(value, targetType);
    }

    private static Expression ResolveMember(Expression root, string path)
    {
        Expression current = root;
        foreach (var segment in path.Split('.', StringSplitOptions.RemoveEmptyEntries))
        {
            var property = current.Type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .FirstOrDefault(p => string.Equals(p.Name, segment, StringComparison.OrdinalIgnoreCase))
                ?? throw new ApplicationBadRequestException($"Field path '{path}' was not found.");
            current = Expression.Property(current, property);
        }
        return current;
    }
}

internal sealed class AccessPolicyEvaluator(
    ICurrentSubjectAccessor subjectAccessor,
    IAccessPolicyRuleStore ruleStore,
    IAccessPolicyConditionParser parser,
    IAccessPolicyConditionValidator validator,
    IAccessPolicyExpressionCompiler compiler) : IAccessPolicyEvaluator
{
    public async Task<IQueryable<TEntity>> ApplyAsync<TEntity>(
        IQueryable<TEntity> query,
        string resource,
        string action,
        CancellationToken cancellationToken)
    {
        var subject = await subjectAccessor.GetCurrentAsync(cancellationToken);
        var policies = await ruleStore.GetEnabledPoliciesAsync(resource, action, cancellationToken);
        var applicable = FilterApplicable(policies, subject);
        if (applicable.Count == 0)
            return query.Where(_ => false);

        var allows = new List<Expression<Func<TEntity, bool>>>();
        var denies = new List<Expression<Func<TEntity, bool>>>();

        foreach (var policy in applicable)
        {
            var condition = parser.Parse(policy.Condition);
            var validation = validator.Validate<TEntity>(condition);
            if (!validation.IsValid)
                continue;
            var expression = compiler.Compile<TEntity>(condition, subject);
            if (policy.Effect == AccessPolicyEffect.Deny)
                denies.Add(expression);
            else
                allows.Add(expression);
        }

        if (allows.Count == 0)
            return query.Where(_ => false);

        var allowPredicate = allows.Aggregate((current, next) => current.Or(next));
        query = query.Where(allowPredicate);
        if (denies.Count > 0)
        {
            var denyPredicate = denies.Aggregate((current, next) => current.Or(next));
            query = query.Where(denyPredicate.Not());
        }

        return query;
    }

    public async Task<bool> CanAccessAsync<TEntity>(
        TEntity entity,
        string resource,
        string action,
        CancellationToken cancellationToken)
    {
        var result = await ApplyAsync(new[] { entity }.AsQueryable(), resource, action, cancellationToken);
        return result.Any();
    }

    private static List<AccessPolicy> FilterApplicable(IReadOnlyList<AccessPolicy> policies, CurrentSubjectContext subject)
    {
        var list = new List<AccessPolicy>();
        foreach (var policy in policies.OrderBy(x => x.Priority))
        {
            if (!subject.IsAuthenticated && policy.SubjectType == AccessPolicySubjectType.Authenticated)
                continue;

            var include = policy.SubjectType switch
            {
                AccessPolicySubjectType.All => true,
                AccessPolicySubjectType.Authenticated => subject.IsAuthenticated,
                AccessPolicySubjectType.User => subject.UserId is not null && string.Equals(policy.SubjectKey, subject.UserId, StringComparison.Ordinal),
                AccessPolicySubjectType.Role => subject.RoleNames.Any(x => string.Equals(x, policy.SubjectKey, StringComparison.OrdinalIgnoreCase)),
                _ => false
            };

            if (include)
                list.Add(policy);
        }
        return list;
    }
}

internal static class AccessPolicyExpressionHelpers
{
    public static Expression<Func<T, bool>> Or<T>(
        this Expression<Func<T, bool>> left,
        Expression<Func<T, bool>> right)
    {
        var parameter = Expression.Parameter(typeof(T), "x");
        var body = Expression.OrElse(
            Expression.Invoke(left, parameter),
            Expression.Invoke(right, parameter));
        return Expression.Lambda<Func<T, bool>>(body, parameter);
    }

    public static Expression<Func<T, bool>> Not<T>(this Expression<Func<T, bool>> expr)
    {
        var parameter = Expression.Parameter(typeof(T), "x");
        var body = Expression.Not(Expression.Invoke(expr, parameter));
        return Expression.Lambda<Func<T, bool>>(body, parameter);
    }
}
