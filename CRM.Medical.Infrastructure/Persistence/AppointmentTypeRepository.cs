using CRM.Medical.Application.Common.Caching;
using CRM.Medical.Application.Features.AppointmentTypes;
using CRM.Medical.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CRM.Medical.Infrastructure.Persistence;

public sealed class AppointmentTypeRepository(MedicalDbContext dbContext, ICacheService cache)
    : IAppointmentTypeRepository
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(10);

    private static string CacheKey(int id) => $"appointment_type:{id}";

    public async Task AddAsync(AppointmentType entity, CancellationToken cancellationToken = default)
    {
        dbContext.AppointmentTypes.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
        await cache.SetAsync(CacheKey(entity.Id), entity, CacheDuration, cancellationToken);
    }

    public async Task<AppointmentType?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var key = CacheKey(id);
        var cached = await cache.GetAsync<AppointmentType>(key, cancellationToken);
        if (cached is not null)
            return cached;

        var entity = await dbContext.AppointmentTypes.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        if (entity is not null)
            await cache.SetAsync(key, entity, CacheDuration, cancellationToken);

        return entity;
    }

    public async Task<IReadOnlyList<AppointmentType>> ListActiveAsync(CancellationToken cancellationToken = default)
    {
        var items = await dbContext.AppointmentTypes
            .AsNoTracking()
            .Where(t => t.IsActive)
            .OrderBy(t => t.Name)
            .ToListAsync(cancellationToken);
        return items;
    }

    public async Task<IReadOnlyList<AppointmentType>> ListAllAsync(CancellationToken cancellationToken = default)
    {
        var items = await dbContext.AppointmentTypes
            .AsNoTracking()
            .OrderBy(t => t.Name)
            .ToListAsync(cancellationToken);
        return items;
    }

    public Task<bool> NameExistsAsync(string name, int? excludeId, CancellationToken cancellationToken = default)
    {
        var n = name.Trim();
        var query = dbContext.AppointmentTypes.AsNoTracking().Where(t => t.Name == n);
        if (excludeId is { } id)
            query = query.Where(t => t.Id != id);
        return query.AnyAsync(cancellationToken);
    }

    public async Task UpdateAsync(AppointmentType entity, CancellationToken cancellationToken = default)
    {
        dbContext.AppointmentTypes.Update(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
        await cache.RemoveAsync(CacheKey(entity.Id), cancellationToken);
    }
}
