using CRM.Medical.Application.Features.Pages.DTOs;
using CRM.Medical.Domain.Entities;

namespace CRM.Medical.Application.Features.Pages;

internal static class PageMappings
{
    public static PageDto ToDto(this Page page)
    {
        var translations = page.Translations
            .OrderBy(t => t.Language)
            .Select(t => new PageTranslationDto(
                t.Id,
                t.Language,
                t.Title,
                t.Slug,
                t.MetaTitle,
                t.MetaDescription,
                t.MetaKeywords,
                t.OpenGraphImageUrl,
                t.CanonicalUrl,
                t.BreadcrumbTitle))
            .ToList();

        var blocks = page.ContentBlocks
            .OrderBy(b => b.Order)
            .ThenBy(b => b.Id)
            .Select(b => new ContentBlockDto(
                b.Id,
                b.BlockType,
                b.Order,
                b.CustomCssClass,
                b.CustomStyles?.RootElement.Clone(),
                b.Animation,
                b.VisibilityRules?.RootElement.Clone(),
                b.IsActive,
                b.CreatedAt,
                b.UpdatedAt,
                b.Localizations
                    .OrderBy(l => l.Language)
                    .Select(l => new BlockLocalizationDto(
                        l.Id,
                        l.Language,
                        l.Heading,
                        l.Subheading,
                        l.Description,
                        l.ContentData?.RootElement.Clone(),
                        l.MediaUrl,
                        l.MediaAltText,
                        l.ButtonText,
                        l.ButtonLink,
                        l.ButtonStyle))
                    .ToList()))
            .ToList();

        var versions = page.Versions
            .OrderByDescending(v => v.VersionNumber)
            .Select(v => new ContentVersionDto(
                v.Id,
                v.VersionNumber,
                v.SnapshotData.RootElement.Clone(),
                v.ChangeNotes,
                v.CreatedByUserId,
                v.CreatedAt))
            .ToList();

        return new PageDto(
            page.Id,
            page.TemplateKey,
            page.ParentId,
            page.Order,
            page.PublishStatus,
            page.PublishScheduledAt,
            page.PublishedAt,
            page.IsVisibleInNav,
            page.IsActive,
            page.VisibleToRoles.OrderBy(r => r, StringComparer.OrdinalIgnoreCase).ToList(),
            page.CreatedByUserId,
            page.UpdatedByUserId,
            page.CreatedAt,
            page.UpdatedAt,
            translations,
            blocks,
            versions);
    }

    public static PageListItemDto ToListItemDto(this Page page)
    {
        var displayTranslation = PickBestTranslation(page.Translations, "en-US");
        return new PageListItemDto(
            page.Id,
            page.TemplateKey,
            page.ParentId,
            page.Order,
            page.PublishStatus,
            page.IsVisibleInNav,
            page.IsActive,
            page.VisibleToRoles.OrderBy(r => r, StringComparer.OrdinalIgnoreCase).ToList(),
            page.CreatedAt,
            page.UpdatedAt,
            displayTranslation?.Title,
            displayTranslation?.Slug);
    }

    public static WebsitePageDto ToWebsiteDto(this Page page, string language)
    {
        var translation = PickBestTranslation(page.Translations, language)
            ?? throw new InvalidOperationException("Page must contain at least one translation.");

        var blocks = page.ContentBlocks
            .Where(b => b.IsActive)
            .OrderBy(b => b.Order)
            .ThenBy(b => b.Id)
            .Select(b =>
            {
                var localization = PickBestLocalization(b.Localizations, language);
                return new WebsiteContentBlockDto(
                    b.Id,
                    b.BlockType,
                    b.Order,
                    b.CustomCssClass,
                    b.CustomStyles?.RootElement.Clone(),
                    b.Animation,
                    localization?.Heading,
                    localization?.Subheading,
                    localization?.Description,
                    localization?.ContentData?.RootElement.Clone(),
                    localization?.MediaUrl,
                    localization?.MediaAltText,
                    localization?.ButtonText,
                    localization?.ButtonLink,
                    localization?.ButtonStyle);
            })
            .ToList();

        return new WebsitePageDto(
            page.Id,
            page.TemplateKey,
            page.ParentId,
            page.Order,
            translation.Language,
            translation.Title,
            translation.Slug,
            translation.MetaTitle,
            translation.MetaDescription,
            translation.MetaKeywords,
            translation.OpenGraphImageUrl,
            translation.CanonicalUrl,
            translation.BreadcrumbTitle,
            blocks);
    }

    public static WebsiteNavigationPageDto ToWebsiteNavigationDto(this Page page, string language)
    {
        var translation = PickBestTranslation(page.Translations, language)
            ?? throw new InvalidOperationException("Page must contain at least one translation.");

        return new WebsiteNavigationPageDto(
            page.Id,
            page.TemplateKey,
            page.ParentId,
            page.Order,
            translation.Language,
            translation.Title,
            translation.Slug,
            translation.BreadcrumbTitle);
    }

    private static PageTranslation? PickBestTranslation(IEnumerable<PageTranslation> translations, string language)
    {
        var list = translations.ToList();
        if (list.Count == 0)
            return null;

        return list.FirstOrDefault(t => string.Equals(t.Language, language, StringComparison.OrdinalIgnoreCase))
               ?? list.FirstOrDefault(t => string.Equals(t.Language, "en-US", StringComparison.OrdinalIgnoreCase))
               ?? list[0];
    }

    private static BlockLocalization? PickBestLocalization(IEnumerable<BlockLocalization> localizations, string language)
    {
        var list = localizations.ToList();
        if (list.Count == 0)
            return null;

        return list.FirstOrDefault(t => string.Equals(t.Language, language, StringComparison.OrdinalIgnoreCase))
               ?? list.FirstOrDefault(t => string.Equals(t.Language, "en-US", StringComparison.OrdinalIgnoreCase))
               ?? list[0];
    }
}
