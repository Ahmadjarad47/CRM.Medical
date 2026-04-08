using CRM.Medical.Application.Features.TestRequests;
using CRM.Medical.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CRM.Medical.Infrastructure.Persistence;

public sealed class TestRequestRepository(MedicalDbContext dbContext) : ITestRequestRepository
{
    public async Task<TestRequest> AddAsync(TestRequest entity, CancellationToken cancellationToken = default)
    {
        dbContext.TestRequests.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public Task<TestRequest?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        dbContext.TestRequests
            .Include(r => r.MedicalTest)
            .Include(r => r.Result)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

    public async Task<(IReadOnlyList<TestRequest> Items, int TotalCount)> ListAsync(
        int? medicalTestId,
        string? status,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        IQueryable<TestRequest> query = dbContext.TestRequests.AsNoTracking()
            .Include(r => r.MedicalTest)
            .Include(r => r.Result);

        if (medicalTestId is not null)
            query = query.Where(r => r.MedicalTestId == medicalTestId);

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(r => r.Status == status);

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(r => r.RequestDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    public async Task UpdateAsync(TestRequest entity, CancellationToken cancellationToken = default)
    {
        dbContext.TestRequests.Update(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(TestRequest entity, CancellationToken cancellationToken = default)
    {
        dbContext.TestRequests.Remove(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<bool> ExistsForMedicalTestAsync(int medicalTestId, CancellationToken cancellationToken = default) =>
        dbContext.TestRequests.AsNoTracking().AnyAsync(r => r.MedicalTestId == medicalTestId, cancellationToken);

    public Task<bool> HasResultAsync(int testRequestId, CancellationToken cancellationToken = default) =>
        dbContext.TestResults.AsNoTracking().AnyAsync(r => r.TestRequestId == testRequestId, cancellationToken);
}
