namespace CRM.Medical.Domain.Entities;

public sealed class MessageRead : BaseEntity
{
    public Guid Id { get; set; }

    public Guid MessageId { get; set; }

    public Message Message { get; set; } = null!;

    public string UserId { get; set; } = string.Empty;

    public User User { get; set; } = null!;

    public DateTime ReadAt { get; set; }
}
