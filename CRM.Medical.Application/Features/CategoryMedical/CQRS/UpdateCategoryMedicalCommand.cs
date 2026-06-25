using MediatR;

namespace CRM.Medical.Application.Features.CategoryMedical.CQRS;

public sealed record UpdateCategoryMedicalCommand(
    int Id,
    string NameAr,
    string NameEn,
    string? Description,
    int DisplayOrder,
    bool IsActive) : IRequest<Unit>;
