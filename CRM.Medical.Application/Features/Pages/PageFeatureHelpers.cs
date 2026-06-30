using System.Text.Json;
using CRM.Medical.Application.Features.Users.Constants;
using CRM.Medical.Domain.Constants;
using CRM.Medical.Domain.Entities;

namespace CRM.Medical.Application.Features.Pages;

internal static class PageFeatureHelpers
{
    public static string NormalizeStatus(string status)
    {
        var trimmed = status.Trim();
        var match = PagePublishStatuses.All
            .FirstOrDefault(x => string.Equals(x, trimmed, StringComparison.OrdinalIgnoreCase));

        return match ?? trimmed;
    }

    public static string NormalizeLanguage(string language) => language.Trim();

    public static string NormalizeSlug(string slug) => slug.Trim().ToLowerInvariant();

    public static IReadOnlyList<string> NormalizeVisibleToRoles(IReadOnlyList<string>? roles)
    {
        if (roles is null || roles.Count == 0)
            return Array.Empty<string>();

        return roles
            .Select(role => role.Trim())
            .Where(role => !string.IsNullOrEmpty(role))
            .Select(role => UserRoles.All.FirstOrDefault(
                knownRole => string.Equals(knownRole, role, StringComparison.OrdinalIgnoreCase)) ?? role)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    public static bool IsVisibleToUser(IEnumerable<string> visibleToRoles, IReadOnlyList<string> userRoles)
    {
        var allowedRoles = visibleToRoles as IReadOnlyList<string> ?? visibleToRoles.ToList();
        if (allowedRoles.Count == 0)
            return true;

        if (userRoles.Count == 0)
            return false;

        return allowedRoles.Any(role =>
            userRoles.Contains(role, StringComparer.OrdinalIgnoreCase));
    }

    public static JsonDocument? ToJsonDocument(JsonElement? value)
    {
        if (value is null)
            return null;

        var element = value.Value;
        if (element.ValueKind is JsonValueKind.Undefined or JsonValueKind.Null)
            return null;

        return JsonDocument.Parse(element.GetRawText());
    }

    public static JsonDocument BuildSnapshot(Page page)
    {
        var snapshot = new
        {
            page.TemplateKey,
            page.ParentId,
            page.Order,
            page.PublishStatus,
            page.PublishScheduledAt,
            page.PublishedAt,
            page.IsVisibleInNav,
            page.IsActive,
            VisibleToRoles = page.VisibleToRoles.OrderBy(r => r, StringComparer.OrdinalIgnoreCase).ToList(),
            Translations = page.Translations
                .OrderBy(t => t.Language)
                .Select(t => new
                {
                    t.Language,
                    t.Title,
                    t.Slug,
                    t.MetaTitle,
                    t.MetaDescription,
                    t.MetaKeywords,
                    t.OpenGraphImageUrl,
                    t.CanonicalUrl,
                    t.BreadcrumbTitle
                })
                .ToList(),
            ContentBlocks = page.ContentBlocks
                .OrderBy(b => b.Order)
                .ThenBy(b => b.Id)
                .Select(b => new
                {
                    b.BlockType,
                    b.Order,
                    b.CustomCssClass,
                    CustomStyles = b.CustomStyles?.RootElement.Clone(),
                    b.Animation,
                    VisibilityRules = b.VisibilityRules?.RootElement.Clone(),
                    b.IsActive,
                    Localizations = b.Localizations
                        .OrderBy(l => l.Language)
                        .Select(l => new
                        {
                            l.Language,
                            l.Heading,
                            l.Subheading,
                            l.Description,
                            ContentData = l.ContentData?.RootElement.Clone(),
                            l.MediaUrl,
                            l.MediaAltText,
                            l.ButtonText,
                            l.ButtonLink,
                            l.ButtonStyle
                        })
                        .ToList()
                })
                .ToList()
        };

        return JsonSerializer.SerializeToDocument(snapshot);
    }
}
