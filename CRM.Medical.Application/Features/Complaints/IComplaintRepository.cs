using CRM.Medical.Domain.Entities;

namespace CRM.Medical.Application.Features.Complaints;

public interface IComplaintRepository
{
    Task<Complaint> AddAsync(Complaint entity, CancellationToken cancellationToken = default);

    Task<Complaint?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<Complaint> Items, int TotalCount)> ListAsync(
        string? userId,
        string? status,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(Complaint entity, CancellationToken cancellationToken = default);
}
