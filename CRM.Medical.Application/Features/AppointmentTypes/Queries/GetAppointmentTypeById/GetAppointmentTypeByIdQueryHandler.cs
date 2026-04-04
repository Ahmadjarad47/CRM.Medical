using CRM.Medical.Application.Exceptions;
using CRM.Medical.Application.Features.AppointmentTypes;
using CRM.Medical.Application.Features.AppointmentTypes.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.AppointmentTypes.Queries.GetAppointmentTypeById;

public sealed class GetAppointmentTypeByIdQueryHandler(IAppointmentTypeRepository appointmentTypes)
    : IRequestHandler<GetAppointmentTypeByIdQuery, AppointmentTypeDto>
{
    public async Task<AppointmentTypeDto> Handle(
        GetAppointmentTypeByIdQuery request,
        CancellationToken cancellationToken)
    {
        var entity = await appointmentTypes.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Appointment type '{request.Id}' was not found.");

        return entity.ToDto();
    }
}
