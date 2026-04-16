using CRM.Medical.Application.Features.Banners;
using CRM.Medical.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CRM.Medical.Infrastructure.Persistence;

public sealed class BannerRepository(MedicalDbContext dbContext)
    : IBannerRepository
{
    public async Task AddAsync(
        Banner entity,
        CancellationToken cancellationToken = default)
    {
        dbContext.Banners.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<Banner?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default) =>
        dbContext.Banners
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Banner>> ListActiveAsync(
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Banners
            .AsNoTracking()
            .Where(b => b.IsActive)
            .OrderBy(b => b.DisplayOrder)
            .ThenByDescending(b => b.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Banner>> ListForWebsiteAsync(
        DateTime nowUtc,
        string? location,
        CancellationToken cancellationToken = default)
    {
        var query = dbContext.Banners
            .AsNoTracking()
            .Where(b =>
                b.IsActive &&
                b.StartDate <= nowUtc &&
                b.EndDate >= nowUtc);

        if (!string.IsNullOrWhiteSpace(location))
            query = query.Where(b => b.Location == location);

        return await query
            .OrderBy(b => b.DisplayOrder)
            .ThenByDescending(b => b.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}

