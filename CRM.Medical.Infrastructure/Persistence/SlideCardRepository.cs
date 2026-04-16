using CRM.Medical.Application.Features.SlideCards;
using CRM.Medical.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CRM.Medical.Infrastructure.Persistence;

public sealed class SlideCardRepository(MedicalDbContext dbContext)
    : ISlideCardRepository
{
    public async Task AddAsync(
        SlideCard entity,
        CancellationToken cancellationToken = default)
    {
        dbContext.SlideCards.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<SlideCard?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default) =>
        dbContext.SlideCards
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

    public async Task<IReadOnlyList<SlideCard>> ListActiveAsync(
        CancellationToken cancellationToken = default)
    {
        return await dbContext.SlideCards
            .AsNoTracking()
            .Where(s => s.IsActive)
            .OrderBy(s => s.DisplayOrder)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<SlideCard>> ListForWebsiteAsync(
        DateTime nowUtc,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.SlideCards
            .AsNoTracking()
            .Where(s => s.IsActive && s.ExpiryDate >= nowUtc)
            .OrderBy(s => s.DisplayOrder)
            .ThenByDescending(s => s.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}

