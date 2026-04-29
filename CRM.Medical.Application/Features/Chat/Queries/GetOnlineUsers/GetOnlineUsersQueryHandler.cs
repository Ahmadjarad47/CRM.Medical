using CRM.Medical.Application.Abstractions.Chat;
using CRM.Medical.Application.Features.Chat.DTOs;
using CRM.Medical.Application.Features.Chat.Services;
using MediatR;

namespace CRM.Medical.Application.Features.Chat.Queries.GetOnlineUsers;

public sealed class GetOnlineUsersQueryHandler(
    IConnectionManager connectionManager,
    IChatAuthorizationService chatAuthorization)
    : IRequestHandler<GetOnlineUsersQuery, IReadOnlyList<OnlineUserDto>>
{
    public async Task<IReadOnlyList<OnlineUserDto>> Handle(GetOnlineUsersQuery request, CancellationToken cancellationToken)
    {
        var onlineIds = await connectionManager.GetAllOnlineUserIdsAsync(cancellationToken);
        var filtered = await chatAuthorization.FilterToPeersActorMayChatAsync(request.ActorUserId, onlineIds, cancellationToken);

        return filtered
            .Select(id => new OnlineUserDto(id, IsOnline: true))
            .ToList();
    }
}
