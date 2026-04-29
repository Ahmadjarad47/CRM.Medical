using CRM.Medical.Domain.Chat;

namespace CRM.Medical.API.Contracts.Chat;

public sealed class CreateDirectConversationRequest
{
    public string OtherUserId { get; set; } = string.Empty;
}

public sealed class CreateGroupConversationRequest
{
    public string Title { get; set; } = string.Empty;

    public IReadOnlyList<string> ParticipantUserIds { get; set; } = Array.Empty<string>();
}

public sealed class PostChatMessageRequest
{
    public Guid ConversationId { get; set; }

    public string? Text { get; set; }

    public ChatMessageType MessageType { get; set; } = ChatMessageType.Text;

    public string? FileUrl { get; set; }

    public Guid? ReplyToId { get; set; }
}
