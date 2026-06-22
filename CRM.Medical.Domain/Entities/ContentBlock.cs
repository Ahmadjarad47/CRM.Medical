using System.Text.Json;

namespace CRM.Medical.Domain.Entities;

public sealed class ContentBlock
{
    public int Id { get; set; }

    public int PageId { get; set; }

    public string BlockType { get; set; } = string.Empty;

    public int Order { get; set; }

    public string? CustomCssClass { get; set; }

    public JsonDocument? CustomStyles { get; set; }

    public string? Animation { get; set; }

    public JsonDocument? VisibilityRules { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Page Page { get; set; } = default!;

    public ICollection<BlockLocalization> Localizations { get; set; } = new List<BlockLocalization>();
}
