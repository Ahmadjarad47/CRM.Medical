using CRM.Medical.Application.Common.Storage;
using CRM.Medical.Application.Common.Time;
using CRM.Medical.Application.Exceptions;
using CRM.Medical.Application.Features.Chat.DTOs;
using CRM.Medical.Application.Features.Chat.Persistence;
using CRM.Medical.Application.Features.Chat.Services;
using CRM.Medical.Domain.Chat;
using CRM.Medical.Domain.Entities;
using MediatR;

namespace CRM.Medical.Application.Features.Chat.Commands.UploadMessageAttachment;

public sealed class UploadMessageAttachmentCommandHandler(
    IChatPersistence chatPersistence,
    IChatAuthorizationService chatAuthorization,
    IFileStorageService fileStorage,
    IDateTimeProvider dateTimeProvider)
    : IRequestHandler<UploadMessageAttachmentCommand, MessageAttachmentDto>
{
    private const string Folder = "chat/attachments";

    public async Task<MessageAttachmentDto> Handle(UploadMessageAttachmentCommand request, CancellationToken cancellationToken)
    {
        var message = await chatPersistence.GetTrackedMessageAsync(request.MessageId, cancellationToken)
            ?? throw new ApplicationNotFoundException("Message was not found.");

        await chatAuthorization.EnsureActiveParticipantAsync(request.ActorUserId, message.ConversationId, cancellationToken);

        if (!string.Equals(message.SenderId, request.ActorUserId, StringComparison.Ordinal))
            throw new ApplicationForbiddenException("Only the sender may attach files to this message.");

        var url = await fileStorage.UploadFileAsync(request.File, Folder, cancellationToken);

        var now = dateTimeProvider.UtcNow;
        var attachment = new MessageAttachment
        {
            Id = Guid.NewGuid(),
            MessageId = message.Id,
            FileName = Path.GetFileName(request.File.FileName),
            FileUrl = url,
            FileType = request.File.ContentType,
            FileSize = request.File.Length,
            UploadedAt = now,
            CreatedAt = now
        };

        await chatPersistence.AddMessageAttachmentAsync(attachment, cancellationToken);

        message.MessageType = ChatMessageType.File;
        message.FileUrl ??= url;
        message.UpdatedAt = now;

        await chatPersistence.SaveChangesAsync(cancellationToken);

        return new MessageAttachmentDto(
            attachment.Id,
            attachment.FileName,
            attachment.FileUrl,
            attachment.FileType,
            attachment.FileSize,
            attachment.UploadedAt);
    }
}
