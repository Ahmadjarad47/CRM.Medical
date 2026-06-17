using CRM.Medical.Application.Features.Store.DTOs;
using CRM.Medical.Application.Features.Store.Services;
using MediatR;

namespace CRM.Medical.Application.Features.Store.Commands.CreateStoreCategory;

public sealed record CreateStoreCategoryCommand(string NameAr, string NameEn, string? Description, string? ImageUrl, int? ParentCategoryId, int DisplayOrder, bool IsActive) : IRequest<ProductCategoryDto>;
