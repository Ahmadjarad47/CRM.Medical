using CRM.Medical.Application.Common.Time;
using CRM.Medical.Application.Features.Banners.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.Banners.Queries.ListWebsiteBanners;

public sealed class ListWebsiteBannersQueryHandler(
    IBannerRepository repository,
    IDateTimeProvider dateTimeProvider)
    : IRequestHandler<ListWebsiteBannersQuery, IReadOnlyList<BannerDto>>
{
    public async Task<IReadOnlyList<BannerDto>> Handle(
        ListWebsiteBannersQuery request,
        CancellationToken cancellationToken)
    {
        var now = dateTimeProvider.UtcNow;
        var items = await repository.ListForWebsiteAsync(now, request.Location, cancellationToken);
        return items.Select(b => b.ToDto()).ToList();
    }
}

