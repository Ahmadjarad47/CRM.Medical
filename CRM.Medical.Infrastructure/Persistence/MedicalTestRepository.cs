using CRM.Medical.Application.Common.Caching;
using CRM.Medical.Application.Features.MedicalTests;
using CRM.Medical.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CRM.Medical.Infrastructure.Persistence;

public sealed class MedicalTestRepository(MedicalDbContext dbContext, ICacheService cache)
    : IMedicalTestRepository
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(10);

    private static string CacheKey(int id) => $"medical_test:{id}";

    public async Task<MedicalTest> AddAsync(MedicalTest entity, CancellationToken cancellationToken = default)
    {
        dbContext.MedicalTests.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
        await cache.SetAsync(CacheKey(entity.Id), entity, CacheDuration, cancellationToken);
        return entity;
    }

    public async Task<MedicalTest?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var key = CacheKey(id);
        var cached = await cache.GetAsync<MedicalTest>(key, cancellationToken);
        if (cached is not null)
            return cached;

        var entity = await dbContext.MedicalTests.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        if (entity is not null)
            await cache.SetAsync(key, entity, CacheDuration, cancellationToken);

        return entity;
    }

    public async Task<(IReadOnlyList<MedicalTest> Items, int TotalCount)> ListAsync(
        string? category,
        string? status,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = dbContext.MedicalTests.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(category))
            query = query.Where(t => t.Category == category);

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(t => t.Status == status);

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(t => t.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    public async Task UpdateAsync(MedicalTest entity, CancellationToken cancellationToken = default)
    {
        dbContext.MedicalTests.Update(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
        await cache.RemoveAsync(CacheKey(entity.Id), cancellationToken);
    }

    public async Task DeleteAsync(MedicalTest entity, CancellationToken cancellationToken = default)
    {
        dbContext.MedicalTests.Remove(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
        await cache.RemoveAsync(CacheKey(entity.Id), cancellationToken);
    }

    public Task<bool> HasTestRequestAsync(int medicalTestId, CancellationToken cancellationToken = default) =>
        dbContext.TestRequests.AsNoTracking().AnyAsync(r => r.MedicalTestId == medicalTestId, cancellationToken);
}
