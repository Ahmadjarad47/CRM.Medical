using CRM.Medical.Application.Features.Availabilities.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.Availabilities.CQRS;

public sealed record CreateAvailabilityCommand(
    string? UserId,
    int DayOfWeek,
    TimeSpan StartTime,
    TimeSpan EndTime,
    int SlotDuration,
    bool IsActive) : IRequest<AvailabilityDto>;
