using CRM.Medical.Application.Features.CategoryMedical.Services;
using MediatR;

namespace CRM.Medical.Application.Features.CategoryMedical.CQRS;

public sealed class DeleteCategoryMedicalCommandHandler(ICategoryMedicalService service)
    : IRequestHandler<DeleteCategoryMedicalCommand, Unit>
{
    public async Task<Unit> Handle(DeleteCategoryMedicalCommand request, CancellationToken cancellationToken)
    {
        await service.DeleteAsync(request.Id, cancellationToken);
        return Unit.Value;
    }
}
