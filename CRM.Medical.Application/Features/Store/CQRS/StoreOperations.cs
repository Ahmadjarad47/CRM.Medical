using CRM.Medical.Application.Common.Responses;
using CRM.Medical.Application.Features.Store.DTOs;
using CRM.Medical.Application.Features.Store.Services;
using CRM.Medical.Domain.Enums;
using MediatR;

namespace CRM.Medical.Application.Features.Store.CQRS;

public sealed record GetStoreHomeQuery : IRequest<StoreHomeDto>;
public sealed class GetStoreHomeQueryHandler(IStoreHomeService service) : IRequestHandler<GetStoreHomeQuery, StoreHomeDto>
{
    public Task<StoreHomeDto> Handle(GetStoreHomeQuery request, CancellationToken cancellationToken) =>
        service.GetHomeAsync(cancellationToken);
}

public sealed record ListStoreCategoriesQuery(bool ActiveOnly) : IRequest<IReadOnlyList<ProductCategoryDto>>;
public sealed class ListStoreCategoriesQueryHandler(IProductCatalogService service) : IRequestHandler<ListStoreCategoriesQuery, IReadOnlyList<ProductCategoryDto>>
{
    public Task<IReadOnlyList<ProductCategoryDto>> Handle(ListStoreCategoriesQuery request, CancellationToken cancellationToken) =>
        service.ListCategoriesAsync(request.ActiveOnly, cancellationToken);
}

public sealed record GetStoreCategoryQuery(int Id, bool ActiveOnly) : IRequest<ProductCategoryDto>;
public sealed class GetStoreCategoryQueryHandler(IProductCatalogService service) : IRequestHandler<GetStoreCategoryQuery, ProductCategoryDto>
{
    public Task<ProductCategoryDto> Handle(GetStoreCategoryQuery request, CancellationToken cancellationToken) =>
        service.GetCategoryAsync(request.Id, request.ActiveOnly, cancellationToken);
}

public sealed record GetStoreCategoryPageQuery(int Id) : IRequest<CategoryPageDto>;
public sealed class GetStoreCategoryPageQueryHandler(IStoreHomeService service) : IRequestHandler<GetStoreCategoryPageQuery, CategoryPageDto>
{
    public Task<CategoryPageDto> Handle(GetStoreCategoryPageQuery request, CancellationToken cancellationToken) =>
        service.GetCategoryPageAsync(request.Id, cancellationToken);
}

public sealed record ListStoreProductsQuery(int Page, int PageSize, string? Search, int? CategoryId, bool ActiveOnly) : IRequest<PagedResult<ProductCardDto>>;
public sealed class ListStoreProductsQueryHandler(IProductCatalogService service) : IRequestHandler<ListStoreProductsQuery, PagedResult<ProductCardDto>>
{
    public Task<PagedResult<ProductCardDto>> Handle(ListStoreProductsQuery request, CancellationToken cancellationToken) =>
        service.ListProductsAsync(request.Page, request.PageSize, request.Search, request.CategoryId, request.ActiveOnly, cancellationToken);
}

public sealed record GetStoreProductQuery(int Id, bool ActiveOnly) : IRequest<ProductDetailsDto>;
public sealed class GetStoreProductQueryHandler(IProductCatalogService service) : IRequestHandler<GetStoreProductQuery, ProductDetailsDto>
{
    public Task<ProductDetailsDto> Handle(GetStoreProductQuery request, CancellationToken cancellationToken) =>
        service.GetProductAsync(request.Id, request.ActiveOnly, cancellationToken);
}

public sealed record GetStoreCartQuery(string LabClientId) : IRequest<CartDto>;
public sealed class GetStoreCartQueryHandler(ICartService service) : IRequestHandler<GetStoreCartQuery, CartDto>
{
    public Task<CartDto> Handle(GetStoreCartQuery request, CancellationToken cancellationToken) =>
        service.GetAsync(request.LabClientId, cancellationToken);
}

