using CRM.Medical.Application.Features.Chat.DTOs;
using CRM.Medical.Application.Features.Chat.Mapping;
using CRM.Medical.Application.Features.Chat.Persistence;
using CRM.Medical.Application.Features.Chat.Services;
using MediatR;

namespace CRM.Medical.Application.Features.Chat.Queries.GetConversationMessages;

public sealed class GetConversationMessagesQueryHandler(
    IChatPersistence chatPersistence,
    IChatAuthorizationService chatAuthorization,
    IChatUserSummaryLookup summaryLookup)
    : IRequestHandler<GetConversationMessagesQuery, IReadOnlyList<MessageDto>>
{
    public async Task<IReadOnlyList<MessageDto>> Handle(GetConversationMessagesQuery request, CancellationToken cancellationToken)
    {
        await chatAuthorization.EnsureActiveParticipantAsync(request.UserId, request.ConversationId, cancellationToken);

        var take = Math.Clamp(request.Take, 1, 200);
        var messages = await chatPersistence.ListMessagesAsync(request.ConversationId, request.BeforeUtc, take, cancellationToken);

        var chronological = messages.OrderBy(m => m.CreatedAt).ToList();

        var senderIds = chronological.Select(m => m.SenderId).Distinct().ToList();
        var map = await summaryLookup.GetSummariesAsync(senderIds, cancellationToken);

        var dtos = new List<MessageDto>();
        foreach (var m in chronological)
            dtos.Add(m.ToDto(map[m.SenderId]));

        return dtos;
    }
}
