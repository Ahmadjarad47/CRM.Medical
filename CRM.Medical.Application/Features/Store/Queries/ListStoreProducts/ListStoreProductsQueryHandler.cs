using CRM.Medical.Application.Features.Store.DTOs;
using CRM.Medical.Application.Common.Responses;
using CRM.Medical.Application.Features.Store.Services;
using MediatR;

namespace CRM.Medical.Application.Features.Store.Queries.ListStoreProducts;

public sealed class ListStoreProductsQueryHandler(IProductCatalogService service)
    : IRequestHandler<ListStoreProductsQuery, PagedResult<ProductCardDto>>
{
    public Task<PagedResult<ProductCardDto>> Handle(ListStoreProductsQuery request, CancellationToken cancellationToken) =>
        service.ListProductsAsync(request.Page, request.PageSize, request.Search, request.CategoryId, request.ActiveOnly, cancellationToken);
}
