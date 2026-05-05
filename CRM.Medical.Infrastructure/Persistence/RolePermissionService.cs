using System.Text.Json;
using CRM.Medical.Application.Common.Caching;
using CRM.Medical.Application.Exceptions;
using CRM.Medical.Application.Features.Permissions.DTOs;
using CRM.Medical.Application.Features.Permissions.Services;
using CRM.Medical.Domain.Entities;
using CRM.Medical.Domain.Enums;
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
    public async Task AssignPermissionToRoleAsync(
        string roleId,
        string name,
        string resource,
        string action,
        PolicyEffect effect,
        int priority,
        JsonDocument? conditionJson,
        string? description,
        bool isEnabled,
        CancellationToken cancellationToken)
    {
        var role = await roleManager.FindByIdAsync(roleId)
            ?? throw new ApplicationNotFoundException($"Role '{roleId}' was not found.");

        var roleName = role.Name ?? throw new ApplicationBadRequestException("Role name is required.");
        var normalizedName = string.IsNullOrWhiteSpace(name) ? $"{roleName}:{resource}:{action}" : name.Trim();
        var normalizedResource = resource?.Trim() ?? string.Empty;
        var normalizedAction = action?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(normalizedResource))
            throw new ApplicationBadRequestException("Resource is required.");
        if (string.IsNullOrWhiteSpace(normalizedAction))
            throw new ApplicationBadRequestException("Action is required.");
        if (!Enum.IsDefined(effect))
            throw new ApplicationBadRequestException("Effect is invalid.");
        if (priority < 0)
            throw new ApplicationBadRequestException("Priority must be greater than or equal to 0.");

        var normalizedCondition = conditionJson is null ? null : JsonSerializer.Serialize(conditionJson.RootElement);
        var exists = await db.AccessPolicies.AnyAsync(
            x => x.Resource == normalizedResource
                 && x.Action == normalizedAction
                 && x.SubjectType == SubjectType.Role
                 && x.SubjectId == roleName
                 && x.Effect == effect
                 && (x.Condition ?? string.Empty) == (normalizedCondition ?? string.Empty)
                 && x.IsEnabled == isEnabled,
            cancellationToken);
        if (exists)
            return;

        db.AccessPolicies.Add(new AccessPolicy
        {
            Id = Guid.NewGuid(),
            Name = normalizedName,
            Resource = normalizedResource,
            Action = normalizedAction,
            SubjectType = SubjectType.Role,
            SubjectId = roleName,
            Effect = effect,
            Priority = priority,
            Condition = normalizedCondition,
            Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim(),
            IsEnabled = isEnabled
        });

        await db.SaveChangesAsync(cancellationToken);
        await InvalidateUsersForRoleCachesAsync(roleId, cancellationToken);
    }

    public async Task RemovePermissionFromRoleAsync(string roleId, Guid policyId, CancellationToken cancellationToken)
    {
        var role = await roleManager.FindByIdAsync(roleId)
            ?? throw new ApplicationNotFoundException($"Role '{roleId}' was not found.");

        var roleName = role.Name ?? throw new ApplicationBadRequestException("Role name is required.");
        var link = await db.AccessPolicies.FirstOrDefaultAsync(
            x => x.Id == policyId && x.SubjectType == SubjectType.Role && x.SubjectId == roleName,
            cancellationToken);
        if (link is null)
            return;

        db.AccessPolicies.Remove(link);
        await db.SaveChangesAsync(cancellationToken);
        await InvalidateUsersForRoleCachesAsync(roleId, cancellationToken);
    }

    public async Task<IReadOnlyList<AccessPolicyDto>> GetRolePermissionsAsync(
        string roleId,
        CancellationToken cancellationToken)
    {
        var role = await roleManager.FindByIdAsync(roleId)
            ?? throw new ApplicationNotFoundException($"Role '{roleId}' was not found.");
        var roleName = role.Name ?? throw new ApplicationBadRequestException("Role name is required.");

        return await db.AccessPolicies.AsNoTracking()
            .Where(x => x.SubjectType == SubjectType.Role && x.SubjectId == roleName)
            .OrderByDescending(x => x.Priority)
            .ThenBy(x => x.Name)
            .Select(x => new AccessPolicyDto(
                x.Id,
                x.Name,
                x.Resource,
                x.Action,
                x.SubjectType,
                x.SubjectId,
                x.Effect,
                x.Priority,
                x.Condition,
                x.Description,
                x.IsEnabled,
                x.CreatedAt,
                x.UpdatedAt))
            .ToListAsync(cancellationToken);
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
