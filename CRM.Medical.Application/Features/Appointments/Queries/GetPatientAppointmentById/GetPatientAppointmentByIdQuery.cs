using CRM.Medical.Application.Features.Appointments.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.Appointments.Queries.GetPatientAppointmentById;

public sealed record GetPatientAppointmentByIdQuery(string PatientUserId, int Id) : IRequest<AppointmentDto>;
