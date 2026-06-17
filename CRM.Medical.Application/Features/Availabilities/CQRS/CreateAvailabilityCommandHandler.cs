using CRM.Medical.Application.Features.Availabilities.DTOs;
using CRM.Medical.Application.Features.Availabilities.Services;
using MediatR;

namespace CRM.Medical.Application.Features.Availabilities.CQRS;

public sealed class CreateAvailabilityCommandHandler(IAvailabilityService availabilities)
    : IRequestHandler<CreateAvailabilityCommand, AvailabilityDto>
{
    public Task<AvailabilityDto> Handle(
        CreateAvailabilityCommand request,
        CancellationToken cancellationToken) =>
        availabilities.CreateAsync(
            request.UserId,
            request.DayOfWeek,
            request.StartTime,
            request.EndTime,
            request.SlotDuration,
            request.IsActive,
            cancellationToken);
}
