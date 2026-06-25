using CRM.Medical.Application.Features.CategoryMedical.DTOs;
using CRM.Medical.Application.Features.CategoryMedical.Services;
using MediatR;

namespace CRM.Medical.Application.Features.CategoryMedical.CQRS;

public sealed class CreateCategoryMedicalCommandHandler(ICategoryMedicalService service)
    : IRequestHandler<CreateCategoryMedicalCommand, CategoryMedicalDto>
{
    public Task<CategoryMedicalDto> Handle(
        CreateCategoryMedicalCommand request,
        CancellationToken cancellationToken) =>
        service.CreateAsync(
            request.NameAr,
            request.NameEn,
            request.Description,
            request.DisplayOrder,
            request.IsActive,
            cancellationToken);
}
