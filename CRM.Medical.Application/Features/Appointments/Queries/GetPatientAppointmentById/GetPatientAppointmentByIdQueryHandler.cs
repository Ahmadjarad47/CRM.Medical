using CRM.Medical.Application.Exceptions;
using CRM.Medical.Application.Features.Appointments;
using CRM.Medical.Application.Features.Appointments.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.Appointments.Queries.GetPatientAppointmentById;

public sealed class GetPatientAppointmentByIdQueryHandler(IAppointmentRepository appointments)
    : IRequestHandler<GetPatientAppointmentByIdQuery, AppointmentDto>
{
    public async Task<AppointmentDto> Handle(
        GetPatientAppointmentByIdQuery request,
        CancellationToken cancellationToken)
    {
        var appointment = await appointments.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Appointment '{request.Id}' was not found.");

        if (!string.Equals(appointment.PatientId, request.PatientUserId, StringComparison.Ordinal))
            throw new ApplicationForbiddenException("You cannot access this appointment.");

        return appointment.ToDto();
    }
}
