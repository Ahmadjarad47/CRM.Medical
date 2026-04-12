using CRM.Medical.Application.Common.Caching;
using CRM.Medical.Application.Features.SubscriptionPackages;
using CRM.Medical.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CRM.Medical.Infrastructure.Persistence;

public sealed class SubscriptionPackageRepository(MedicalDbContext dbContext, ICacheService cache)
    : ISubscriptionPackageRepository
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(10);

    private static string CacheKey(int id) => $"subscription_package:{id}";

    public async Task<SubscriptionPackage> AddAsync(SubscriptionPackage entity, CancellationToken cancellationToken = default)
    {
        dbContext.SubscriptionPackages.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
        await cache.SetAsync(CacheKey(entity.Id), entity, CacheDuration, cancellationToken);
        return entity;
    }

    public async Task<SubscriptionPackage?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var key = CacheKey(id);
        var cached = await cache.GetAsync<SubscriptionPackage>(key, cancellationToken);
        if (cached is not null)
            return cached;

        var entity = await dbContext.SubscriptionPackages
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        if (entity is not null)
            await cache.SetAsync(key, entity, CacheDuration, cancellationToken);

        return entity;
    }

    public async Task<(IReadOnlyList<SubscriptionPackage> Items, int TotalCount)> ListAsync(
        SubscriptionPackageTargetAudience? targetAudience,
        bool? isActive,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = dbContext.SubscriptionPackages.AsNoTracking();

        if (targetAudience is not null)
            query = query.Where(p => p.TargetAudience == targetAudience);

        if (isActive is not null)
            query = query.Where(p => p.IsActive == isActive);

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    public async Task UpdateAsync(SubscriptionPackage entity, CancellationToken cancellationToken = default)
    {
        dbContext.SubscriptionPackages.Update(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
        await cache.RemoveAsync(CacheKey(entity.Id), cancellationToken);
    }
}
