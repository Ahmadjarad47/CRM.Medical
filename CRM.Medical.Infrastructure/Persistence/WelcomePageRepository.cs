using CRM.Medical.Application.Features.WelcomePages;
using CRM.Medical.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CRM.Medical.Infrastructure.Persistence;

public sealed class WelcomePageRepository(MedicalDbContext dbContext) : IWelcomePageRepository
{
    public async Task AddAsync(WelcomePage entity, CancellationToken cancellationToken = default)

    {
        dbContext.WelcomePages.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(WelcomePage entity, CancellationToken cancellationToken = default)
    {
        dbContext.WelcomePages.Update(entity);

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(WelcomePage entity, CancellationToken cancellationToken = default)
    {
        dbContext.WelcomePages.Remove(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<WelcomePage?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        dbContext.WelcomePages
            .AsNoTracking()
            .FirstOrDefaultAsync(w => w.Id == id, cancellationToken);

    public async Task<IReadOnlyList<WelcomePage>> ListAsync(CancellationToken cancellationToken = default) =>
        await dbContext.WelcomePages
            .AsNoTracking()
            .OrderByDescending(w => w.CreatedAt)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<WelcomePage>> ListActiveAsync(CancellationToken cancellationToken = default) =>
        await dbContext.WelcomePages
            .AsNoTracking()
            .Where(w => w.IsActive)
            .OrderByDescending(w => w.CreatedAt)
            .ToListAsync(cancellationToken);
}
