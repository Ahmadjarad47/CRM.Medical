using CRM.Medical.Application.Common.Responses;
using CRM.Medical.Application.Features.Appointments;
using CRM.Medical.Application.Features.Appointments.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.Appointments.Queries.ListDoctorAppointments;

public sealed class ListDoctorAppointmentsQueryHandler(IAppointmentRepository appointments)
    : IRequestHandler<ListDoctorAppointmentsQuery, PagedResult<AppointmentDto>>
{
    public async Task<PagedResult<AppointmentDto>> Handle(
        ListDoctorAppointmentsQuery request,
        CancellationToken cancellationToken)
    {
        var (items, total) = await appointments.ListForDoctorAsync(
            request.DoctorUserId,
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