public sealed record AddStoreCartItemCommand(string LabClientId, int ProductId, int Quantity) : IRequest<CartDto>;
public sealed class AddStoreCartItemCommandHandler(ICartService service) : IRequestHandler<AddStoreCartItemCommand, CartDto>
{
    public Task<CartDto> Handle(AddStoreCartItemCommand request, CancellationToken cancellationToken) =>
        service.AddItemAsync(request.LabClientId, request.ProductId, request.Quantity, cancellationToken);
}

public sealed record UpdateStoreCartItemCommand(string LabClientId, int ItemId, int Quantity) : IRequest<CartDto>;
public sealed class UpdateStoreCartItemCommandHandler(ICartService service) : IRequestHandler<UpdateStoreCartItemCommand, CartDto>
{
    public Task<CartDto> Handle(UpdateStoreCartItemCommand request, CancellationToken cancellationToken) =>
        service.UpdateItemAsync(request.LabClientId, request.ItemId, request.Quantity, cancellationToken);
}

public sealed record RemoveStoreCartItemCommand(string LabClientId, int ItemId) : IRequest;
public sealed class RemoveStoreCartItemCommandHandler(ICartService service) : IRequestHandler<RemoveStoreCartItemCommand>
{
    public Task Handle(RemoveStoreCartItemCommand request, CancellationToken cancellationToken) =>
        service.RemoveItemAsync(request.LabClientId, request.ItemId, cancellationToken);
}

public sealed record ApplyStoreCouponCommand(string LabClientId, string Code) : IRequest<CartDto>;
public sealed class ApplyStoreCouponCommandHandler(ICartService service) : IRequestHandler<ApplyStoreCouponCommand, CartDto>
{
    public Task<CartDto> Handle(ApplyStoreCouponCommand request, CancellationToken cancellationToken) =>
        service.ApplyCouponAsync(request.LabClientId, request.Code, cancellationToken);
}

public sealed record RemoveStoreCouponCommand(string LabClientId) : IRequest<CartDto>;
public sealed class RemoveStoreCouponCommandHandler(ICartService service) : IRequestHandler<RemoveStoreCouponCommand, CartDto>
{
    public Task<CartDto> Handle(RemoveStoreCouponCommand request, CancellationToken cancellationToken) =>
        service.RemoveCouponAsync(request.LabClientId, cancellationToken);
}

public sealed record StoreCheckoutCommand(string LabClientId, PaymentMethod PaymentMethod, string? Notes) : IRequest<StoreOrderDetailsDto>;
public sealed class StoreCheckoutCommandHandler(ICheckoutService service) : IRequestHandler<StoreCheckoutCommand, StoreOrderDetailsDto>
{
    public Task<StoreOrderDetailsDto> Handle(StoreCheckoutCommand request, CancellationToken cancellationToken) =>
        service.CheckoutAsync(request.LabClientId, request.PaymentMethod, request.Notes, cancellationToken);
}

public sealed record ListMyStoreOrdersQuery(string LabClientId, int Page, int PageSize) : IRequest<PagedResult<StoreOrderDto>>;
public sealed class ListMyStoreOrdersQueryHandler(IStoreOrderService service) : IRequestHandler<ListMyStoreOrdersQuery, PagedResult<StoreOrderDto>>
{
    public Task<PagedResult<StoreOrderDto>> Handle(ListMyStoreOrdersQuery request, CancellationToken cancellationToken) =>
        service.ListMyOrdersAsync(request.LabClientId, request.Page, request.PageSize, cancellationToken);
}

public sealed record GetMyStoreOrderQuery(string LabClientId, int Id) : IRequest<StoreOrderDetailsDto>;
public sealed class GetMyStoreOrderQueryHandler(IStoreOrderService service) : IRequestHandler<GetMyStoreOrderQuery, StoreOrderDetailsDto>
{
    public Task<StoreOrderDetailsDto> Handle(GetMyStoreOrderQuery request, CancellationToken cancellationToken) =>
        service.GetMyOrderAsync(request.LabClientId, request.Id, cancellationToken);
}

