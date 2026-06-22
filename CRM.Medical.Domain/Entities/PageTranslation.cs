namespace CRM.Medical.Domain.Entities;

public sealed class PageTranslation
{
    public int Id { get; set; }

    public int PageId { get; set; }

    public string Language { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Slug { get; set; } = string.Empty;

    public string? MetaTitle { get; set; }

    public string? MetaDescription { get; set; }

    public string? MetaKeywords { get; set; }

    public string? OpenGraphImageUrl { get; set; }

    public string? CanonicalUrl { get; set; }

    public string? BreadcrumbTitle { get; set; }

    public Page Page { get; set; } = default!;
}
