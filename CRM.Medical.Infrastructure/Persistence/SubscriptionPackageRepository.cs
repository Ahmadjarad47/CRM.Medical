using CRM.Medical.Application.Features.SubscriptionPackages;
using CRM.Medical.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CRM.Medical.Infrastructure.Persistence;

public sealed class SubscriptionPackageRepository(MedicalDbContext dbContext) : ISubscriptionPackageRepository
{
    public async Task<SubscriptionPackage> AddAsync(SubscriptionPackage entity, CancellationToken cancellationToken = default)
    {
        dbContext.SubscriptionPackages.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public Task<SubscriptionPackage?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        dbContext.SubscriptionPackages
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

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
    }
}
