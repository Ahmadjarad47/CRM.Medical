using CRM.Medical.Application.Features.Pages;
using CRM.Medical.Domain.Constants;
using CRM.Medical.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CRM.Medical.Infrastructure.Persistence;

public sealed class PageRepository(MedicalDbContext dbContext) : IPageRepository
{
    public Task<Page?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default) =>
        DetailedQuery(tracking: false)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public Task<Page?> GetByIdWithDetailsForUpdateAsync(int id, CancellationToken cancellationToken = default) =>
        DetailedQuery(tracking: true)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Page>> ListAsync(CancellationToken cancellationToken = default) =>
        await dbContext.Pages
            .AsNoTracking()
            .AsSplitQuery()
            .Include(p => p.Translations)
            .OrderBy(p => p.Order)
            .ThenByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);

    public Task<Page?> GetPublishedBySlugAsync(
        string language,
        string slug,
        DateTime nowUtc,
        CancellationToken cancellationToken = default)
    {
        var languageNormalized = language.Trim().ToLowerInvariant();
        var slugNormalized = slug.Trim().ToLowerInvariant();

        return DetailedQuery(tracking: false)
            .Where(p =>
                p.IsActive &&
                p.PublishStatus == PagePublishStatuses.Published &&
                (!p.PublishedAt.HasValue || p.PublishedAt <= nowUtc) &&
                (!p.PublishScheduledAt.HasValue || p.PublishScheduledAt <= nowUtc))
            .Where(p => p.Translations.Any(t =>
                t.Language.ToLower() == languageNormalized &&
                t.Slug.ToLower() == slugNormalized))
            .OrderByDescending(p => p.PublishedAt)
            .ThenBy(p => p.Order)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Page>> ListPublishedForNavigationAsync(
        string language,
        DateTime nowUtc,
        CancellationToken cancellationToken = default)
    {
        var languageNormalized = language.Trim().ToLowerInvariant();

        return await dbContext.Pages
            .AsNoTracking()
            .AsSplitQuery()
            .Include(p => p.Translations)
            .Where(p =>
                p.IsActive &&
                p.IsVisibleInNav &&
                p.PublishStatus == PagePublishStatuses.Published &&
                (!p.PublishedAt.HasValue || p.PublishedAt <= nowUtc) &&
                (!p.PublishScheduledAt.HasValue || p.PublishScheduledAt <= nowUtc) &&
                p.Translations.Any(t => t.Language.ToLower() == languageNormalized))
            .OrderBy(p => p.Order)
            .ThenBy(p => p.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> TemplateKeyExistsAsync(
        string templateKey,
        int? excludePageId = null,
        CancellationToken cancellationToken = default)
    {
        var normalized = templateKey.Trim().ToLowerInvariant();
        return await dbContext.Pages
            .AsNoTracking()
            .Where(p => !excludePageId.HasValue || p.Id != excludePageId.Value)
            .AnyAsync(p => p.TemplateKey.ToLower() == normalized, cancellationToken);
    }

    public async Task<bool> SlugExistsAsync(
        string language,
        string slug,
        int? excludePageId = null,
        CancellationToken cancellationToken = default)
    {
        var languageNormalized = language.Trim().ToLowerInvariant();
        var slugNormalized = slug.Trim().ToLowerInvariant();

        return await dbContext.PageTranslations
            .AsNoTracking()
            .Where(t => !excludePageId.HasValue || t.PageId != excludePageId.Value)
            .AnyAsync(t =>
                t.Language.ToLower() == languageNormalized &&
                t.Slug.ToLower() == slugNormalized, cancellationToken);
    }

    public async Task AddAsync(Page page, CancellationToken cancellationToken = default)
    {
        dbContext.Pages.Add(page);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Page page, CancellationToken cancellationToken = default)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Page page, CancellationToken cancellationToken = default)
    {
        dbContext.Pages.Remove(page);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private IQueryable<Page> DetailedQuery(bool tracking)
    {
        var query = tracking ? dbContext.Pages : dbContext.Pages.AsNoTracking();
        return query
            .AsSplitQuery()
            .Include(p => p.Translations)
            .Include(p => p.ContentBlocks)
                .ThenInclude(b => b.Localizations)
            .Include(p => p.Versions);
    }
}
