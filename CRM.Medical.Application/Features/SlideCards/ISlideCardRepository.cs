using CRM.Medical.Domain.Entities;

namespace CRM.Medical.Application.Features.SlideCards;

public interface ISlideCardRepository
{
    Task<SlideCard?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task AddAsync(SlideCard entity, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<SlideCard>> ListActiveAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<SlideCard>> ListForWebsiteAsync(DateTime nowUtc, CancellationToken cancellationToken = default);
}

