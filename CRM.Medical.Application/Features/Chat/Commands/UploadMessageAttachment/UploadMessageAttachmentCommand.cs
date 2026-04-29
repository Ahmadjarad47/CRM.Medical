using CRM.Medical.Application.Features.Chat.DTOs;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace CRM.Medical.Application.Features.Chat.Commands.UploadMessageAttachment;

public sealed record UploadMessageAttachmentCommand(string ActorUserId, Guid MessageId, IFormFile File)
    : IRequest<MessageAttachmentDto>;
