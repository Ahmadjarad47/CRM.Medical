using MediatR;

namespace CRM.Medical.Application.Features.Appointments.CQRS;

public sealed record UpdateAppointmentCommand(
    int Id,
    int AvailabilityId,
    int TestRequestId,
    string? UserId,
    string PatientLocationType,
    int? Age,
    string? Gender,
    double? PatientLatitude,
    double? PatientLongitude,
    string? Notes) : IRequest<Unit>;
