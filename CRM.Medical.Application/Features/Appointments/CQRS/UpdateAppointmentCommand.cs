using MediatR;

namespace CRM.Medical.Application.Features.Appointments.CQRS;

public sealed record UpdateAppointmentCommand(
    int Id,
    int TestRequestId,
    string? UserId,
    DateTime StartTime,
    DateTime EndTime,
    string PatientLocationType,
    double? PatientLatitude,
    double? PatientLongitude,
    string? Notes) : IRequest<Unit>;
