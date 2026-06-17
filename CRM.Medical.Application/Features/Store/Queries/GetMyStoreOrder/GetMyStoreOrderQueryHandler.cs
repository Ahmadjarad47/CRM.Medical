using CRM.Medical.Application.Features.Store.DTOs;
using CRM.Medical.Application.Features.Store.Services;
using MediatR;

namespace CRM.Medical.Application.Features.Store.Queries.GetMyStoreOrder;

public sealed class GetMyStoreOrderQueryHandler(IStoreOrderService service)
    : IRequestHandler<GetMyStoreOrderQuery, StoreOrderDetailsDto>
{
    public Task<StoreOrderDetailsDto> Handle(GetMyStoreOrderQuery request, CancellationToken cancellationToken) =>
        service.GetMyOrderAsync(request.LabClientId, request.Id, cancellationToken);
}
