namespace CRM.Medical.Domain.Entities;

public sealed class MessageAttachment : BaseEntity
{
    public Guid Id { get; set; }

    public Guid MessageId { get; set; }

    public Message Message { get; set; } = null!;

    public string FileName { get; set; } = string.Empty;

    public string FileUrl { get; set; } = string.Empty;

    public string? FileType { get; set; }

    public long FileSize { get; set; }

    public DateTime UploadedAt { get; set; }
}
