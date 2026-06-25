using CRM.Medical.Application.Common.Responses;
using CRM.Medical.Application.Features.CategoryMedical.DTOs;
using CRM.Medical.Application.Features.CategoryMedical.Services;
using MediatR;

namespace CRM.Medical.Application.Features.CategoryMedical.CQRS;

public sealed class ListCategoryMedicalQueryHandler(ICategoryMedicalService service)
    : IRequestHandler<ListCategoryMedicalQuery, PagedResult<CategoryMedicalDto>>
{
    public Task<PagedResult<CategoryMedicalDto>> Handle(
        ListCategoryMedicalQuery request,
        CancellationToken cancellationToken) =>
        service.ListAsync(request.Page, request.PageSize, request.Search, request.ActiveOnly, cancellationToken);
}
