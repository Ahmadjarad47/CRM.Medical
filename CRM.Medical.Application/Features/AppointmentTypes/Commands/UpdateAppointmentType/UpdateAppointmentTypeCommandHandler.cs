using CRM.Medical.Application.Exceptions;
using CRM.Medical.Application.Features.AppointmentTypes;
using CRM.Medical.Application.Features.AppointmentTypes.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.AppointmentTypes.Commands.UpdateAppointmentType;

public sealed class UpdateAppointmentTypeCommandHandler(IAppointmentTypeRepository appointmentTypes)
    : IRequestHandler<UpdateAppointmentTypeCommand, AppointmentTypeDto>
{
    public async Task<AppointmentTypeDto> Handle(
        UpdateAppointmentTypeCommand request,
        CancellationToken cancellationToken)
    {
        var entity = await appointmentTypes.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Appointment type '{request.Id}' was not found.");

        var name = request.Name.Trim();
        if (await appointmentTypes.NameExistsAsync(name, request.Id, cancellationToken))
            throw new ApplicationBadRequestException("An appointment type with this name already exists.");

        entity.Name = name;
        entity.IsActive = request.IsActive;

        await appointmentTypes.UpdateAsync(entity, cancellationToken);
        return entity.ToDto();
    }
}
