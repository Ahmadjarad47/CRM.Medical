using CRM.Medical.Application.Features.Permissions.Services;
using Microsoft.EntityFrameworkCore;

namespace CRM.Medical.Infrastructure.Persistence;

public sealed class UserEffectivePermissionsProvider(MedicalDbContext db) : IUserEffectivePermissionsProvider
{
    public async Task<IReadOnlyList<string>> GetPermissionNamesForUserAsync(
        string userId,
        CancellationToken cancellationToken)
    {
        var roleIds = db.UserRoles
            .AsNoTracking()
            .Where(ur => ur.UserId == userId)
            .Select(ur => ur.RoleId);

        return await db.RolePermissions
            .AsNoTracking()
            .Where(rp => roleIds.Contains(rp.RoleId))
            .Select(rp => rp.Permission.Name)
            .Distinct()
            .OrderBy(n => n)
            .ToListAsync(cancellationToken);
    }
}
