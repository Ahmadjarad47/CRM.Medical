using CRM.Medical.Domain.Entities;

namespace CRM.Medical.Application.Features.TestRequests;

public interface ITestResultRepository
{
    Task<TestResult> AddAsync(TestResult entity, CancellationToken cancellationToken = default);

    Task<TestResult?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<TestResult> Items, int TotalCount)> ListAsync(
        int? testRequestId,
        string? status,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(TestResult entity, CancellationToken cancellationToken = default);

    Task DeleteAsync(TestResult entity, CancellationToken cancellationToken = default);

    Task<bool> ExistsForTestRequestAsync(int testRequestId, CancellationToken cancellationToken = default);
}
