using System.Text.Json;
using CRM.Medical.Application.Features.MedicalTests.DTOs;

namespace CRM.Medical.Application.Features.MedicalTests.Services;

public interface IMedicalTestService
{
    Task<IReadOnlyList<MedicalTestDto>> ListAsync(CancellationToken cancellationToken);

    Task<MedicalTestDto> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<MedicalTestDto> CreateAsync(
        string nameAr,
        string nameEn,
        double price,
        string category,
        string sampleType,
        JsonDocument? parameterSchema,
        string status,
        CancellationToken cancellationToken);

    Task UpdateAsync(
        int id,
        string nameAr,
        string nameEn,
        double price,
        string category,
        string sampleType,
        JsonDocument? parameterSchema,
        string status,
        CancellationToken cancellationToken);

    Task DeleteAsync(int id, CancellationToken cancellationToken);
}
