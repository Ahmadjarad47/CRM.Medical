using CRM.Medical.Application.Features.Appointments.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.Appointments.Queries.GetAdminAppointmentById;

public sealed record GetAdminAppointmentByIdQuery(int Id) : IRequest<AppointmentDto>;
