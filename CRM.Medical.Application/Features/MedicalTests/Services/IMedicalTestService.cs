using CRM.Medical.Application.Common.Responses;
using System.Text.Json;
using CRM.Medical.Application.Features.MedicalTests.DTOs;
using CRM.Medical.Domain.Enums;

namespace CRM.Medical.Application.Features.MedicalTests.Services;

public interface IMedicalTestService
{
    Task<PagedResult<MedicalTestDto>> ListAsync(
        int page,
        int pageSize,
        string? search,
        int? categoryMedicalId,
        CancellationToken cancellationToken);

    Task<MedicalTestDto> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<MedicalTestDto> CreateAsync(
        string nameAr,
        string nameEn,
        double price,
        int categoryMedicalId,
        string sampleType,
        JsonDocument? parameterSchema,
        MedicalTestStatus status,
        CancellationToken cancellationToken);

    Task UpdateAsync(
        int id,
        string nameAr,
        string nameEn,
        double price,
        int categoryMedicalId,
        string sampleType,
        JsonDocument? parameterSchema,
        MedicalTestStatus status,
        CancellationToken cancellationToken);

    Task ToggleStatusAsync(int id, MedicalTestStatus status, CancellationToken cancellationToken);

    Task DeleteAsync(int id, CancellationToken cancellationToken);
}
