using CRM.Medical.Application.Exceptions;
using CRM.Medical.Application.Features.Appointments;
using CRM.Medical.Application.Features.Appointments.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.Appointments.Queries.GetLabAppointmentById;

public sealed class GetLabAppointmentByIdQueryHandler(IAppointmentRepository appointments)
    : IRequestHandler<GetLabAppointmentByIdQuery, AppointmentDto>
{
    public async Task<AppointmentDto> Handle(
        GetLabAppointmentByIdQuery request,
        CancellationToken cancellationToken)
    {
        var appointment = await appointments.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Appointment '{request.Id}' was not found.");

        if (!string.Equals(appointment.LabPartnerId, request.LabUserId, StringComparison.Ordinal))
            throw new ApplicationForbiddenException("You cannot access this appointment.");

        return appointment.ToDto();
    }
}
