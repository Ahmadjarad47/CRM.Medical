using CRM.Medical.Application.Features.Chat.DTOs;
using CRM.Medical.Application.Features.Chat.Services;
using CRM.Medical.Application.Features.Users.Constants;
using MediatR;

namespace CRM.Medical.Application.Features.Chat.Queries.GetAvailableChatUsers;

public sealed class GetAvailableChatUsersQueryHandler(
    IChatAuthorizationService chatAuthorization,
    IChatUserSummaryLookup summaryLookup)
    : IRequestHandler<GetAvailableChatUsersQuery, IReadOnlyList<ChatUserSummaryDto>>
{
    public async Task<IReadOnlyList<ChatUserSummaryDto>> Handle(
        GetAvailableChatUsersQuery request,
        CancellationToken cancellationToken)
    {
        var userIds = await chatAuthorization.GetPeerUserIdsActorMayChatAsync(request.ActorUserId, cancellationToken);
        var summaries = await summaryLookup.GetSummariesAsync(userIds, cancellationToken);

        return userIds
            .Select(id => summaries[id])
            .OrderBy(summary => GetRoleOrder(summary.Role))
            .ThenBy(summary => summary.FullName, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static int GetRoleOrder(string? role) =>
        role switch
        {
            UserRoles.Admin => 0,
            UserRoles.Doctor => 1,
            UserRoles.LabPartner => 2,
            _ => 3
        };
}
