using CRM.Medical.Domain.Entities;

namespace CRM.Medical.Application.Features.SubscriptionPackages;

public interface ISubscriptionPackageRepository
{
    Task<SubscriptionPackage> AddAsync(SubscriptionPackage entity, CancellationToken cancellationToken = default);

    Task<SubscriptionPackage?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<SubscriptionPackage> Items, int TotalCount)> ListAsync(
        SubscriptionPackageTargetAudience? targetAudience,
        bool? isActive,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(SubscriptionPackage entity, CancellationToken cancellationToken = default);
}
