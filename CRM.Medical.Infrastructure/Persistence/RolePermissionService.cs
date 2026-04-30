using CRM.Medical.Application.Common.Caching;
using CRM.Medical.Application.Exceptions;
using CRM.Medical.Application.Features.Permissions.DTOs;
using CRM.Medical.Application.Features.Permissions.Services;
using CRM.Medical.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CRM.Medical.Infrastructure.Persistence;

public sealed class RolePermissionService(
    MedicalDbContext db,
    RoleManager<IdentityRole> roleManager,
    UserManager<User> userManager,
    ICacheService cache)
    : IRolePermissionService
{
    public async Task AssignPermissionToRoleAsync(string roleId, Guid permissionId, CancellationToken cancellationToken)
    {
        _ = await roleManager.FindByIdAsync(roleId)
            ?? throw new ApplicationNotFoundException($"Role '{roleId}' was not found.");

        _ = await db.Permissions.AsNoTracking().FirstOrDefaultAsync(p => p.Id == permissionId, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Permission '{permissionId}' was not found.");

        var exists = await db.RolePermissions.AnyAsync(
            rp => rp.RoleId == roleId && rp.PermissionId == permissionId,
            cancellationToken);
        if (exists)
            return;

        db.RolePermissions.Add(new RolePermission
        {
            RoleId = roleId,
            PermissionId = permissionId
        });

        await db.SaveChangesAsync(cancellationToken);
        await InvalidateUsersForRoleCachesAsync(roleId, cancellationToken);
    }

    public async Task RemovePermissionFromRoleAsync(string roleId, Guid permissionId, CancellationToken cancellationToken)
    {
        _ = await roleManager.FindByIdAsync(roleId)
            ?? throw new ApplicationNotFoundException($"Role '{roleId}' was not found.");

        var link = await db.RolePermissions.FirstOrDefaultAsync(
            rp => rp.RoleId == roleId && rp.PermissionId == permissionId,
            cancellationToken);
        if (link is null)
            return;

        db.RolePermissions.Remove(link);
        await db.SaveChangesAsync(cancellationToken);
        await InvalidateUsersForRoleCachesAsync(roleId, cancellationToken);
    }

    public async Task<IReadOnlyList<PermissionDto>> GetRolePermissionsAsync(
        string roleId,
        CancellationToken cancellationToken)
    {
        _ = await roleManager.FindByIdAsync(roleId)
            ?? throw new ApplicationNotFoundException($"Role '{roleId}' was not found.");

        return await (
            from rp in db.RolePermissions.AsNoTracking()
            join p in db.Permissions.AsNoTracking() on rp.PermissionId equals p.Id
            where rp.RoleId == roleId
            orderby p.Name
            select new PermissionDto(p.Id, p.Name, p.Description, p.CreatedAt)
        ).ToListAsync(cancellationToken);
    }

    private async Task InvalidateUsersForRoleCachesAsync(string roleId, CancellationToken cancellationToken)
    {
        var role = await roleManager.FindByIdAsync(roleId);
        if (string.IsNullOrEmpty(role?.Name))
            return;

        foreach (var user in await userManager.GetUsersInRoleAsync(role.Name))
            await cache.RemoveAsync(CacheKeys.UserById(user.Id), cancellationToken);
    }
}
