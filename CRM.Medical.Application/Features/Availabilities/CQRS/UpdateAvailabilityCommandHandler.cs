using CRM.Medical.Application.Features.Availabilities.Services;
using MediatR;

namespace CRM.Medical.Application.Features.Availabilities.CQRS;

public sealed class UpdateAvailabilityCommandHandler(IAvailabilityService availabilities)
    : IRequestHandler<UpdateAvailabilityCommand, Unit>
{
    public async Task<Unit> Handle(
        UpdateAvailabilityCommand request,
        CancellationToken cancellationToken)
    {
        await availabilities.UpdateAsync(
            request.Id,
            request.DayOfWeek,
            request.StartTime,
            request.EndTime,
            request.SlotDuration,
            request.IsActive,
            cancellationToken);

        return Unit.Value;
    }
}
