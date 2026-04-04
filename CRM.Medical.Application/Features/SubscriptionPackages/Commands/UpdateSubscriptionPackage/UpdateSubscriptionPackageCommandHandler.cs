using CRM.Medical.Application.Common.Json;
using CRM.Medical.Application.Common.Time;
using CRM.Medical.Application.Exceptions;
using CRM.Medical.Application.Features.SubscriptionPackages;
using CRM.Medical.Application.Features.SubscriptionPackages.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.SubscriptionPackages.Commands.UpdateSubscriptionPackage;

public sealed class UpdateSubscriptionPackageCommandHandler(
    ISubscriptionPackageRepository repository,
    IDateTimeProvider dateTimeProvider)
    : IRequestHandler<UpdateSubscriptionPackageCommand, SubscriptionPackageDto>
{
    public async Task<SubscriptionPackageDto> Handle(
        UpdateSubscriptionPackageCommand request,
        CancellationToken cancellationToken)
    {
        var entity = await repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Subscription package '{request.Id}' was not found.");

        entity.IncludedTests?.Dispose();
        entity.IncludedTests = ProfileMetadataMapper.ToJsonDocument(request.IncludedTests);

        entity.Name = request.Name;
        entity.Price = request.Price;
        entity.ValidityDays = request.ValidityDays;
        entity.TargetAudience = request.TargetAudience;
        entity.IsActive = request.IsActive;
        entity.UpdatedAt = dateTimeProvider.UtcNow;

        await repository.UpdateAsync(entity, cancellationToken);
        return entity.ToDto();
    }
}
