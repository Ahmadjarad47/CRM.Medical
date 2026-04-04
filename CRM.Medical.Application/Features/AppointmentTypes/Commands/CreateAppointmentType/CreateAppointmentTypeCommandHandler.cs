using CRM.Medical.Application.Common.Time;
using CRM.Medical.Application.Exceptions;
using CRM.Medical.Application.Features.AppointmentTypes;
using CRM.Medical.Application.Features.AppointmentTypes.DTOs;
using CRM.Medical.Domain.Entities;
using MediatR;

namespace CRM.Medical.Application.Features.AppointmentTypes.Commands.CreateAppointmentType;

public sealed class CreateAppointmentTypeCommandHandler(
    IAppointmentTypeRepository appointmentTypes,
    IDateTimeProvider dateTimeProvider)
    : IRequestHandler<CreateAppointmentTypeCommand, AppointmentTypeDto>
{
    public async Task<AppointmentTypeDto> Handle(
        CreateAppointmentTypeCommand request,
        CancellationToken cancellationToken)
    {
        var name = request.Name.Trim();
        if (await appointmentTypes.NameExistsAsync(name, null, cancellationToken))
            throw new ApplicationBadRequestException("An appointment type with this name already exists.");

        var entity = new AppointmentType
        {
            Name = name,
            IsActive = true,
            CreatedAt = dateTimeProvider.UtcNow
        };

        await appointmentTypes.AddAsync(entity, cancellationToken);
        return entity.ToDto();
    }
}
