using CRM.Medical.Application.Features.Chat.DTOs;
using CRM.Medical.Application.Features.Chat.Persistence;
using CRM.Medical.Domain.Chat;
using MediatR;

namespace CRM.Medical.Application.Features.Chat.Queries.GetMyConversations;

public sealed class GetMyConversationsQueryHandler(IChatPersistence chatPersistence)
    : IRequestHandler<GetMyConversationsQuery, IReadOnlyList<ConversationSummaryDto>>
{
    public async Task<IReadOnlyList<ConversationSummaryDto>> Handle(GetMyConversationsQuery request, CancellationToken cancellationToken)
    {
        var list = await chatPersistence.ListConversationsForUserAsync(request.UserId, request.Skip, request.Take, cancellationToken);
        var ids = list.Select(c => c.Id).ToList();
        var lastMap = await chatPersistence.GetLastMessagesByConversationIdsAsync(ids, cancellationToken);

        var result = new List<ConversationSummaryDto>();
        foreach (var c in list)
        {
            var unread = await chatPersistence.CountUnreadForUserAsync(request.UserId, c.Id, cancellationToken);
            MessagePreviewDto? preview = null;
            if (lastMap.TryGetValue(c.Id, out var last) && last is not null)
                preview = new MessagePreviewDto(last.Text, last.MessageType, last.CreatedAt);

            result.Add(new ConversationSummaryDto(c.Id, c.Type, c.Title, c.CreatedAt, preview, unread));
        }

        return result;
    }
}
