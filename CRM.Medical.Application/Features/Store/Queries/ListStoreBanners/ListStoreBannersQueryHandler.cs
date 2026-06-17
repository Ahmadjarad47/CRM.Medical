using CRM.Medical.Application.Features.Store.DTOs;
using CRM.Medical.Application.Features.Store.Services;
using MediatR;

namespace CRM.Medical.Application.Features.Store.Queries.ListStoreBanners;

public sealed class ListStoreBannersQueryHandler(IStoreAdminService service)
    : IRequestHandler<ListStoreBannersQuery, IReadOnlyList<StoreBannerDto>>
{
    public Task<IReadOnlyList<StoreBannerDto>> Handle(ListStoreBannersQuery request, CancellationToken cancellationToken) =>
        service.ListBannersAsync(cancellationToken);
}
