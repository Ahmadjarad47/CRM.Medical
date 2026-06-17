using CRM.Medical.Application.Features.Store.DTOs;
using CRM.Medical.Application.Features.Store.Services;
using MediatR;

namespace CRM.Medical.Application.Features.Store.Commands.UpdateStoreCategory;

public sealed record UpdateStoreCategoryCommand(int Id, string NameAr, string NameEn, string? Description, string? ImageUrl, int? ParentCategoryId, int DisplayOrder, bool IsActive) : IRequest<ProductCategoryDto>;
