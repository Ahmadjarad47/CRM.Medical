using CRM.Medical.Application.Features.Appointments.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.Appointments.CQRS;

public sealed record CreateAppointmentCommand(
    int AvailabilityId,
    int? TestRequestId,
    DateTime StartTime,
    DateTime EndTime,
    string PatientLocationType,
    int? Age,
    string? Gender,
    double? PatientLatitude,
    double? PatientLongitude,
    string? Notes) : IRequest<AppointmentDto>;