public sealed record GetStoreSettingsQuery : IRequest<StoreSettingDto>;
public sealed class GetStoreSettingsQueryHandler(IStoreAdminService service) : IRequestHandler<GetStoreSettingsQuery, StoreSettingDto>
{
    public Task<StoreSettingDto> Handle(GetStoreSettingsQuery request, CancellationToken cancellationToken) =>
        service.GetSettingsAsync(cancellationToken);
}

public sealed record UpdateStoreSettingsCommand(StoreSettingDto Request) : IRequest<StoreSettingDto>;
public sealed class UpdateStoreSettingsCommandHandler(IStoreAdminService service) : IRequestHandler<UpdateStoreSettingsCommand, StoreSettingDto>
{
    public Task<StoreSettingDto> Handle(UpdateStoreSettingsCommand request, CancellationToken cancellationToken) =>
        service.UpdateSettingsAsync(request.Request, cancellationToken);
}

public sealed record CreateStoreCategoryCommand(string NameAr, string NameEn, string? Description, string? ImageUrl, int? ParentCategoryId, int DisplayOrder, bool IsActive) : IRequest<ProductCategoryDto>;
public sealed class CreateStoreCategoryCommandHandler(IStoreAdminService service) : IRequestHandler<CreateStoreCategoryCommand, ProductCategoryDto>
{
    public Task<ProductCategoryDto> Handle(CreateStoreCategoryCommand r, CancellationToken ct) =>
        service.CreateCategoryAsync(r.NameAr, r.NameEn, r.Description, r.ImageUrl, r.ParentCategoryId, r.DisplayOrder, r.IsActive, ct);
}

public sealed record UpdateStoreCategoryCommand(int Id, string NameAr, string NameEn, string? Description, string? ImageUrl, int? ParentCategoryId, int DisplayOrder, bool IsActive) : IRequest<ProductCategoryDto>;
public sealed class UpdateStoreCategoryCommandHandler(IStoreAdminService service) : IRequestHandler<UpdateStoreCategoryCommand, ProductCategoryDto>
{
    public Task<ProductCategoryDto> Handle(UpdateStoreCategoryCommand r, CancellationToken ct) =>
        service.UpdateCategoryAsync(r.Id, r.NameAr, r.NameEn, r.Description, r.ImageUrl, r.ParentCategoryId, r.DisplayOrder, r.IsActive, ct);
}

public sealed record DeleteStoreCategoryCommand(int Id) : IRequest;
public sealed class DeleteStoreCategoryCommandHandler(IStoreAdminService service) : IRequestHandler<DeleteStoreCategoryCommand>
{
    public Task Handle(DeleteStoreCategoryCommand request, CancellationToken cancellationToken) =>
        service.DeleteCategoryAsync(request.Id, cancellationToken);
}

public sealed record SaveStoreProductCommand(int? Id, int CategoryId, string NameAr, string NameEn, string? Description, string ImageUrl, string SaleUnit, decimal Price, decimal? DiscountPrice, string? TopBadge, int DisplayOrder, bool IsRecommended, bool IsBestSeller, bool IsActive) : IRequest<ProductDetailsDto>;
public sealed class SaveStoreProductCommandHandler(IStoreAdminService service) : IRequestHandler<SaveStoreProductCommand, ProductDetailsDto>
{
    public Task<ProductDetailsDto> Handle(SaveStoreProductCommand r, CancellationToken ct) =>
        r.Id is null
            ? service.CreateProductAsync(r.CategoryId, r.NameAr, r.NameEn, r.Description, r.ImageUrl, r.SaleUnit, r.Price, r.DiscountPrice, r.TopBadge, r.DisplayOrder, r.IsRecommended, r.IsBestSeller, r.IsActive, ct)
            : service.UpdateProductAsync(r.Id.Value, r.CategoryId, r.NameAr, r.NameEn, r.Description, r.ImageUrl, r.SaleUnit, r.Price, r.DiscountPrice, r.TopBadge, r.DisplayOrder, r.IsRecommended, r.IsBestSeller, r.IsActive, ct);
}

