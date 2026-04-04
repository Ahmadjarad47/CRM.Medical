using CRM.Medical.Application.Exceptions;
using CRM.Medical.Application.Features.Appointments;
using CRM.Medical.Application.Features.Appointments.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.Appointments.Queries.GetDoctorAppointmentById;

public sealed class GetDoctorAppointmentByIdQueryHandler(IAppointmentRepository appointments)
    : IRequestHandler<GetDoctorAppointmentByIdQuery, AppointmentDto>
{
    public async Task<AppointmentDto> Handle(
        GetDoctorAppointmentByIdQuery request,
        CancellationToken cancellationToken)
    {
        var appointment = await appointments.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Appointment '{request.Id}' was not found.");

        if (!string.Equals(appointment.DoctorId, request.DoctorUserId, StringComparison.Ordinal))
            throw new ApplicationForbiddenException("You cannot access this appointment.");

        return appointment.ToDto();
    }
}
