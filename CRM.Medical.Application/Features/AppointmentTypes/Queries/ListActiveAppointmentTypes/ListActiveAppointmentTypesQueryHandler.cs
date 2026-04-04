using CRM.Medical.Application.Features.AppointmentTypes;
using CRM.Medical.Application.Features.AppointmentTypes.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.AppointmentTypes.Queries.ListActiveAppointmentTypes;

public sealed class ListActiveAppointmentTypesQueryHandler(IAppointmentTypeRepository appointmentTypes)
    : IRequestHandler<ListActiveAppointmentTypesQuery, IReadOnlyList<AppointmentTypeDto>>
{
    public async Task<IReadOnlyList<AppointmentTypeDto>> Handle(
        ListActiveAppointmentTypesQuery request,
        CancellationToken cancellationToken)
    {
        var items = await appointmentTypes.ListActiveAsync(cancellationToken);
        return items.Select(t => t.ToDto()).ToList();
    }
}
