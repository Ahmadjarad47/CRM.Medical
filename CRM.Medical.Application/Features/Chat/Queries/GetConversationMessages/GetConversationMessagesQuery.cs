using CRM.Medical.Application.Features.Chat.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.Chat.Queries.GetConversationMessages;

public sealed record GetConversationMessagesQuery(
    string UserId,
    Guid ConversationId,
    DateTime? BeforeUtc,
    int Take)
    : IRequest<IReadOnlyList<MessageDto>>;
