using CRM.Medical.Application.Features.Store.DTOs;
using CRM.Medical.Application.Features.Store.Services;
using MediatR;

namespace CRM.Medical.Application.Features.Store.Commands.SaveStoreProduct;

public sealed class SaveStoreProductCommandHandler(IStoreAdminService service)
    : IRequestHandler<SaveStoreProductCommand, ProductDetailsDto>
{
    public Task<ProductDetailsDto> Handle(SaveStoreProductCommand request, CancellationToken cancellationToken) =>
        request.Id is null
            ? service.CreateProductAsync(
                request.CategoryId,
                request.NameAr,
                request.NameEn,
                request.Description,
                request.ImageUrl,
                request.SaleUnit,
                request.Price,
                request.DiscountPrice,
                request.TopBadge,
                request.DisplayOrder,
                request.IsRecommended,
                request.IsBestSeller,
                request.IsActive,
                cancellationToken)
            : service.UpdateProductAsync(
                request.Id.Value,
                request.CategoryId,
                request.NameAr,
                request.NameEn,
                request.Description,
                request.ImageUrl,
                request.SaleUnit,
                request.Price,
                request.DiscountPrice,
                request.TopBadge,
                request.DisplayOrder,
                request.IsRecommended,
                request.IsBestSeller,
                request.IsActive,
                cancellationToken);
}
