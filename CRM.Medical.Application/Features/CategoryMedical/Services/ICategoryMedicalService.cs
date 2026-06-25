using CRM.Medical.Application.Common.Responses;
using CRM.Medical.Application.Features.CategoryMedical.DTOs;

namespace CRM.Medical.Application.Features.CategoryMedical.Services;

public interface ICategoryMedicalService
{
    Task<PagedResult<CategoryMedicalDto>> ListAsync(
        int page,
        int pageSize,
        string? search,
        bool? activeOnly,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<CategoryMedicalDto>> ListAllAsync(
        bool activeOnly,
        CancellationToken cancellationToken);

    Task<CategoryMedicalDto> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<CategoryMedicalDto> CreateAsync(
        string nameAr,
        string nameEn,
        string? description,
        int displayOrder,
        bool isActive,
        CancellationToken cancellationToken);

    Task UpdateAsync(
        int id,
        string nameAr,
        string nameEn,
        string? description,
        int displayOrder,
        bool isActive,
        CancellationToken cancellationToken);

    Task DeleteAsync(int id, CancellationToken cancellationToken);
}
