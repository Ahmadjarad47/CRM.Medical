using CRM.Medical.Domain.Chat;

namespace CRM.Medical.Application.Features.Chat.DTOs;

public sealed record ConversationSummaryDto(
    Guid Id,
    ConversationType Type,
    string? Title,
    DateTime CreatedAtUtc,
    MessagePreviewDto? LastMessage,
    int UnreadCount);

public sealed record MessagePreviewDto(string? Text, ChatMessageType MessageType, DateTime CreatedAtUtc);
