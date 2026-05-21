using CRM.Medical.Application.Authorization;
using CRM.Medical.Domain.Entities;
using CRM.Medical.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CRM.Medical.Infrastructure.Authorization;

internal sealed class AccessPolicyReadService(MedicalDbContext db) : IAccessPolicyReadService
{
    public async Task<IReadOnlyDictionary<string, IReadOnlyList<AccessPolicySummaryDto>>> GetPoliciesForRolesAsync(
        IEnumerable<string> roleNames,
        CancellationToken cancellationToken)
    {
        var normalizedRoleNames = roleNames
            .Where(roleName => !string.IsNullOrWhiteSpace(roleName))
            .Select(roleName => roleName.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if (normalizedRoleNames.Length == 0)
            return new Dictionary<string, IReadOnlyList<AccessPolicySummaryDto>>(StringComparer.OrdinalIgnoreCase);

        var policies = await db.AccessPolicies
            .AsNoTracking()
            .Where(policy =>
                policy.SubjectType == AccessPolicySubjectType.Role &&
                normalizedRoleNames.Contains(policy.SubjectKey))
            .OrderBy(policy => policy.SubjectKey)
            .ThenBy(policy => policy.Priority)
            .ThenBy(policy => policy.Resource)
            .ThenBy(policy => policy.Action)
            .Select(policy => new AccessPolicySummaryDto(
                policy.Id,
                policy.Resource,
                policy.Action,
                policy.Effect,
                policy.SubjectType,
                policy.SubjectKey,
                policy.Priority,
                policy.IsEnabled,
                policy.Description,
                policy.ValidFrom,
                policy.ValidTo))
            .ToListAsync(cancellationToken);

        var grouped = policies
            .GroupBy(policy => policy.SubjectKey, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(
                group => group.Key,
                group => (IReadOnlyList<AccessPolicySummaryDto>)group.ToList(),
                StringComparer.OrdinalIgnoreCase);

        foreach (var roleName in normalizedRoleNames)
        {
            grouped.TryAdd(roleName, []);
        }

        return grouped;
    }
}
