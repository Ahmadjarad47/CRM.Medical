using CRM.Medical.Application.Common.Caching;
using CRM.Medical.Application.Features.TestRequests;
using CRM.Medical.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CRM.Medical.Infrastructure.Persistence;

public sealed class TestResultRepository(MedicalDbContext dbContext, ICacheService cache)
    : ITestResultRepository
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(10);

    private static string CacheKey(int id) => $"test_result:{id}";

    public async Task<TestResult> AddAsync(TestResult entity, CancellationToken cancellationToken = default)
    {
        dbContext.TestResults.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
        await cache.SetAsync(CacheKey(entity.Id), entity, CacheDuration, cancellationToken);
        return entity;
    }

    public async Task<TestResult?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var key = CacheKey(id);
        var cached = await cache.GetAsync<TestResult>(key, cancellationToken);
        if (cached is not null)
            return cached;

        var entity = await dbContext.TestResults
            .Include(r => r.TestRequest)
            .ThenInclude(q => q.MedicalTest)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
        if (entity is not null)
            await cache.SetAsync(key, entity, CacheDuration, cancellationToken);

        return entity;
    }

    public async Task<(IReadOnlyList<TestResult> Items, int TotalCount)> ListAsync(
        int? testRequestId,
        string? status,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        IQueryable<TestResult> query = dbContext.TestResults.AsNoTracking()
            .Include(r => r.TestRequest)
            .ThenInclude(q => q.MedicalTest);

        if (testRequestId is not null)
            query = query.Where(r => r.TestRequestId == testRequestId);

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(r => r.Status == status);

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(r => r.ResultDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    public async Task UpdateAsync(TestResult entity, CancellationToken cancellationToken = default)
    {
        dbContext.TestResults.Update(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
        await cache.RemoveAsync(CacheKey(entity.Id), cancellationToken);
    }

    public async Task DeleteAsync(TestResult entity, CancellationToken cancellationToken = default)
    {
        dbContext.TestResults.Remove(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
        await cache.RemoveAsync(CacheKey(entity.Id), cancellationToken);
    }

    public Task<bool> ExistsForTestRequestAsync(int testRequestId, CancellationToken cancellationToken = default) =>
        dbContext.TestResults.AsNoTracking().AnyAsync(r => r.TestRequestId == testRequestId, cancellationToken);
}
