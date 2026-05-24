using CRM.Medical.Application.Features.Chat.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.Chat.Queries.GetAvailableChatUsers;

public sealed record GetAvailableChatUsersQuery(string ActorUserId) : IRequest<IReadOnlyList<ChatUserSummaryDto>>;
