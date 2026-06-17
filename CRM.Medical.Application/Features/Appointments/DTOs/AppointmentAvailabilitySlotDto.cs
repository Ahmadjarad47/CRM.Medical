namespace CRM.Medical.Application.Features.Appointments.DTOs;

public sealed record AppointmentAvailabilitySlotDto(
    DateTime StartTime,
    DateTime EndTime,
    int DurationMinutes,
    bool IsAvailable,
    int? AppointmentId,
    int? TestRequestId);
