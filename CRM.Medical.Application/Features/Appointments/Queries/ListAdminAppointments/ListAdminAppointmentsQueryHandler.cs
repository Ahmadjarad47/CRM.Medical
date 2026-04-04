using CRM.Medical.Application.Common.Responses;
using CRM.Medical.Application.Features.Appointments;
using CRM.Medical.Application.Features.Appointments.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.Appointments.Queries.ListAdminAppointments;

public sealed class ListAdminAppointmentsQueryHandler(IAppointmentRepository appointments)
    : IRequestHandler<ListAdminAppointmentsQuery, PagedResult<AppointmentDto>>
{
    public async Task<PagedResult<AppointmentDto>> Handle(
        ListAdminAppointmentsQuery request,
        CancellationToken cancellationToken)
    {
        var (items, total) = await appointments.ListAdminAsync(
            request.PatientId,
            request.DoctorId,
            request.LabPartnerId,
            request.Status,
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
