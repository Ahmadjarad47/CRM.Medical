using CRM.Medical.Application.Common.Json;
using CRM.Medical.Application.Features.SubscriptionPackages.DTOs;
using CRM.Medical.Domain.Entities;

namespace CRM.Medical.Application.Features.SubscriptionPackages;

internal static class SubscriptionPackageMappings
{
    public static SubscriptionPackageDto ToDto(this SubscriptionPackage p) =>
        new(
            p.Id,
            p.Name,
            p.Price,
            p.ValidityDays,
            ProfileMetadataMapper.ToJsonElement(p.IncludedTests),
            p.TargetAudience,
            p.IsActive,
            p.CreatedAt,
            p.UpdatedAt);
}
