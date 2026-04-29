using CRM.Medical.Application.Features.Chat.DTOs;
using CRM.Medical.Domain.Chat;
using CRM.Medical.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace CRM.Medical.Application.Features.Chat.Mapping;

public static class ChatDtoMapper
{
    public static MessageDto ToDto(this Message message, User? sender)
    {
        var attachments = (message.Attachments ?? [])
            .OrderBy(a => a.UploadedAt)
            .Select(a => new MessageAttachmentDto(
                a.Id,
                a.FileName,
                a.FileUrl,
                a.FileType,
                a.FileSize,
                a.UploadedAt))
            .ToList()
            .AsReadOnly();

        return new MessageDto(
            message.Id,
            message.ConversationId,
            message.SenderId,
            sender?.FullName,
            message.Text,
            message.MessageType,
            message.FileUrl,
            message.ReplyToId,
            message.IsRead,
            message.CreatedAt,
            message.UpdatedAt,
            attachments);
    }

    public static async Task<MessageDto> ToDtoAsync(this Message message, UserManager<User> userManager)
    {
        var sender = await userManager.FindByIdAsync(message.SenderId);
        return message.ToDto(sender);
    }
}
