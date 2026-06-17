namespace CRM.Medical.Application.Features.Appointments.DTOs;

public sealed record AppointmentAvailabilityWindowDto(
    DateTime StartTime,
    DateTime EndTime,
    int SlotDurationMinutes);
