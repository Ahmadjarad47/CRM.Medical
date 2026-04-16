using CRM.Medical.Application.Features.Templates;
using CRM.Medical.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CRM.Medical.Infrastructure.Persistence;

public sealed class TemplateRepository(MedicalDbContext dbContext) : ITemplateRepository
{
    public async Task<Template> AddAsync(Template entity, CancellationToken cancellationToken = default)
    {
        dbContext.Templates.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public Task<Template?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        dbContext.Templates
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Template>> ListByRoleAsync(
        string role,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Templates
            .AsNoTracking()
            .Where(t => t.Role == role)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}

