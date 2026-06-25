using CRM.Medical.Application.Common.Responses;
using CRM.Medical.Application.Features.CategoryMedical.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.CategoryMedical.CQRS;

public sealed record ListCategoryMedicalQuery(
    int Page = 1,
    int PageSize = 20,
    string? Search = null,
    bool? ActiveOnly = null) : IRequest<PagedResult<CategoryMedicalDto>>;