public sealed record DeleteStoreProductCommand(int Id) : IRequest;
public sealed class DeleteStoreProductCommandHandler(IStoreAdminService service) : IRequestHandler<DeleteStoreProductCommand>
{
    public Task Handle(DeleteStoreProductCommand request, CancellationToken cancellationToken) =>
        service.DeleteProductAsync(request.Id, cancellationToken);
}

public sealed record ListStoreSlidersQuery : IRequest<IReadOnlyList<StoreSliderDto>>;
public sealed class ListStoreSlidersQueryHandler(IStoreAdminService service) : IRequestHandler<ListStoreSlidersQuery, IReadOnlyList<StoreSliderDto>>
{
    public Task<IReadOnlyList<StoreSliderDto>> Handle(ListStoreSlidersQuery request, CancellationToken cancellationToken) =>
        service.ListSlidersAsync(cancellationToken);
}

public sealed record SaveStoreSliderCommand(int? Id, string Title, StoreSliderType Type, int DisplayOrder, bool IsActive, IReadOnlyList<int> ProductIds) : IRequest<StoreSliderDto>;
public sealed class SaveStoreSliderCommandHandler(IStoreAdminService service) : IRequestHandler<SaveStoreSliderCommand, StoreSliderDto>
{
    public Task<StoreSliderDto> Handle(SaveStoreSliderCommand r, CancellationToken ct) =>
        r.Id is null
            ? service.CreateSliderAsync(r.Title, r.Type, r.DisplayOrder, r.IsActive, r.ProductIds, ct)
            : service.UpdateSliderAsync(r.Id.Value, r.Title, r.Type, r.DisplayOrder, r.IsActive, r.ProductIds, ct);
}

public sealed record DeleteStoreSliderCommand(int Id) : IRequest;
public sealed class DeleteStoreSliderCommandHandler(IStoreAdminService service) : IRequestHandler<DeleteStoreSliderCommand>
{
    public Task Handle(DeleteStoreSliderCommand request, CancellationToken cancellationToken) =>
        service.DeleteSliderAsync(request.Id, cancellationToken);
}

public sealed record ListStoreBannersQuery : IRequest<IReadOnlyList<StoreBannerDto>>;
public sealed class ListStoreBannersQueryHandler(IStoreAdminService service) : IRequestHandler<ListStoreBannersQuery, IReadOnlyList<StoreBannerDto>>
{
    public Task<IReadOnlyList<StoreBannerDto>> Handle(ListStoreBannersQuery request, CancellationToken cancellationToken) =>
        service.ListBannersAsync(cancellationToken);
}

public sealed record SaveStoreBannerCommand(int? Id, string Title, string ImageUrl, string? LinkUrl, string Location, int? CategoryId, int DisplayOrder, bool IsActive, DateTime? StartsAt, DateTime? EndsAt) : IRequest<StoreBannerDto>;
public sealed class SaveStoreBannerCommandHandler(IStoreAdminService service) : IRequestHandler<SaveStoreBannerCommand, StoreBannerDto>
{
    public Task<StoreBannerDto> Handle(SaveStoreBannerCommand r, CancellationToken ct) =>
        r.Id is null
            ? service.CreateBannerAsync(r.Title, r.ImageUrl, r.LinkUrl, r.Location, r.CategoryId, r.DisplayOrder, r.IsActive, r.StartsAt, r.EndsAt, ct)
            : service.UpdateBannerAsync(r.Id.Value, r.Title, r.ImageUrl, r.LinkUrl, r.Location, r.CategoryId, r.DisplayOrder, r.IsActive, r.StartsAt, r.EndsAt, ct);
}

