using CRM.Medical.Application.Common.Responses;
using CRM.Medical.Application.Features.Appointments;
using CRM.Medical.Application.Features.Appointments.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.Appointments.Queries.ListLabAppointments;

public sealed class ListLabAppointmentsQueryHandler(IAppointmentRepository appointments)
    : IRequestHandler<ListLabAppointmentsQuery, PagedResult<AppointmentDto>>
{
    public async Task<PagedResult<AppointmentDto>> Handle(
        ListLabAppointmentsQuery request,
        CancellationToken cancellationToken)
    {
        var (items, total) = await appointments.ListForLabAsync(
            request.LabUserId,
            request.Page,
            request.PageSize,
            cancellationToken);

        return new PagedResult<AppointmentDto>
        {
            Items = items.Select(a => a.ToDto()).ToList(),
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = total
        };
    }
}
