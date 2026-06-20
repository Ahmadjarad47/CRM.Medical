using CRM.Medical.Application.Features.Appointments.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.Appointments.CQRS;

public sealed record CreateAppointmentCommand(
    int AvailabilityId,
    int? TestRequestId,
    string PatientLocationType,
    double? PatientLatitude,
    double? PatientLongitude,
    string? Notes) : IRequest<AppointmentDto>;
