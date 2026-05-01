using CRM.Medical.Domain.Chat;

namespace CRM.Medical.Application.Features.Chat.DTOs;

public sealed record MessageDto(
    Guid Id,
    Guid ConversationId,
    string SenderId,
    string? SenderFullName,
    ChatUserSummaryDto? Sender,
    string? Text,
    ChatMessageType MessageType,
    string? FileUrl,
    Guid? ReplyToId,
    bool IsRead,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc,
    IReadOnlyList<MessageAttachmentDto> Attachments);

public sealed record MessageAttachmentDto(
    Guid Id,
    string FileName,
    string FileUrl,
    string? FileType,
    long FileSize,
    DateTime UploadedAtUtc,
    string UploadedByUserId,
    ChatUserSummaryDto? UploadedByUser);
