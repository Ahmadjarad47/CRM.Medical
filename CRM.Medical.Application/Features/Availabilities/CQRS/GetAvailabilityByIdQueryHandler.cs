using CRM.Medical.Application.Features.Availabilities.DTOs;
using CRM.Medical.Application.Features.Availabilities.Services;
using MediatR;

namespace CRM.Medical.Application.Features.Availabilities.CQRS;

public sealed class GetAvailabilityByIdQueryHandler(IAvailabilityService availabilities)
    : IRequestHandler<GetAvailabilityByIdQuery, AvailabilityDto>
{
    public Task<AvailabilityDto> Handle(GetAvailabilityByIdQuery request, CancellationToken cancellationToken) =>
        availabilities.GetByIdAsync(request.Id, cancellationToken);
}
