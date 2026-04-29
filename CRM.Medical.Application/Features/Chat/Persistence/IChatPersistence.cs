using CRM.Medical.Domain.Entities;
using CRM.Medical.Domain.Chat;

namespace CRM.Medical.Application.Features.Chat.Persistence;

/// <summary>
/// Unit-of-work style data access for chat (EF implementation in Infrastructure).
/// </summary>
public interface IChatPersistence
{
    Task<Conversation?> FindActiveDirectConversationBetweenAsync(string userIdA, string userIdB, CancellationToken cancellationToken = default);

    Task<Conversation?> GetConversationWithParticipantsAsync(Guid conversationId, CancellationToken cancellationToken = default);

    Task AddConversationAsync(Conversation conversation, CancellationToken cancellationToken = default);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    Task<bool> IsActiveParticipantAsync(string userId, Guid conversationId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ConversationParticipant>> GetActiveParticipantsAsync(Guid conversationId, CancellationToken cancellationToken = default);

    Task<Message?> GetMessageAsync(Guid messageId, CancellationToken cancellationToken = default);

    Task AddMessageAsync(Message message, CancellationToken cancellationToken = default);

    Task<int> CountUnreadForUserAsync(string userId, Guid conversationId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Conversation>> ListConversationsForUserAsync(string userId, int skip, int take, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Message>> ListMessagesAsync(Guid conversationId, DateTime? beforeUtc, int take, CancellationToken cancellationToken = default);

    Task<Dictionary<Guid, Message?>> GetLastMessagesByConversationIdsAsync(IReadOnlyCollection<Guid> conversationIds, CancellationToken cancellationToken = default);

    Task AddMessageReadAsync(MessageRead read, CancellationToken cancellationToken = default);

    Task<MessageAttachment?> GetAttachmentByIdAsync(Guid attachmentId, CancellationToken cancellationToken = default);

    Task AddMessageAttachmentAsync(MessageAttachment attachment, CancellationToken cancellationToken = default);

    Task<ConversationParticipant?> GetParticipantAsync(string userId, Guid conversationId, CancellationToken cancellationToken = default);

    Task AddParticipantsAsync(IReadOnlyCollection<ConversationParticipant> participants, CancellationToken cancellationToken = default);

    Task<bool> HasUserReadMessageAsync(Guid messageId, string userId, CancellationToken cancellationToken = default);

    Task<Message?> GetTrackedMessageAsync(Guid messageId, CancellationToken cancellationToken = default);

    Task LeaveConversationAsync(string userId, Guid conversationId, DateTime leftAtUtc, CancellationToken cancellationToken = default);
}
