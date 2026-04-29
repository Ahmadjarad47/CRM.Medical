using CRM.Medical.Application.Common.Caching;
using CRM.Medical.Application.Exceptions;
using CRM.Medical.Application.Features.Permissions.DTOs;
using CRM.Medical.Application.Features.Permissions.Services;
using CRM.Medical.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CRM.Medical.Infrastructure.Persistence;

public sealed class UserPermissionService(
    MedicalDbContext db,
    UserManager<User> userManager,
    ICacheService cache)
    : IUserPermissionService
{
    public async Task AssignPermissionToUserAsync(
        string userId,
        Guid permissionId,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(userId)
            ?? throw new ApplicationNotFoundException($"User '{userId}' not found.");

        _ = await db.Permissions.AsNoTracking().FirstOrDefaultAsync(p => p.Id == permissionId, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Permission '{permissionId}' not found.");

        var exists = await db.UserPermissions.AnyAsync(
            up => up.UserId == userId && up.PermissionId == permissionId,
            cancellationToken);
        if (exists)
            return;

        db.UserPermissions.Add(new UserPermission
        {
            UserId = user.Id,
            PermissionId = permissionId
        });

        await db.SaveChangesAsync(cancellationToken);
        await cache.RemoveAsync(CacheKeys.UserById(user.Id), cancellationToken);
    }

    public async Task RemovePermissionFromUserAsync(
        string userId,
        Guid permissionId,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(userId)
            ?? throw new ApplicationNotFoundException($"User '{userId}' not found.");

        var link = await db.UserPermissions.FirstOrDefaultAsync(
            up => up.UserId == userId && up.PermissionId == permissionId,
            cancellationToken);
        if (link is null)
            return;

        db.UserPermissions.Remove(link);
        await db.SaveChangesAsync(cancellationToken);
        await cache.RemoveAsync(CacheKeys.UserById(user.Id), cancellationToken);
    }

    public async Task<IReadOnlyList<PermissionDto>> GetUserPermissionsAsync(
        string userId,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(userId)
            ?? throw new ApplicationNotFoundException($"User '{userId}' not found.");

        return await db.UserPermissions
            .AsNoTracking()
            .Where(up => up.UserId == user.Id)
            .Select(up => new PermissionDto(
                up.Permission.Id,
                up.Permission.Name,
                up.Permission.Description,
                up.Permission.CreatedAt))
            .OrderBy(d => d.Name)
            .ToListAsync(cancellationToken);
    }
}
