using CRM.Medical.Application.Features.Permissions.DTOs;

namespace CRM.Medical.Application.Abstractions;

public interface IPermissionRepository
{
    Task<IReadOnlyList<PermissionDto>> ListAsync(CancellationToken cancellationToken);

    Task<PermissionDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<PermissionDto?> GetByNameAsync(string name, CancellationToken cancellationToken);

    Task<IReadOnlySet<string>> GetAllNamesAsync(CancellationToken cancellationToken);

    Task<PermissionDto> CreateAsync(string name, string? description, DateTime createdAtUtc, CancellationToken cancellationToken);

    Task UpdateDescriptionAsync(Guid id, string? description, CancellationToken cancellationToken);

    /// <summary>
    /// Deletes the catalog entry and removes matching user permission claims. Returns user ids for cache invalidation.
    /// </summary>
    Task<PermissionDeletionOutcome?> DeleteAsync(Guid id, CancellationToken cancellationToken);
}
