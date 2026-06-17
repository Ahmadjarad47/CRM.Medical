using CRM.Medical.Application.Features.Store.DTOs;
using CRM.Medical.Application.Common.Responses;
using CRM.Medical.Domain.Enums;
using CRM.Medical.Application.Features.Store.Services;
using MediatR;

namespace CRM.Medical.Application.Features.Store.Queries.ListStoreOrders;

public sealed class ListStoreOrdersQueryHandler(IStoreOrderService service)
    : IRequestHandler<ListStoreOrdersQuery, PagedResult<StoreOrderDto>>
{
    public Task<PagedResult<StoreOrderDto>> Handle(ListStoreOrdersQuery request, CancellationToken cancellationToken) =>
        service.ListOrdersAsync(request.Page, request.PageSize, request.Search, request.Status, cancellationToken);
}
