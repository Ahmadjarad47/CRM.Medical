using CRM.Medical.Application.Features.Store.DTOs;
using CRM.Medical.Application.Common.Responses;
using CRM.Medical.Application.Features.Store.Services;
using MediatR;

namespace CRM.Medical.Application.Features.Store.Queries.ListMyStoreOrders;

public sealed class ListMyStoreOrdersQueryHandler(IStoreOrderService service)
    : IRequestHandler<ListMyStoreOrdersQuery, PagedResult<StoreOrderDto>>
{
    public Task<PagedResult<StoreOrderDto>> Handle(ListMyStoreOrdersQuery request, CancellationToken cancellationToken) =>
        service.ListMyOrdersAsync(request.LabClientId, request.Page, request.PageSize, cancellationToken);
}
