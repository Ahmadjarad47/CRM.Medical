using CRM.Medical.Application.Features.Permissions.DTOs;

namespace CRM.Medical.Application.Features.Permissions.Services;

public interface IRolePermissionService
{
    Task AssignPermissionToRoleAsync(string roleId, Guid permissionId, CancellationToken cancellationToken);

    Task RemovePermissionFromRoleAsync(string roleId, Guid permissionId, CancellationToken cancellationToken);

    Task<IReadOnlyList<PermissionDto>> GetRolePermissionsAsync(string roleId, CancellationToken cancellationToken);
}
