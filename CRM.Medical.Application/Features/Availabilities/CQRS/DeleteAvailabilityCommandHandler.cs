using CRM.Medical.Application.Features.Availabilities.Services;
using MediatR;

namespace CRM.Medical.Application.Features.Availabilities.CQRS;

public sealed class DeleteAvailabilityCommandHandler(IAvailabilityService availabilities)
    : IRequestHandler<DeleteAvailabilityCommand, Unit>
{
    public async Task<Unit> Handle(DeleteAvailabilityCommand request, CancellationToken cancellationToken)
    {
        await availabilities.DeleteAsync(request.Id, cancellationToken);
        return Unit.Value;
    }
}
