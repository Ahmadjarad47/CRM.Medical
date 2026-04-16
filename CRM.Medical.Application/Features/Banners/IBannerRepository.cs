using CRM.Medical.Domain.Entities;

namespace CRM.Medical.Application.Features.Banners;

public interface IBannerRepository
{
    Task<Banner?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task AddAsync(Banner entity, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Banner>> ListActiveAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Banner>> ListForWebsiteAsync(DateTime nowUtc, string? location, CancellationToken cancellationToken = default);
}

