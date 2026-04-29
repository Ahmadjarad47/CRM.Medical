using CRM.Medical.Application.Features.Permissions.Services;
using Microsoft.EntityFrameworkCore;

namespace CRM.Medical.Infrastructure.Persistence;

public sealed class UserEffectivePermissionsProvider(MedicalDbContext db) : IUserEffectivePermissionsProvider
{
    public async Task<IReadOnlyList<string>> GetPermissionNamesForUserAsync(
        string userId,
        CancellationToken cancellationToken)
    {
        return await db.UserPermissions
            .AsNoTracking()
            .Where(up => up.UserId == userId)
            .Select(up => up.Permission.Name)
            .OrderBy(n => n)
            .ToListAsync(cancellationToken);
    }
}
