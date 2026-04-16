using System.Text.Json;

namespace CRM.Medical.Domain.Entities;

public sealed class Banner
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;

    public string MediaUrl { get; set; } = string.Empty;

    public string InternalLink { get; set; } = string.Empty;

    public string ExternalLink { get; set; } = string.Empty;

    public string TargetType { get; set; } = string.Empty;

    public string Location { get; set; } = string.Empty;

    public int DisplayOrder { get; set; }

    public bool IsActive { get; set; }

    public JsonDocument? VisibilityRules { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public int ViewsCount { get; set; }

    public int ClicksCount { get; set; }

    public DateTime CreatedAt { get; set; }
}

