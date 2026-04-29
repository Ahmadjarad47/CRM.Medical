using CRM.Medical.Application.Features.Chat.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.Chat.Queries.GetMyConversations;

public sealed record GetMyConversationsQuery(string UserId, int Skip, int Take)
    : IRequest<IReadOnlyList<ConversationSummaryDto>>;
