using CRM.Medical.Application.Features.CategoryMedical.DTOs;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace CRM.Medical.Application.Features.CategoryMedical.CQRS;

public sealed record CreateCategoryMedicalCommand(
    string NameAr,
    string NameEn,
    string? Description,
    IFormFile? Image,
    int DisplayOrder,
    bool IsActive) : IRequest<CategoryMedicalDto>;
