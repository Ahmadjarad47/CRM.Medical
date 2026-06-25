using CRM.Medical.Application.Features.MedicalTests.Services;
using MediatR;

namespace CRM.Medical.Application.Features.MedicalTests.CQRS;

public sealed class ToggleMedicalTestStatusCommandHandler(IMedicalTestService medicalTests)
    : IRequestHandler<ToggleMedicalTestStatusCommand, Unit>
{
    public async Task<Unit> Handle(ToggleMedicalTestStatusCommand request, CancellationToken cancellationToken)
    {
        await medicalTests.ToggleStatusAsync(request.Id, request.Status, cancellationToken);
        return Unit.Value;
    }
}
