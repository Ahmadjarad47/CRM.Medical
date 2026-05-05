using System.Text.Json;
using CRM.Medical.Domain.Enums;
using CRM.Medical.Application.Features.Permissions.DTOs;

namespace CRM.Medical.Application.Features.Permissions.Services;

public interface IRolePermissionService
{
    Task AssignPermissionToRoleAsync(
        string roleId,
        string name,
        string resource,
        string action,
        PolicyEffect effect,
        int priority,
        JsonDocument? conditionJson,
        string? description,
        bool isEnabled,
        CancellationToken cancellationToken);

    Task RemovePermissionFromRoleAsync(string roleId, Guid policyId, CancellationToken cancellationToken);

    Task<IReadOnlyList<AccessPolicyDto>> GetRolePermissionsAsync(string roleId, CancellationToken cancellationToken);
}
