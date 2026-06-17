using CRM.Medical.Application.Features.Store.DTOs;
using CRM.Medical.Application.Features.Store.Services;
using MediatR;

namespace CRM.Medical.Application.Features.Store.Queries.ListStoreSliders;

public sealed class ListStoreSlidersQueryHandler(IStoreAdminService service)
    : IRequestHandler<ListStoreSlidersQuery, IReadOnlyList<StoreSliderDto>>
{
    public Task<IReadOnlyList<StoreSliderDto>> Handle(ListStoreSlidersQuery request, CancellationToken cancellationToken) =>
        service.ListSlidersAsync(cancellationToken);
}
