using CRM.Medical.Application.Features.Appointments.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.Appointments.CQRS;

public sealed record GetDayAppointmentAvailabilityQuery(
    DateTime Date,
    string? UserId = null) : IRequest<AppointmentDayAvailabilityDto>;
