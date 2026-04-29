using CRM.Medical.Application.Features.Permissions.DTOs;

namespace CRM.Medical.Application.Features.Permissions.Services;

public interface IPermissionService
{
    Task<PermissionDto> CreateAsync(string name, string? description, CancellationToken cancellationToken);

    Task UpdateAsync(Guid id, string name, string? description, CancellationToken cancellationToken);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyList<PermissionDto>> GetPermissionsAsync(CancellationToken cancellationToken);
}
