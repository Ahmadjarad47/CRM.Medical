using CRM.Medical.Application.Features.AppointmentTypes.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.AppointmentTypes.Queries.GetAppointmentTypeById;

public sealed record GetAppointmentTypeByIdQuery(int Id) : IRequest<AppointmentTypeDto>;
