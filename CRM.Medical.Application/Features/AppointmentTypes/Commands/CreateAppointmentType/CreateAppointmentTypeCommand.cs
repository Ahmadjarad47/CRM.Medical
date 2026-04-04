using CRM.Medical.Application.Features.AppointmentTypes.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.AppointmentTypes.Commands.CreateAppointmentType;

public sealed record CreateAppointmentTypeCommand(string Name) : IRequest<AppointmentTypeDto>;
