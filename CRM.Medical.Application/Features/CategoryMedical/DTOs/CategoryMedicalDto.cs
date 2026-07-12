namespace CRM.Medical.Application.Features.CategoryMedical.DTOs;

public sealed record CategoryMedicalDto(
    int Id,
    string NameAr,
    string NameEn,
    string? Description,
    string? ImageUrl,
    int DisplayOrder,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
