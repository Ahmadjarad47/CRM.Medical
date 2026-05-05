using CRM.Medical.Application.Features.MedicalTests.Services;
using MediatR;

namespace CRM.Medical.Application.Features.MedicalTests.CQRS;

public sealed class DeleteMedicalTestCommandHandler(IMedicalTestService medicalTests)
    : IRequestHandler<DeleteMedicalTestCommand, Unit>
{
    public async Task<Unit> Handle(DeleteMedicalTestCommand request, CancellationToken cancellationToken)
    {
        await medicalTests.DeleteAsync(request.Id, cancellationToken);
        return Unit.Value;
    }
}
