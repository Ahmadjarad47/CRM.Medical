using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace CRM.Medical.API.Contracts.Admin.Pages;

public sealed class CreatePageRequest
{
    [Required(ErrorMessage = "TemplateKey is required.")]
    [StringLength(120, MinimumLength = 1, ErrorMessage = "TemplateKey is required.")]
    public string TemplateKey { get; set; } = default!;

    public int? ParentId { get; set; }

    public int Order { get; set; }

    [Required(ErrorMessage = "PublishStatus is required.")]
    [StringLength(32, MinimumLength = 1, ErrorMessage = "PublishStatus is required.")]
    public string PublishStatus { get; set; } = "Draft";

    public DateTime? PublishScheduledAt { get; set; }

    public DateTime? PublishedAt { get; set; }

    public bool IsVisibleInNav { get; set; } = true;

    public bool IsActive { get; set; } = true;

    public List<string> VisibleToRoles { get; set; } = [];

    [Required(ErrorMessage = "At least one translation is required.")]
    [MinLength(1, ErrorMessage = "At least one translation is required.")]
    public List<PageTranslationRequest> Translations { get; set; } = [];

    public List<ContentBlockRequest> ContentBlocks { get; set; } = [];

    [StringLength(2000)]
    public string? ChangeNotes { get; set; }
}

public sealed class UpdatePageRequest
{
    [Required(ErrorMessage = "TemplateKey is required.")]
    [StringLength(120, MinimumLength = 1, ErrorMessage = "TemplateKey is required.")]
    public string TemplateKey { get; set; } = default!;

    public int? ParentId { get; set; }

    public int Order { get; set; }

    [Required(ErrorMessage = "PublishStatus is required.")]
    [StringLength(32, MinimumLength = 1, ErrorMessage = "PublishStatus is required.")]
    public string PublishStatus { get; set; } = "Draft";

    public DateTime? PublishScheduledAt { get; set; }

    public DateTime? PublishedAt { get; set; }

    public bool IsVisibleInNav { get; set; } = true;

    public bool IsActive { get; set; } = true;

    public List<string> VisibleToRoles { get; set; } = [];

    [Required(ErrorMessage = "At least one translation is required.")]
    [MinLength(1, ErrorMessage = "At least one translation is required.")]
    public List<PageTranslationRequest> Translations { get; set; } = [];

    public List<ContentBlockRequest> ContentBlocks { get; set; } = [];

    [StringLength(2000)]
    public string? ChangeNotes { get; set; }
}

public sealed class PageTranslationRequest
{
    [Required(ErrorMessage = "Language is required.")]
    [StringLength(10, MinimumLength = 1, ErrorMessage = "Language is required.")]
    public string Language { get; set; } = default!;

    [Required(ErrorMessage = "Title is required.")]
    [StringLength(300, MinimumLength = 1, ErrorMessage = "Title is required.")]
    public string Title { get; set; } = default!;

    [Required(ErrorMessage = "Slug is required.")]
    [StringLength(300, MinimumLength = 1, ErrorMessage = "Slug is required.")]
    public string Slug { get; set; } = default!;

    [StringLength(300)]
    public string? MetaTitle { get; set; }

    [StringLength(1000)]
    public string? MetaDescription { get; set; }

    [StringLength(1000)]
    public string? MetaKeywords { get; set; }

    [StringLength(2048)]
    public string? OpenGraphImageUrl { get; set; }

    [StringLength(2048)]
    public string? CanonicalUrl { get; set; }

    [StringLength(300)]
    public string? BreadcrumbTitle { get; set; }
}

public sealed class ContentBlockRequest
{
    [Required(ErrorMessage = "BlockType is required.")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "BlockType is required.")]
    public string BlockType { get; set; } = default!;

    public int Order { get; set; }

    [StringLength(200)]
    public string? CustomCssClass { get; set; }

    public JsonElement? CustomStyles { get; set; }

    [StringLength(100)]
    public string? Animation { get; set; }

    public JsonElement? VisibilityRules { get; set; }

    public bool IsActive { get; set; } = true;

    public List<BlockLocalizationRequest> Localizations { get; set; } = [];
}

public sealed class BlockLocalizationRequest
{
    [Required(ErrorMessage = "Language is required.")]
    [StringLength(10, MinimumLength = 1, ErrorMessage = "Language is required.")]
    public string Language { get; set; } = default!;

    [StringLength(300)]
    public string? Heading { get; set; }

    [StringLength(600)]
    public string? Subheading { get; set; }

    [StringLength(4000)]
    public string? Description { get; set; }

    public JsonElement? ContentData { get; set; }

    [StringLength(2048)]
    public string? MediaUrl { get; set; }

    [StringLength(500)]
    public string? MediaAltText { get; set; }

    [StringLength(300)]
    public string? ButtonText { get; set; }

    [StringLength(2048)]
    public string? ButtonLink { get; set; }

    [StringLength(100)]
    public string? ButtonStyle { get; set; }
}
