using System.Text.Json;

namespace CRM.Medical.Domain.Entities;

public sealed class ContentVersion
{
    public int Id { get; set; }

    public int PageId { get; set; }

    public JsonDocument SnapshotData { get; set; } = default!;

    public int VersionNumber { get; set; }

    public string? ChangeNotes { get; set; }

    public string CreatedByUserId { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public Page Page { get; set; } = default!;
}
