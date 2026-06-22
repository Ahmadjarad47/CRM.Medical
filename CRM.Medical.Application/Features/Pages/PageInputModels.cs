using System.Text.Json;

namespace CRM.Medical.Application.Features.Pages;

public sealed record PageTranslationInput(
    string Language,
    string Title,
    string Slug,
    string? MetaTitle,
    string? MetaDescription,
    string? MetaKeywords,
    string? OpenGraphImageUrl,
    string? CanonicalUrl,
    string? BreadcrumbTitle);

public sealed record BlockLocalizationInput(
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

public sealed record ContentBlockInput(
    string BlockType,
    int Order,
    string? CustomCssClass,
    JsonElement? CustomStyles,
    string? Animation,
    JsonElement? VisibilityRules,
    bool IsActive,
    IReadOnlyList<BlockLocalizationInput> Localizations);
