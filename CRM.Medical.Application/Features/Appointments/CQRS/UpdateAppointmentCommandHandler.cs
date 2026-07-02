using CRM.Medical.Application.Features.Appointments.Services;
using MediatR;

namespace CRM.Medical.Application.Features.Appointments.CQRS;

public sealed class UpdateAppointmentCommandHandler(IAppointmentService appointments)
    : IRequestHandler<UpdateAppointmentCommand, Unit>
{
    public async Task<Unit> Handle(UpdateAppointmentCommand request, CancellationToken cancellationToken)
    {
        await appointments.UpdateAsync(
            request.Id,
            request.AvailabilityId,
            request.TestRequestId,
            request.UserId,
            request.PatientLocationType,
            request.PatientLatitude,
            request.PatientLongitude,
            request.Notes,
            request.Attachment,
            cancellationToken);

        return Unit.Value;
    }
}
