using CRM.Medical.Application.Features.Chat.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.Chat.Queries.GetOnlineUsers;

public sealed record GetOnlineUsersQuery(string ActorUserId) : IRequest<IReadOnlyList<OnlineUserDto>>;
