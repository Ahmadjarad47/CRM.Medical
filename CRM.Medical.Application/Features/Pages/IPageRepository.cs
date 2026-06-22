using CRM.Medical.Domain.Entities;

namespace CRM.Medical.Application.Features.Pages;

public interface IPageRepository
{
    Task<Page?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default);

    Task<Page?> GetByIdWithDetailsForUpdateAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Page>> ListAsync(CancellationToken cancellationToken = default);

    Task<Page?> GetPublishedBySlugAsync(
        string language,
        string slug,
        DateTime nowUtc,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Page>> ListPublishedForNavigationAsync(
        string language,
        DateTime nowUtc,
        CancellationToken cancellationToken = default);

    Task<bool> TemplateKeyExistsAsync(
        string templateKey,
        int? excludePageId = null,
        CancellationToken cancellationToken = default);

    Task<bool> SlugExistsAsync(
        string language,
        string slug,
        int? excludePageId = null,
        CancellationToken cancellationToken = default);

    Task AddAsync(Page page, CancellationToken cancellationToken = default);

    Task UpdateAsync(Page page, CancellationToken cancellationToken = default);

    Task DeleteAsync(Page page, CancellationToken cancellationToken = default);
}
