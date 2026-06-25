using CRM.Medical.Application.Features.MedicalTests.Services;
using MediatR;

namespace CRM.Medical.Application.Features.MedicalTests.CQRS;

public sealed class UpdateMedicalTestCommandHandler(IMedicalTestService medicalTests)
    : IRequestHandler<UpdateMedicalTestCommand, Unit>
{
    public async Task<Unit> Handle(
        UpdateMedicalTestCommand request,
        CancellationToken cancellationToken)
    {
        await medicalTests.UpdateAsync(
            request.Id,
            request.NameAr,
            request.NameEn,
            request.Price,
            request.CategoryMedicalId,
            request.SampleType,
            request.ParameterSchema,
            request.Status,
            cancellationToken);
        return Unit.Value;
    }
}
