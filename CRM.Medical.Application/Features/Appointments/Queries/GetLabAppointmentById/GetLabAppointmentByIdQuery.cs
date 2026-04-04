using CRM.Medical.Application.Features.Appointments.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.Appointments.Queries.GetLabAppointmentById;

public sealed record GetLabAppointmentByIdQuery(string LabUserId, int Id) : IRequest<AppointmentDto>;
