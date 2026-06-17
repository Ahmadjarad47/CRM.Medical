using CRM.Medical.Application.Features.Store.DTOs;
using CRM.Medical.Application.Features.Store.Services;
using MediatR;

namespace CRM.Medical.Application.Features.Store.Queries.GetStoreCategory;

public sealed class GetStoreCategoryQueryHandler(IProductCatalogService service)
    : IRequestHandler<GetStoreCategoryQuery, ProductCategoryDto>
{
    public Task<ProductCategoryDto> Handle(GetStoreCategoryQuery request, CancellationToken cancellationToken) =>
        service.GetCategoryAsync(request.Id, request.ActiveOnly, cancellationToken);
}
