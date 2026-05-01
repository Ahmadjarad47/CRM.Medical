using CRM.Medical.Application.Features.Chat.DTOs;
using CRM.Medical.Domain.Chat;

namespace CRM.Medical.Application.Features.Chat.Models;

public sealed record ChatMessageRealtimePayload(
    Guid MessageId,
    Guid ConversationId,
    string SenderId,
    string? SenderDisplayName,
    ChatUserSummaryDto? Sender,
    string? Text,
    ChatMessageType MessageType,
    string? FileUrl,
    Guid? ReplyToId,
    DateTime CreatedAtUtc);

public sealed record ChatTypingPayload(
    string UserId,
    string? DisplayName,
    bool IsTyping,
    ChatUserSummaryDto? User);

public sealed record ChatReadReceiptPayload(
    Guid MessageId,
    string ReaderUserId,
    DateTime ReadAtUtc,
    ChatUserSummaryDto? Reader);

/// <summary>Pushed to <c>user:{userId}</c> group when a peer sends a message (badge/list refresh).</summary>
public sealed record ConversationUpdatedPayload(Guid ConversationId, Guid MessageId);
