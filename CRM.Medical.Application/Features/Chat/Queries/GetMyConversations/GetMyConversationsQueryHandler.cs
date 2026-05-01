using CRM.Medical.Application.Features.Chat.DTOs;
using CRM.Medical.Application.Features.Chat.Persistence;
using CRM.Medical.Application.Features.Chat.Services;
using CRM.Medical.Domain.Chat;
using MediatR;

namespace CRM.Medical.Application.Features.Chat.Queries.GetMyConversations;

public sealed class GetMyConversationsQueryHandler(IChatPersistence chatPersistence, IChatUserSummaryLookup summaryLookup)
    : IRequestHandler<GetMyConversationsQuery, IReadOnlyList<ConversationSummaryDto>>
{
    public async Task<IReadOnlyList<ConversationSummaryDto>> Handle(GetMyConversationsQuery request, CancellationToken cancellationToken)
    {
        var list = await chatPersistence.ListConversationsForUserAsync(request.UserId, request.Skip, request.Take, cancellationToken);
        var ids = list.Select(c => c.Id).ToList();
        var lastMap = await chatPersistence.GetLastMessagesByConversationIdsAsync(ids, cancellationToken);

        var userIds = new HashSet<string>(StringComparer.Ordinal);
        foreach (var c in list)
        {
            if (!string.IsNullOrEmpty(c.CreatedByUserId))
                userIds.Add(c.CreatedByUserId);

            if (lastMap.TryGetValue(c.Id, out var lm) && lm is not null && !string.IsNullOrEmpty(lm.SenderId))
                userIds.Add(lm.SenderId);
        }

        var summaries = await summaryLookup.GetSummariesAsync(userIds.ToList(), cancellationToken);

        var result = new List<ConversationSummaryDto>();
        foreach (var c in list)
        {
            var unread = await chatPersistence.CountUnreadForUserAsync(request.UserId, c.Id, cancellationToken);
            MessagePreviewDto? preview = null;

            summaries.TryGetValue(c.CreatedByUserId ?? string.Empty, out var createdByUser);
            if (string.IsNullOrEmpty(c.CreatedByUserId))
                createdByUser = null;

            if (lastMap.TryGetValue(c.Id, out var last) && last is not null)
            {
                summaries.TryGetValue(last.SenderId, out var lastSender);
                preview = new MessagePreviewDto(last.Text, last.MessageType, last.CreatedAt, last.SenderId, lastSender);
            }

            result.Add(new ConversationSummaryDto(
                c.Id,
                c.Type,
                c.Title,
                c.CreatedAt,
                c.CreatedByUserId,
                createdByUser,
                preview,
                unread));
        }

        return result;
    }
}
