using System.Text.Json;

namespace CRM.Medical.Application.Features.Pages.DTOs;

public sealed record PageTranslationDto(
    int Id,
    string Language,
    string Title,
    string Slug,
    string? MetaTitle,
    string? MetaDescription,
    string? MetaKeywords,
    string? OpenGraphImageUrl,
    string? CanonicalUrl,
    string? BreadcrumbTitle);

public sealed record BlockLocalizationDto(
    int Id,
    string Language,
    string? Heading,
    string? Subheading,
    string? Description,
    JsonElement? ContentData,
    string? MediaUrl,
    string? MediaAltText,
    string? ButtonText,
    string? ButtonLink,
    string? ButtonStyle);

public sealed record ContentBlockDto(
    int Id,
    string BlockType,
    int Order,
    string? CustomCssClass,
    JsonElement? CustomStyles,
    string? Animation,
    JsonElement? VisibilityRules,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    IReadOnlyList<BlockLocalizationDto> Localizations);

public sealed record ContentVersionDto(
    int Id,
    int VersionNumber,
    JsonElement SnapshotData,
    string? ChangeNotes,
    string CreatedByUserId,
    DateTime CreatedAt);

public sealed record PageDto(
    int Id,
    string TemplateKey,
    int? ParentId,
    int Order,
    string PublishStatus,
    DateTime? PublishScheduledAt,
    DateTime? PublishedAt,
    bool IsVisibleInNav,
    bool IsActive,
    string CreatedByUserId,
    string? UpdatedByUserId,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    IReadOnlyList<PageTranslationDto> Translations,
    IReadOnlyList<ContentBlockDto> ContentBlocks,
    IReadOnlyList<ContentVersionDto> Versions);

public sealed record PageListItemDto(
    int Id,
    string TemplateKey,
    int? ParentId,
    int Order,
    string PublishStatus,
    bool IsVisibleInNav,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    string? DisplayTitle,
    string? DisplaySlug);

public sealed record WebsiteContentBlockDto(
    int Id,
    string BlockType,
    int Order,
    string? CustomCssClass,
    JsonElement? CustomStyles,
    string? Animation,
    string? Heading,
    string? Subheading,
    string? Description,
    JsonElement? ContentData,
    string? MediaUrl,
    string? MediaAltText,
    string? ButtonText,
    string? ButtonLink,
    string? ButtonStyle);

public sealed record WebsitePageDto(
    int Id,
    string TemplateKey,
    int? ParentId,
    int Order,
    string Language,
    string Title,
    string Slug,
    string? MetaTitle,
    string? MetaDescription,
    string? MetaKeywords,
    string? OpenGraphImageUrl,
    string? CanonicalUrl,
    string? BreadcrumbTitle,
    IReadOnlyList<WebsiteContentBlockDto> ContentBlocks);

public sealed record WebsiteNavigationPageDto(
    int Id,
    string TemplateKey,
    int? ParentId,
    int Order,
    string Language,
    string Title,
    string Slug,
    string? BreadcrumbTitle);
