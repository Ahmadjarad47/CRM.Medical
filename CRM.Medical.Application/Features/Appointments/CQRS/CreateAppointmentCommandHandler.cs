using CRM.Medical.Application.Features.Appointments.DTOs;
using CRM.Medical.Application.Features.Appointments.Services;
using MediatR;

namespace CRM.Medical.Application.Features.Appointments.CQRS;

public sealed class CreateAppointmentCommandHandler(IAppointmentService appointments)
    : IRequestHandler<CreateAppointmentCommand, AppointmentDto>
{
    public Task<AppointmentDto> Handle(
        CreateAppointmentCommand request,
        CancellationToken cancellationToken) =>
        appointments.CreateAsync(
            request.AvailabilityId,
            request.TestRequestId,
            request.PatientLocationType,
            request.PatientLatitude,
            request.PatientLongitude,
            request.Notes,
            cancellationToken);
}
