using CRM.Medical.Application.Features.Store.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.Store.Commands.SaveStoreProduct;

public sealed record SaveStoreProductCommand(
    int? Id,
    int CategoryId,
    string NameAr,
    string NameEn,
    string? Description,
    string ImageUrl,
    string SaleUnit,
    decimal Price,
    decimal? DiscountPrice,
    string? TopBadge,
    int DisplayOrder,
    bool IsRecommended,
    bool IsBestSeller,
    bool IsActive) : IRequest<ProductDetailsDto>;
