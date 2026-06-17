using CRM.Medical.Application.Features.Appointments.DTOs;
using CRM.Medical.Application.Features.Appointments.Services;
using MediatR;

namespace CRM.Medical.Application.Features.Appointments.CQRS;

public sealed class GetDayAppointmentAvailabilityQueryHandler(IAppointmentService appointments)
    : IRequestHandler<GetDayAppointmentAvailabilityQuery, AppointmentDayAvailabilityDto>
{
    public Task<AppointmentDayAvailabilityDto> Handle(
        GetDayAppointmentAvailabilityQuery request,
        CancellationToken cancellationToken) =>
        appointments.GetDayAvailabilityAsync(request.Date, request.UserId, cancellationToken);
}
