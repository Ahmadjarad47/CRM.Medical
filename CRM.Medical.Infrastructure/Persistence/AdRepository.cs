using CRM.Medical.Application.Features.Ads;
using CRM.Medical.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CRM.Medical.Infrastructure.Persistence;

public sealed class AdRepository(MedicalDbContext dbContext) : IAdRepository
{
    public async Task AddAsync(Ad entity, CancellationToken cancellationToken = default)
    {
        dbContext.Ads.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Ad entity, CancellationToken cancellationToken = default)
    {
        dbContext.Ads.Update(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Ad entity, CancellationToken cancellationToken = default)
    {
        dbContext.Ads.Remove(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<Ad?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        dbContext.Ads
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Ad>> ListAsync(CancellationToken cancellationToken = default) =>
        await dbContext.Ads
            .AsNoTracking()
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync(cancellationToken);
}
