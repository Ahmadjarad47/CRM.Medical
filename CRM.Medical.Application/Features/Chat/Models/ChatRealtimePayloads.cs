using CRM.Medical.Domain.Chat;

namespace CRM.Medical.Application.Features.Chat.Models;

public sealed record ChatMessageRealtimePayload(
    Guid MessageId,
    Guid ConversationId,
    string SenderId,
    string? SenderDisplayName,
    string? Text,
    ChatMessageType MessageType,
    string? FileUrl,
    Guid? ReplyToId,
    DateTime CreatedAtUtc);

public sealed record ChatTypingPayload(string UserId, bool IsTyping);

public sealed record ChatReadReceiptPayload(Guid MessageId, string ReaderUserId, DateTime ReadAtUtc);

/// <summary>Pushed to <c>user:{userId}</c> group when a peer sends a message (badge/list refresh).</summary>
public sealed record ConversationUpdatedPayload(Guid ConversationId, Guid MessageId);
