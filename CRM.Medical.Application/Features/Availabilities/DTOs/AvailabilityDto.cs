namespace CRM.Medical.Application.Features.Availabilities.DTOs;

public sealed record AvailabilityDto(
    int Id,
    string UserId,
    DayOfWeek DayOfWeek,
    TimeSpan StartTime,
    TimeSpan EndTime,
    int SlotDuration,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
