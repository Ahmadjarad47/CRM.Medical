using CRM.Medical.Application.Features.Store.DTOs;
using CRM.Medical.Application.Features.Store.Services;
using MediatR;

namespace CRM.Medical.Application.Features.Store.Queries.GetStoreCategoryPage;

public sealed class GetStoreCategoryPageQueryHandler(IStoreHomeService service)
    : IRequestHandler<GetStoreCategoryPageQuery, CategoryPageDto>
{
    public Task<CategoryPageDto> Handle(GetStoreCategoryPageQuery request, CancellationToken cancellationToken) =>
        service.GetCategoryPageAsync(request.Id, cancellationToken);
}
