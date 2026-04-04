using CRM.Medical.Application.Common.Responses;
using CRM.Medical.Application.Features.Appointments;
using CRM.Medical.Application.Features.Appointments.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.Appointments.Queries.ListPatientAppointments;

public sealed class ListPatientAppointmentsQueryHandler(IAppointmentRepository appointments)
    : IRequestHandler<ListPatientAppointmentsQuery, PagedResult<AppointmentDto>>
{
    public async Task<PagedResult<AppointmentDto>> Handle(
        ListPatientAppointmentsQuery request,
        CancellationToken cancellationToken)
    {
        var (items, total) = await appointments.ListForPatientAsync(
            request.PatientUserId,
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
