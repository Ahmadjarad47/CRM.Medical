using CRM.Medical.Domain.Chat;

namespace CRM.Medical.Domain.Entities;

public sealed class ConversationParticipant : BaseEntity
{
    public Guid Id { get; set; }

    public Guid ConversationId { get; set; }

    public Conversation Conversation { get; set; } = null!;

    public string UserId { get; set; } = string.Empty;

    public User User { get; set; } = null!;

    public DateTime JoinedAt { get; set; }

    /// <summary>When set, the user left the conversation (soft leave).</summary>
    public DateTime? LeftAt { get; set; }

    public ConversationParticipantRole Role { get; set; }
}
