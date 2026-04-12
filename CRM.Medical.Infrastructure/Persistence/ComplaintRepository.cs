using CRM.Medical.Application.Common.Caching;
using CRM.Medical.Application.Features.Complaints;
using CRM.Medical.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CRM.Medical.Infrastructure.Persistence;

public sealed class ComplaintRepository(MedicalDbContext dbContext, ICacheService cache)
    : IComplaintRepository
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(10);

    private static string CacheKey(int id) => $"complaint:{id}";

    public async Task<Complaint> AddAsync(Complaint entity, CancellationToken cancellationToken = default)
    {
        dbContext.Complaints.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
        await cache.SetAsync(CacheKey(entity.Id), entity, CacheDuration, cancellationToken);
        return entity;
    }

    public async Task<Complaint?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var key = CacheKey(id);
        var cached = await cache.GetAsync<Complaint>(key, cancellationToken);
        if (cached is not null)
            return cached;

        var entity = await dbContext.Complaints
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        if (entity is not null)
            await cache.SetAsync(key, entity, CacheDuration, cancellationToken);

        return entity;
    }

    public async Task<(IReadOnlyList<Complaint> Items, int TotalCount)> ListAsync(
        string? userId,
        string? status,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = dbContext.Complaints.AsNoTracking();

        if (!string.IsNullOrEmpty(userId))
            query = query.Where(c => c.UserId == userId);

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(c => c.Status == status);

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    public async Task UpdateAsync(Complaint entity, CancellationToken cancellationToken = default)
    {
        dbContext.Complaints.Update(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
        await cache.RemoveAsync(CacheKey(entity.Id), cancellationToken);
    }
}
