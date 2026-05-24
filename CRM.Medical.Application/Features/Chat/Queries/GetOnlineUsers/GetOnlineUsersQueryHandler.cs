using CRM.Medical.Application.Features.Chat.DTOs;
using CRM.Medical.Application.Features.Chat.Services;
using MediatR;

namespace CRM.Medical.Application.Features.Chat.Queries.GetOnlineUsers;

public sealed class GetOnlineUsersQueryHandler(
    IChatAuthorizationService chatAuthorization,
    IChatUserSummaryLookup summaryLookup)
    : IRequestHandler<GetOnlineUsersQuery, IReadOnlyList<OnlineUserDto>>
{
    public async Task<IReadOnlyList<OnlineUserDto>> Handle(GetOnlineUsersQuery request, CancellationToken cancellationToken)
    {
        var list = (await chatAuthorization.GetPeerUserIdsActorMayChatAsync(request.ActorUserId, cancellationToken)).ToList();
        var summaries = await summaryLookup.GetSummariesAsync(list, cancellationToken);

        return list
            .Select(id =>
            {
                var s = summaries[id];
                return new OnlineUserDto(id, s.IsOnline, s);
            })
            .ToList();
    }
}
