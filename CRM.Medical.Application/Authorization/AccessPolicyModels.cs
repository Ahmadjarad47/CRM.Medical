using System.Linq.Expressions;
using System.Text.Json;
using CRM.Medical.Domain.Entities;

namespace CRM.Medical.Application.Authorization;

public sealed record CurrentSubjectContext(
    string? UserId,
    IReadOnlyList<string> RoleNames,
    IReadOnlyList<string> RoleIds,
    string? City,
    string? CreatedByUserId,
    bool IsAuthenticated,
    string? Email = null,
    string? TenantId = null);

public sealed record AccessPolicyEvaluationRequest(string Resource, string Action);

public sealed class AccessPolicyValidationResult
{
    public bool IsValid => Errors.Count == 0;
    public List<string> Errors { get; } = [];
}

public abstract record AccessConditionNode;
public sealed record AccessConditionGroup(IReadOnlyList<AccessConditionNode> All, IReadOnlyList<AccessConditionNode> Any) : AccessConditionNode;
public sealed record AccessConditionPredicate(string Field, string Operator, JsonElement? Value) : AccessConditionNode;
public sealed record AccessConditionCollectionAny(string Path, AccessConditionNode Condition) : AccessConditionNode;

public interface ICurrentSubjectAccessor
{
    Task<CurrentSubjectContext> GetCurrentAsync(CancellationToken cancellationToken);
}

public interface IAccessPolicyRuleStore
{
    Task<IReadOnlyList<AccessPolicy>> GetEnabledPoliciesAsync(string resource, string action, CancellationToken cancellationToken);
}

public interface IAccessPolicyConditionParser
{
    AccessConditionNode? Parse(JsonDocument? condition);
}

public interface IAccessPolicyConditionValidator
{
    AccessPolicyValidationResult Validate<TEntity>(AccessConditionNode? condition);
}

public interface IAccessPolicyMetadataProvider
{
    string ResolveResource<TEntity>();
}

public interface IAccessPolicyRuntimeTokenResolver
{
    object? Resolve(string token, CurrentSubjectContext subject);
}

public interface IAccessPolicyExpressionCompiler
{
    Expression<Func<TEntity, bool>> Compile<TEntity>(AccessConditionNode? condition, CurrentSubjectContext subject);
}

public interface IAccessPolicyEvaluator
{
    Task<IQueryable<TEntity>> ApplyAsync<TEntity>(
        IQueryable<TEntity> query,
        string resource,
        string action,
        CancellationToken cancellationToken);

    Task<bool> CanAccessAsync<TEntity>(
        TEntity entity,
        string resource,
        string action,
        CancellationToken cancellationToken);
}
