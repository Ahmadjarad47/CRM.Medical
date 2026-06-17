using CRM.Medical.Application.Features.Ads.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.Ads.Queries.ListAds;

public sealed class ListAdsQueryHandler(IAdRepository ads)
    : IRequestHandler<ListAdsQuery, IReadOnlyList<AdDto>>
{
    public async Task<IReadOnlyList<AdDto>> Handle(ListAdsQuery request, CancellationToken cancellationToken)
    {
        var items = await ads.ListAsync(cancellationToken);
        return items.Select(a => a.ToDto()).ToList();
    }
}
