using CRM.Medical.Application.Common.Json;
using CRM.Medical.Application.Common.Time;
using CRM.Medical.Application.Features.SubscriptionPackages;
using CRM.Medical.Application.Features.SubscriptionPackages.DTOs;
using CRM.Medical.Domain.Entities;
using MediatR;

namespace CRM.Medical.Application.Features.SubscriptionPackages.Commands.CreateSubscriptionPackage;

public sealed class CreateSubscriptionPackageCommandHandler(
    ISubscriptionPackageRepository repository,
    IDateTimeProvider dateTimeProvider)
    : IRequestHandler<CreateSubscriptionPackageCommand, SubscriptionPackageDto>
{
    public async Task<SubscriptionPackageDto> Handle(
        CreateSubscriptionPackageCommand request,
        CancellationToken cancellationToken)
    {
        var now = dateTimeProvider.UtcNow;
        var entity = new SubscriptionPackage
        {
            Name = request.Name,
            Price = request.Price,
            ValidityDays = request.ValidityDays,
            IncludedTests = ProfileMetadataMapper.ToJsonDocument(request.IncludedTests),
            TargetAudience = request.TargetAudience,
            IsActive = request.IsActive,
            CreatedAt = now
        };

        await repository.AddAsync(entity, cancellationToken);
        return entity.ToDto();
    }
}
