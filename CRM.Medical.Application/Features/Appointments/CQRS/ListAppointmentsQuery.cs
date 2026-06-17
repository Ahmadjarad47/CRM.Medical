using CRM.Medical.Application.Features.Appointments.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.Appointments.CQRS;

public sealed record ListAppointmentsQuery(
    DateTime? FromUtc = null,
    DateTime? ToUtc = null,
    string? UserId = null,
    string? Status = null) : IRequest<IReadOnlyList<AppointmentDto>>;
