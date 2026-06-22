using System.Text.Json;

namespace CRM.Medical.Domain.Entities;

public sealed class BlockLocalization
{
    public int Id { get; set; }

    public int ContentBlockId { get; set; }

    public string Language { get; set; } = string.Empty;

    public string? Heading { get; set; }

    public string? Subheading { get; set; }

    public string? Description { get; set; }

    public JsonDocument? ContentData { get; set; }

    public string? MediaUrl { get; set; }

    public string? MediaAltText { get; set; }

    public string? ButtonText { get; set; }

    public string? ButtonLink { get; set; }

    public string? ButtonStyle { get; set; }

    public ContentBlock ContentBlock { get; set; } = default!;
}
