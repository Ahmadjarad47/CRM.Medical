using CRM.Medical.Application.Exceptions;
using CRM.Medical.Application.Features.SubscriptionPackages;
using CRM.Medical.Application.Features.SubscriptionPackages.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.SubscriptionPackages.Queries.GetSubscriptionPackageById;

public sealed class GetSubscriptionPackageByIdQueryHandler(ISubscriptionPackageRepository repository)
    : IRequestHandler<GetSubscriptionPackageByIdQuery, SubscriptionPackageDto>
{
    public async Task<SubscriptionPackageDto> Handle(
        GetSubscriptionPackageByIdQuery request,
        CancellationToken cancellationToken)
    {
        var entity = await repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Subscription package '{request.Id}' was not found.");

        return entity.ToDto();
    }
}
