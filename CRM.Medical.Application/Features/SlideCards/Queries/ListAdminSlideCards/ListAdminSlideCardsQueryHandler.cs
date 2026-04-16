using CRM.Medical.Application.Features.SlideCards.DTOs;
using CRM.Medical.Application.Features.SlideCards;
using MediatR;

namespace CRM.Medical.Application.Features.SlideCards.Queries.ListAdminSlideCards;

public sealed class ListAdminSlideCardsQueryHandler(
    ISlideCardRepository repository)
    : IRequestHandler<ListAdminSlideCardsQuery, IReadOnlyList<SlideCardDto>>
{
    public async Task<IReadOnlyList<SlideCardDto>> Handle(
        ListAdminSlideCardsQuery request,
        CancellationToken cancellationToken)
    {
        var items = await repository.ListActiveAsync(cancellationToken);
        return items.Select(s => s.ToDto()).ToList();
    }
}

