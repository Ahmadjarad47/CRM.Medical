using CRM.Medical.Domain.Entities;

namespace CRM.Medical.API.Contracts.Admin.SubscriptionPackages;

public sealed record ListSubscriptionPackagesRequest
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public SubscriptionPackageTargetAudience? TargetAudience { get; init; }
    public bool? IsActive { get; init; }
}
