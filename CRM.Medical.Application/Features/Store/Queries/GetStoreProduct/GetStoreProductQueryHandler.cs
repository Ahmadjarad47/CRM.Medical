using CRM.Medical.Application.Features.Store.DTOs;
using CRM.Medical.Application.Features.Store.Services;
using MediatR;

namespace CRM.Medical.Application.Features.Store.Queries.GetStoreProduct;

public sealed class GetStoreProductQueryHandler(IProductCatalogService service)
    : IRequestHandler<GetStoreProductQuery, ProductDetailsDto>
{
    public Task<ProductDetailsDto> Handle(GetStoreProductQuery request, CancellationToken cancellationToken) =>
        service.GetProductAsync(request.Id, request.ActiveOnly, cancellationToken);
}
