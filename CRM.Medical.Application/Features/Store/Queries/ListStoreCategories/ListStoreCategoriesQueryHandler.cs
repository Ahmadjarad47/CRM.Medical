using CRM.Medical.Application.Features.Store.DTOs;
using CRM.Medical.Application.Features.Store.Services;
using MediatR;

namespace CRM.Medical.Application.Features.Store.Queries.ListStoreCategories;

public sealed class ListStoreCategoriesQueryHandler(IProductCatalogService service)
    : IRequestHandler<ListStoreCategoriesQuery, IReadOnlyList<ProductCategoryDto>>
{
    public Task<IReadOnlyList<ProductCategoryDto>> Handle(ListStoreCategoriesQuery request, CancellationToken cancellationToken) =>
        service.ListCategoriesAsync(request.ActiveOnly, cancellationToken);
}
