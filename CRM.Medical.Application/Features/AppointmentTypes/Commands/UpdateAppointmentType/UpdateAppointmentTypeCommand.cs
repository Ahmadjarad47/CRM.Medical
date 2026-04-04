using CRM.Medical.Application.Features.AppointmentTypes.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.AppointmentTypes.Commands.UpdateAppointmentType;

public sealed record UpdateAppointmentTypeCommand(int Id, string Name, bool IsActive) : IRequest<AppointmentTypeDto>;
