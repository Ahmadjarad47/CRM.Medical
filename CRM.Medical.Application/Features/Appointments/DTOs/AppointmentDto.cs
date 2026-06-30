namespace CRM.Medical.Application.Features.Appointments.DTOs;

public sealed record AppointmentDto(
    int Id,
    int? AvailabilityId,
    int? TestRequestId,
    string UserId,
    DateTime StartTime,
    DateTime EndTime,
    string Status,
    string PatientLocationType,
    int? Age,
    string? Gender,
    double? PatientLatitude,
    double? PatientLongitude,
    string? Notes,
    string? CreatedByUserId,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
