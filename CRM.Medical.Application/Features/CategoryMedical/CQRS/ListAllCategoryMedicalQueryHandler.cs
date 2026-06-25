using CRM.Medical.Application.Features.CategoryMedical.DTOs;
using CRM.Medical.Application.Features.CategoryMedical.Services;
using MediatR;

namespace CRM.Medical.Application.Features.CategoryMedical.CQRS;

public sealed class ListAllCategoryMedicalQueryHandler(ICategoryMedicalService service)
    : IRequestHandler<ListAllCategoryMedicalQuery, IReadOnlyList<CategoryMedicalDto>>
{
    public Task<IReadOnlyList<CategoryMedicalDto>> Handle(
        ListAllCategoryMedicalQuery request,
        CancellationToken cancellationToken) =>
        service.ListAllAsync(request.ActiveOnly, cancellationToken);
}
