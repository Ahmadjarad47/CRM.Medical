using CRM.Medical.Application.Common.Responses;
using System.Text.Json;
using CRM.Medical.Application.Features.MedicalTests.DTOs;

namespace CRM.Medical.Application.Features.MedicalTests.Services;

public interface IMedicalTestService
{
    Task<PagedResult<MedicalTestDto>> ListAsync(
        int page,
        int pageSize,
        string? search,
        CancellationToken cancellationToken);

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
