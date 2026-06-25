using CRM.Medical.Application.Features.CategoryMedical.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.CategoryMedical.CQRS;

public sealed record CreateCategoryMedicalCommand(
    string NameAr,
    string NameEn,
    string? Description,
    int DisplayOrder,
    bool IsActive) : IRequest<CategoryMedicalDto>;
