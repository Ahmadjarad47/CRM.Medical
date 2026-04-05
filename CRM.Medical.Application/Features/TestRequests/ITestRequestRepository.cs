using CRM.Medical.Domain.Entities;

namespace CRM.Medical.Application.Features.TestRequests;

public interface ITestRequestRepository
{
    Task<TestRequest> AddAsync(TestRequest entity, CancellationToken cancellationToken = default);

    Task<TestRequest?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<TestRequest> Items, int TotalCount)> ListAsync(
        int? medicalTestId,
        string? status,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(TestRequest entity, CancellationToken cancellationToken = default);

    Task DeleteAsync(TestRequest entity, CancellationToken cancellationToken = default);

    Task<bool> ExistsForMedicalTestAsync(int medicalTestId, CancellationToken cancellationToken = default);

    Task<bool> HasResultAsync(int testRequestId, CancellationToken cancellationToken = default);
}
