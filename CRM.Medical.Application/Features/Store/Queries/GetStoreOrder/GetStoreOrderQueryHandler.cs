using CRM.Medical.Application.Features.Store.DTOs;
using CRM.Medical.Application.Features.Store.Services;
using MediatR;

namespace CRM.Medical.Application.Features.Store.Queries.GetStoreOrder;

public sealed class GetStoreOrderQueryHandler(IStoreOrderService service)
    : IRequestHandler<GetStoreOrderQuery, StoreOrderDetailsDto>
{
    public Task<StoreOrderDetailsDto> Handle(GetStoreOrderQuery request, CancellationToken cancellationToken) =>
        service.GetOrderAsync(request.Id, cancellationToken);
}
