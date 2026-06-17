namespace CRM.Medical.Application.Features.Appointments.DTOs;

public sealed record AppointmentDayAvailabilityDto(
    string UserId,
    DateTime Date,
    IReadOnlyList<AppointmentAvailabilityWindowDto> Windows,
    IReadOnlyList<AppointmentAvailabilitySlotDto> Slots,
    int TotalSlots,
    int AvailableSlots,
    int BookedSlots);
