using CRM.Medical.Domain.Chat;

namespace CRM.Medical.RealTime.Dtos;

/// <summary>SignalR payload for hub SendMessage.</summary>
public sealed class SendMessageRequest
{
    public Guid ConversationId { get; set; }

    public string? Text { get; set; }

    public ChatMessageType MessageType { get; set; } = ChatMessageType.Text;

    public string? FileUrl { get; set; }

    public Guid? ReplyToId { get; set; }
}
