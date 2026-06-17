using CRM.Medical.Domain.Entities;

namespace CRM.Medical.Application.Features.Ads;

public interface IAdRepository
{
    Task<Ad?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task AddAsync(Ad entity, CancellationToken cancellationToken = default);

    Task UpdateAsync(Ad entity, CancellationToken cancellationToken = default);

    Task DeleteAsync(Ad entity, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Ad>> ListAsync(CancellationToken cancellationToken = default);
}
