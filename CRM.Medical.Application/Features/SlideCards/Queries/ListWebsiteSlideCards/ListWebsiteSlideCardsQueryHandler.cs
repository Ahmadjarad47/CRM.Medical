using CRM.Medical.Application.Common.Time;
using CRM.Medical.Application.Features.SlideCards.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.SlideCards.Queries.ListWebsiteSlideCards;

public sealed class ListWebsiteSlideCardsQueryHandler(
    ISlideCardRepository repository,
    IDateTimeProvider dateTimeProvider)
    : IRequestHandler<ListWebsiteSlideCardsQuery, IReadOnlyList<SlideCardDto>>
{
    public async Task<IReadOnlyList<SlideCardDto>> Handle(
        ListWebsiteSlideCardsQuery request,
        CancellationToken cancellationToken)
    {
        var now = dateTimeProvider.UtcNow;
        var items = await repository.ListForWebsiteAsync(now, cancellationToken);
        return items.Select(s => s.ToDto()).ToList();
    }
}

