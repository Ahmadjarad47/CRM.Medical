using CRM.Medical.Application.Features.Appointments.DTOs;
using CRM.Medical.Application.Features.Appointments.Services;
using MediatR;

namespace CRM.Medical.Application.Features.Appointments.CQRS;

public sealed class ListAppointmentsQueryHandler(IAppointmentService appointments)
    : IRequestHandler<ListAppointmentsQuery, IReadOnlyList<AppointmentDto>>
{
    public Task<IReadOnlyList<AppointmentDto>> Handle(
        ListAppointmentsQuery request,
        CancellationToken cancellationToken) =>
        appointments.ListAsync(
            request.FromUtc,
            request.ToUtc,
            request.UserId,
            request.Status,
            cancellationToken);
}
