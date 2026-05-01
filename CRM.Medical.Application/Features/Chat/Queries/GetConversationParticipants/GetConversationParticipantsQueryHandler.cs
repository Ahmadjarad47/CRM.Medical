using CRM.Medical.Application.Features.Chat.DTOs;
using CRM.Medical.Application.Features.Chat.Persistence;
using CRM.Medical.Application.Features.Chat.Services;
using MediatR;

namespace CRM.Medical.Application.Features.Chat.Queries.GetConversationParticipants;

public sealed class GetConversationParticipantsQueryHandler(
    IChatPersistence chatPersistence,
    IChatAuthorizationService chatAuthorization,
    IChatUserSummaryLookup summaryLookup)
    : IRequestHandler<GetConversationParticipantsQuery, IReadOnlyList<ConversationParticipantDto>>
{
    public async Task<IReadOnlyList<ConversationParticipantDto>> Handle(
        GetConversationParticipantsQuery request,
        CancellationToken cancellationToken)
    {
        await chatAuthorization.EnsureActiveParticipantAsync(request.UserId, request.ConversationId, cancellationToken);

        var rows = await chatPersistence.GetActiveParticipantsAsync(request.ConversationId, cancellationToken);
        var ids = rows.Select(r => r.UserId).Distinct().ToList();
        var summaries = await summaryLookup.GetSummariesAsync(ids, cancellationToken);

        return rows
            .Select(p =>
            {
                var s = summaries[p.UserId];
                return new ConversationParticipantDto(
                    p.Id,
                    p.UserId,
                    s.FullName,
                    s,
                    p.Role,
                    p.JoinedAt,
                    p.LeftAt);
            })
            .ToList();
    }
}
