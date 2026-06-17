using CRM.Medical.Application.Features.Store.DTOs;
using CRM.Medical.Application.Features.Store.Services;
using MediatR;

namespace CRM.Medical.Application.Features.Store.Queries.GetStoreCart;

public sealed class GetStoreCartQueryHandler(ICartService service)
    : IRequestHandler<GetStoreCartQuery, CartDto>
{
    public Task<CartDto> Handle(GetStoreCartQuery request, CancellationToken cancellationToken) =>
        service.GetAsync(request.LabClientId, cancellationToken);
}
