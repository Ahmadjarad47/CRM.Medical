using CRM.Medical.Domain.Entities;

namespace CRM.Medical.Application.Features.MedicalTests;

public interface IMedicalTestRepository
{
    Task<MedicalTest> AddAsync(MedicalTest entity, CancellationToken cancellationToken = default);

    Task<MedicalTest?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<MedicalTest> Items, int TotalCount)> ListAsync(
        string? category,
        string? status,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(MedicalTest entity, CancellationToken cancellationToken = default);

    Task DeleteAsync(MedicalTest entity, CancellationToken cancellationToken = default);

    Task<bool> HasTestRequestAsync(int medicalTestId, CancellationToken cancellationToken = default);
}
