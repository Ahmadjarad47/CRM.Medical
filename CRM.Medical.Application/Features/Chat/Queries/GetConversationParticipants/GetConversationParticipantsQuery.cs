using CRM.Medical.Application.Features.Chat.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.Chat.Queries.GetConversationParticipants;

public sealed record GetConversationParticipantsQuery(string UserId, Guid ConversationId)
    : IRequest<IReadOnlyList<ConversationParticipantDto>>;
