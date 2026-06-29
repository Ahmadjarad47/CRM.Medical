namespace CRM.Medical.Domain.Entities;

public sealed class Page
{
    public int Id { get; set; }

    public string TemplateKey { get; set; } = string.Empty;

    public int? ParentId { get; set; }

    public int Order { get; set; }

    public string PublishStatus { get; set; } = string.Empty;

    public DateTime? PublishScheduledAt { get; set; }

    public DateTime? PublishedAt { get; set; }

    public bool IsVisibleInNav { get; set; }

    public bool IsActive { get; set; } = true;

    /// <summary>Role names allowed to view this page. Empty means visible to everyone.</summary>
    public ICollection<string> VisibleToRoles { get; set; } = new List<string>();

    public string CreatedByUserId { get; set; } = string.Empty;

    public string? UpdatedByUserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Page? Parent { get; set; }

    public ICollection<Page> Children { get; set; } = new List<Page>();

    public ICollection<PageTranslation> Translations { get; set; } = new List<PageTranslation>();

    public ICollection<ContentBlock> ContentBlocks { get; set; } = new List<ContentBlock>();

    public ICollection<ContentVersion> Versions { get; set; } = new List<ContentVersion>();
}
