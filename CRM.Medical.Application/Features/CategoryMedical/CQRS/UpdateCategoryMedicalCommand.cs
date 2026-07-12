using MediatR;
using Microsoft.AspNetCore.Http;

namespace CRM.Medical.Application.Features.CategoryMedical.CQRS;

public sealed record UpdateCategoryMedicalCommand(
    int Id,
    string NameAr,
    string NameEn,
    string? Description,
    IFormFile? Image,
    int DisplayOrder,
    bool IsActive) : IRequest<Unit>;