public sealed record DeleteStoreBannerCommand(int Id) : IRequest;
public sealed class DeleteStoreBannerCommandHandler(IStoreAdminService service) : IRequestHandler<DeleteStoreBannerCommand>
{
    public Task Handle(DeleteStoreBannerCommand request, CancellationToken cancellationToken) =>
        service.DeleteBannerAsync(request.Id, cancellationToken);
}

public sealed record ListStoreCouponsQuery : IRequest<IReadOnlyList<CouponDto>>;
public sealed class ListStoreCouponsQueryHandler(IStoreAdminService service) : IRequestHandler<ListStoreCouponsQuery, IReadOnlyList<CouponDto>>
{
    public Task<IReadOnlyList<CouponDto>> Handle(ListStoreCouponsQuery request, CancellationToken cancellationToken) =>
        service.ListCouponsAsync(cancellationToken);
}

public sealed record SaveStoreCouponCommand(int? Id, string Code, DiscountType DiscountType, decimal Amount, decimal? MinimumSubtotal, decimal? MaximumDiscountAmount, DateTime? StartsAt, DateTime? ExpiresAt, bool IsActive) : IRequest<CouponDto>;
public sealed class SaveStoreCouponCommandHandler(IStoreAdminService service) : IRequestHandler<SaveStoreCouponCommand, CouponDto>
{
    public Task<CouponDto> Handle(SaveStoreCouponCommand r, CancellationToken ct) =>
        r.Id is null
            ? service.CreateCouponAsync(r.Code, r.DiscountType, r.Amount, r.MinimumSubtotal, r.MaximumDiscountAmount, r.StartsAt, r.ExpiresAt, r.IsActive, ct)
            : service.UpdateCouponAsync(r.Id.Value, r.Code, r.DiscountType, r.Amount, r.MinimumSubtotal, r.MaximumDiscountAmount, r.StartsAt, r.ExpiresAt, r.IsActive, ct);
}

public sealed record DeleteStoreCouponCommand(int Id) : IRequest;
public sealed class DeleteStoreCouponCommandHandler(IStoreAdminService service) : IRequestHandler<DeleteStoreCouponCommand>
{
    public Task Handle(DeleteStoreCouponCommand request, CancellationToken cancellationToken) =>
        service.DeleteCouponAsync(request.Id, cancellationToken);
}

public sealed record ListStoreOrdersQuery(int Page, int PageSize, string? Search, StoreOrderStatus? Status) : IRequest<PagedResult<StoreOrderDto>>;
public sealed class ListStoreOrdersQueryHandler(IStoreOrderService service) : IRequestHandler<ListStoreOrdersQuery, PagedResult<StoreOrderDto>>
{
    public Task<PagedResult<StoreOrderDto>> Handle(ListStoreOrdersQuery request, CancellationToken cancellationToken) =>
        service.ListOrdersAsync(request.Page, request.PageSize, request.Search, request.Status, cancellationToken);
}

public sealed record GetStoreOrderQuery(int Id) : IRequest<StoreOrderDetailsDto>;
public sealed class GetStoreOrderQueryHandler(IStoreOrderService service) : IRequestHandler<GetStoreOrderQuery, StoreOrderDetailsDto>
{
    public Task<StoreOrderDetailsDto> Handle(GetStoreOrderQuery request, CancellationToken cancellationToken) =>
        service.GetOrderAsync(request.Id, cancellationToken);
}

public sealed record UpdateStoreOrderStatusCommand(int Id, StoreOrderStatus Status) : IRequest<StoreOrderDetailsDto>;
public sealed class UpdateStoreOrderStatusCommandHandler(IStoreOrderService service) : IRequestHandler<UpdateStoreOrderStatusCommand, StoreOrderDetailsDto>
{
    public Task<StoreOrderDetailsDto> Handle(UpdateStoreOrderStatusCommand request, CancellationToken cancellationToken) =>
        service.UpdateStatusAsync(request.Id, request.Status, cancellationToken);
}
