using CRM.Medical.Application.Features.Appointments.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.Appointments.Queries.GetDoctorAppointmentById;

public sealed record GetDoctorAppointmentByIdQuery(string DoctorUserId, int Id) : IRequest<AppointmentDto>;
