using CRM.Medical.Application.Features.CategoryMedical.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.CategoryMedical.CQRS;

public sealed record GetCategoryMedicalByIdQuery(int Id) : IRequest<CategoryMedicalDto>;
