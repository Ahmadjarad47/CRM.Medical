using CRM.Medical.Application.Features.TestRequests.Services;
using MediatR;

namespace CRM.Medical.Application.Features.TestRequests.CQRS;

public sealed class UpdateTestRequestCommandHandler(ITestRequestService testRequests)
    : IRequestHandler<UpdateTestRequestCommand, Unit>
{
    public async Task<Unit> Handle(UpdateTestRequestCommand request, CancellationToken cancellationToken)
    {
        await testRequests.UpdateAsync(
            request.Id,
            request.RequestDate,
            request.Status,
            request.TotalAmount,
            request.Notes,
            request.Metadata,
            request.DoctorId,
            request.LabClientId,
            request.DirectPatientId,
            request.ExternalPatientId,
            cancellationToken);
        return Unit.Value;
    }
}
