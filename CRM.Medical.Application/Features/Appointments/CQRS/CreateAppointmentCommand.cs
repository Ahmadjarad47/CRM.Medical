using CRM.Medical.Application.Features.Appointments.DTOs;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace CRM.Medical.Application.Features.Appointments.CQRS;

public sealed record CreateAppointmentCommand(
    int AvailabilityId,
    int? TestRequestId,
    DateTime StartTime,
    DateTime EndTime,
    string PatientLocationType,
    double? PatientLatitude,
    double? PatientLongitude,
    string? Notes,
    IFormFile? Attachment) : IRequest<AppointmentDto>;
