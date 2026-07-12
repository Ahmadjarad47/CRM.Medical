using CRM.Medical.Application.Features.CategoryMedical.Services;
using MediatR;

namespace CRM.Medical.Application.Features.CategoryMedical.CQRS;

public sealed class UpdateCategoryMedicalCommandHandler(ICategoryMedicalService service)
    : IRequestHandler<UpdateCategoryMedicalCommand, Unit>
{
    public async Task<Unit> Handle(UpdateCategoryMedicalCommand request, CancellationToken cancellationToken)
    {
        await service.UpdateAsync(
            request.Id,
            request.NameAr,
            request.NameEn,
            request.Description,
            request.Image,
            request.DisplayOrder,
            request.IsActive,
            cancellationToken);
        return Unit.Value;
    }
}
