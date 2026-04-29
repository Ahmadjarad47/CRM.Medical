using CRM.Medical.Application.Features.Permissions.DTOs;

namespace CRM.Medical.Application.Features.Permissions.Services;

public interface IUserPermissionService
{
    Task AssignPermissionToUserAsync(string userId, Guid permissionId, CancellationToken cancellationToken);

    Task RemovePermissionFromUserAsync(string userId, Guid permissionId, CancellationToken cancellationToken);

    Task<IReadOnlyList<PermissionDto>> GetUserPermissionsAsync(string userId, CancellationToken cancellationToken);
}
