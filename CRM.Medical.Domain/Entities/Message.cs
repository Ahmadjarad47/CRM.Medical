using CRM.Medical.Domain.Chat;

namespace CRM.Medical.Domain.Entities;

public sealed class Message : BaseEntity
{
    public Guid Id { get; set; }

    public Guid ConversationId { get; set; }

    public Conversation Conversation { get; set; } = null!;

    public string SenderId { get; set; } = string.Empty;

    public User Sender { get; set; } = null!;

    public string? Text { get; set; }

    public ChatMessageType MessageType { get; set; }

    public string? FileUrl { get; set; }

    public Guid? ReplyToId { get; set; }

    public Message? ReplyTo { get; set; }

    /// <summary>
    /// Denormalized convenience flag (e.g. at least one recipient read); detailed receipts live in <see cref="Reads"/>.
    /// </summary>
    public bool IsRead { get; set; }

    public ICollection<Message> Replies { get; set; } = new List<Message>();

    public ICollection<MessageRead> Reads { get; set; } = new List<MessageRead>();

    public ICollection<MessageAttachment> Attachments { get; set; } = new List<MessageAttachment>();
}
