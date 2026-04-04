using CRM.Medical.Application.Features.AppointmentTypes;
using CRM.Medical.Application.Features.AppointmentTypes.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.AppointmentTypes.Queries.ListAdminAppointmentTypes;

public sealed class ListAdminAppointmentTypesQueryHandler(IAppointmentTypeRepository appointmentTypes)
    : IRequestHandler<ListAdminAppointmentTypesQuery, IReadOnlyList<AppointmentTypeDto>>
{
    public async Task<IReadOnlyList<AppointmentTypeDto>> Handle(
        ListAdminAppointmentTypesQuery request,
        CancellationToken cancellationToken)
    {
        var items = await appointmentTypes.ListAllAsync(cancellationToken);
        return items.Select(t => t.ToDto()).ToList();
    }
}
