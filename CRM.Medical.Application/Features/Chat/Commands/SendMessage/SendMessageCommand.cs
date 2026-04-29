using CRM.Medical.Application.Features.Chat.DTOs;
using CRM.Medical.Domain.Chat;
using MediatR;

namespace CRM.Medical.Application.Features.Chat.Commands.SendMessage;

public sealed record SendMessageCommand(
    string SenderUserId,
    Guid ConversationId,
    string? Text,
    ChatMessageType MessageType,
    string? FileUrl,
    Guid? ReplyToId)
    : IRequest<MessageDto>;
