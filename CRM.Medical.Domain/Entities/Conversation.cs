using CRM.Medical.Domain.Chat;

namespace CRM.Medical.Domain.Entities;

public sealed class Conversation : BaseEntity
{
    public Guid Id { get; set; }

    public ConversationType Type { get; set; }

    /// <summary>Optional title for group conversations.</summary>
    public string? Title { get; set; }

    public User CreatedBy { get; set; } = null!;

    public ICollection<ConversationParticipant> Participants { get; set; } = new List<ConversationParticipant>();

    public ICollection<Message> Messages { get; set; } = new List<Message>();
}
