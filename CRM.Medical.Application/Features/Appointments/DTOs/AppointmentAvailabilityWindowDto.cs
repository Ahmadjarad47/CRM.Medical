namespace CRM.Medical.Application.Features.Appointments.DTOs;

public sealed record AppointmentAvailabilityWindowDto(
    int AvailabilityId,
    DateTime StartTime,
    DateTime EndTime,
    int SlotDurationMinutes);
