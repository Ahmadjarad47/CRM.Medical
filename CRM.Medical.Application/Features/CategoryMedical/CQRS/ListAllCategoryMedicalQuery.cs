using CRM.Medical.Application.Features.CategoryMedical.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.CategoryMedical.CQRS;

public sealed record ListAllCategoryMedicalQuery(bool ActiveOnly = true) : IRequest<IReadOnlyList<CategoryMedicalDto>>;
