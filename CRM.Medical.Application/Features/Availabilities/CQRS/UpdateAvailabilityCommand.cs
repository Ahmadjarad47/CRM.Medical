using MediatR;

namespace CRM.Medical.Application.Features.Availabilities.CQRS;

public sealed record UpdateAvailabilityCommand(
    int Id,
    string? UserId,
    int DayOfWeek,
    TimeSpan StartTime,
    TimeSpan EndTime,
    int SlotDuration,
    bool IsActive) : IRequest<Unit>;
