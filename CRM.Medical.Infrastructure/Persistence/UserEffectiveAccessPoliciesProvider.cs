using CRM.Medical.Application.Features.Permissions.DTOs;
using CRM.Medical.Application.Features.Permissions.Services;
using CRM.Medical.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace CRM.Medical.Infrastructure.Persistence;

public sealed class UserEffectiveAccessPoliciesProvider(MedicalDbContext db) : IUserEffectiveAccessPoliciesProvider
{
    public async Task<IReadOnlyList<AccessPolicyDto>> GetEffectiveAllowPoliciesForUserAsync(
        string userId,
        CancellationToken cancellationToken)
    {
        var roleNames = await db.UserRoles
            .AsNoTracking()
            .Where(ur => ur.UserId == userId)
            .Join(db.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r.Name!)
            .Where(n => n != null)
            .Distinct()
            .ToListAsync(cancellationToken);

        return await db.AccessPolicies
            .AsNoTracking()
            .Where(p =>
                p.IsEnabled
                && p.Effect == PolicyEffect.Allow
                && (
                    (p.SubjectType == SubjectType.User && p.SubjectId == userId)
                    || (p.SubjectType == SubjectType.Role && roleNames.Contains(p.SubjectId))
                    || (p.SubjectType == SubjectType.Group && roleNames.Contains(p.SubjectId))))
            .OrderByDescending(p => p.Priority)
            .ThenBy(p => p.Resource)
            .ThenBy(p => p.Action)
            .Select(p => new AccessPolicyDto(
                p.Id,
                p.Name,
                p.Resource,
                p.Action,
                p.SubjectType,
                p.SubjectId,
                p.Effect,
                p.Priority,
                p.Condition,
                p.Description,
                p.IsEnabled,
                p.CreatedAt,
                p.UpdatedAt))
            .ToListAsync(cancellationToken);
    }
}
