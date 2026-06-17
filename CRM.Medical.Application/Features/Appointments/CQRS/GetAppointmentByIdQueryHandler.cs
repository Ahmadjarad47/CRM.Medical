using CRM.Medical.Application.Features.Appointments.DTOs;
using CRM.Medical.Application.Features.Appointments.Services;
using MediatR;

namespace CRM.Medical.Application.Features.Appointments.CQRS;

public sealed class GetAppointmentByIdQueryHandler(IAppointmentService appointments)
    : IRequestHandler<GetAppointmentByIdQuery, AppointmentDto>
{
    public Task<AppointmentDto> Handle(GetAppointmentByIdQuery request, CancellationToken cancellationToken) =>
        appointments.GetByIdAsync(request.Id, cancellationToken);
}
