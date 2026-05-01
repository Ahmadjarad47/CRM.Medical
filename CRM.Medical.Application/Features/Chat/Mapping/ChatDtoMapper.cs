using CRM.Medical.Application.Features.Chat.DTOs;
using CRM.Medical.Domain.Chat;
using CRM.Medical.Domain.Entities;

namespace CRM.Medical.Application.Features.Chat.Mapping;

public static class ChatDtoMapper
{
    public static MessageDto ToDto(this Message message, ChatUserSummaryDto? sender)
    {
        var senderFullName = string.IsNullOrWhiteSpace(sender?.FullName) ? null : sender!.FullName;
        var attachments = (message.Attachments ?? [])
            .OrderBy(a => a.UploadedAt)
            .Select(a => new MessageAttachmentDto(
                a.Id,
                a.FileName,
                a.FileUrl,
                a.FileType,
                a.FileSize,
                a.UploadedAt,
                message.SenderId,
                sender))
            .ToList()
            .AsReadOnly();

        return new MessageDto(
            message.Id,
            message.ConversationId,
            message.SenderId,
            senderFullName,
            sender,
            message.Text,
            message.MessageType,
            message.FileUrl,
            message.ReplyToId,
            message.IsRead,
            message.CreatedAt,
            message.UpdatedAt,
            attachments);
    }
}
