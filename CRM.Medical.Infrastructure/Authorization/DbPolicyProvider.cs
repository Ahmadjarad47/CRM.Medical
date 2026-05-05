using CRM.Medical.Application.Authorization;
using CRM.Medical.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace CRM.Medical.Infrastructure.Authorization;

public sealed class DbPolicyProvider(MedicalDbContext db, IMemoryCache cache) : IPolicyProvider
{
    public async Task<IReadOnlyList<AbacPolicyDefinition>> GetPoliciesAsync(
        PolicyEvaluationContext context,
        CancellationToken cancellationToken)
    {
        var permission = context.Permission;
        var rolesKey = string.Join(",", context.Roles.OrderBy(x => x, StringComparer.OrdinalIgnoreCase));
        var cacheKey = $"abac:policies:{permission.Key}:u:{context.UserId}:r:{rolesKey}";
        if (cache.TryGetValue(cacheKey, out IReadOnlyList<AbacPolicyDefinition>? cached) && cached is not null)
            return cached;

        var policies = await db.AccessPolicies
            .AsNoTracking()
            .Where(x =>
                x.IsEnabled
                && x.Resource == permission.Resource
                && x.Action == permission.Action
                && ((x.SubjectType == Domain.Enums.SubjectType.User && x.SubjectId == context.UserId)
                    || (x.SubjectType == Domain.Enums.SubjectType.Role && context.Roles.Contains(x.SubjectId))
                    || (x.SubjectType == Domain.Enums.SubjectType.Group && context.Roles.Contains(x.SubjectId))))
            .OrderByDescending(x => x.Priority)
            .ThenBy(x => x.Id)
            .Select(x => new AbacPolicyDefinition(
                x.Id,
                x.Name,
                x.Resource,
                x.Action,
                x.Effect,
                x.SubjectType,
                x.SubjectId,
                x.Condition,
                x.Priority,
                x.IsEnabled,
                x.Description))
            .ToListAsync(cancellationToken);

        cache.Set(cacheKey, policies, TimeSpan.FromMinutes(5));
        return policies;
    }
}
