namespace CRM.Medical.Application.Features.Appointments.DTOs;

public sealed record AppointmentAvailabilitySlotDto(
    int AvailabilityId,
    DateTime StartTime,
    DateTime EndTime,
    int DurationMinutes,
    bool IsAvailable,
    int? AppointmentId,
    int? TestRequestId);
