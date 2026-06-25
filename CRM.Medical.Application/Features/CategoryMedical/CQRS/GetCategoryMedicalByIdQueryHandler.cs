using CRM.Medical.Application.Features.CategoryMedical.DTOs;
using CRM.Medical.Application.Features.CategoryMedical.Services;
using MediatR;

namespace CRM.Medical.Application.Features.CategoryMedical.CQRS;

public sealed class GetCategoryMedicalByIdQueryHandler(ICategoryMedicalService service)
    : IRequestHandler<GetCategoryMedicalByIdQuery, CategoryMedicalDto>
{
    public Task<CategoryMedicalDto> Handle(
        GetCategoryMedicalByIdQuery request,
        CancellationToken cancellationToken) =>
        service.GetByIdAsync(request.Id, cancellationToken);
}
