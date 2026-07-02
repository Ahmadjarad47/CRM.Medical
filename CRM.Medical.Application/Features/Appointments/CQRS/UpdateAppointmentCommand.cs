using MediatR;
using Microsoft.AspNetCore.Http;

namespace CRM.Medical.Application.Features.Appointments.CQRS;

public sealed record UpdateAppointmentCommand(
    int Id,
    int AvailabilityId,
    int TestRequestId,
    string? UserId,
    string PatientLocationType,
    double? PatientLatitude,
    double? PatientLongitude,
    string? Notes,
    IFormFile? Attachment) : IRequest<Unit>;
