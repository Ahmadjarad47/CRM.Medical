using CRM.Medical.Application.Features.Appointments.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.Appointments.CQRS;

public sealed record GetAppointmentByIdQuery(int Id) : IRequest<AppointmentDto>;
