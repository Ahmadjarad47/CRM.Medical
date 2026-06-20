using CRM.Medical.Application.Features.Availabilities.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.Availabilities.CQRS;

public sealed record CreateAvailabilityCommand(
    DayOfWeek DayOfWeek,
    TimeSpan StartTime,
    TimeSpan EndTime,
    int SlotDuration,
    bool IsActive) : IRequest<AvailabilityDto>;
