using CRM.Medical.Application.Exceptions;
using CRM.Medical.Application.Features.Appointments;
using CRM.Medical.Application.Features.Appointments.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.Appointments.Queries.GetAdminAppointmentById;

public sealed class GetAdminAppointmentByIdQueryHandler(IAppointmentRepository appointments)
    : IRequestHandler<GetAdminAppointmentByIdQuery, AppointmentDto>
{
    public async Task<AppointmentDto> Handle(
        GetAdminAppointmentByIdQuery request,
        CancellationToken cancellationToken)
    {
        var appointment = await appointments.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Appointment '{request.Id}' was not found.");

        return appointment.ToDto();
    }
}
