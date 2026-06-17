using CRM.Medical.Application.Features.Availabilities.DTOs;
using CRM.Medical.Application.Features.Availabilities.Services;
using MediatR;

namespace CRM.Medical.Application.Features.Availabilities.CQRS;

public sealed class ListAvailabilitiesQueryHandler(IAvailabilityService availabilities)
    : IRequestHandler<ListAvailabilitiesQuery, IReadOnlyList<AvailabilityDto>>
{
    public Task<IReadOnlyList<AvailabilityDto>> Handle(
        ListAvailabilitiesQuery request,
        CancellationToken cancellationToken) =>
        availabilities.ListAsync(request.UserId, cancellationToken);
}
