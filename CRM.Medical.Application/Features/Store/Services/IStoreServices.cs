using CRM.Medical.Application.Common.Responses;
using CRM.Medical.Application.Features.Store.DTOs;
using CRM.Medical.Domain.Enums;

namespace CRM.Medical.Application.Features.Store.Services;

public interface IStoreHomeService
{
    Task<StoreHomeDto> GetHomeAsync(CancellationToken cancellationToken);
    Task<CategoryPageDto> GetCategoryPageAsync(int categoryId, CancellationToken cancellationToken);
}

public interface IProductCatalogService
{
    Task<IReadOnlyList<ProductCategoryDto>> ListCategoriesAsync(bool activeOnly, CancellationToken cancellationToken);
    Task<ProductCategoryDto> GetCategoryAsync(int id, bool activeOnly, CancellationToken cancellationToken);
    Task<PagedResult<ProductCardDto>> ListProductsAsync(
        int page,
        int pageSize,
        string? search,
        int? categoryId,
        bool activeOnly,
        CancellationToken cancellationToken);
    Task<ProductDetailsDto> GetProductAsync(int id, bool activeOnly, CancellationToken cancellationToken);
}

public interface ICartService
{
    Task<CartDto> GetAsync(string labClientId, CancellationToken cancellationToken);
    Task<CartDto> AddItemAsync(string labClientId, int productId, int quantity, CancellationToken cancellationToken);
    Task<CartDto> UpdateItemAsync(string labClientId, int cartItemId, int quantity, CancellationToken cancellationToken);
    Task RemoveItemAsync(string labClientId, int cartItemId, CancellationToken cancellationToken);
    Task<CartDto> ApplyCouponAsync(string labClientId, string code, CancellationToken cancellationToken);
    Task<CartDto> RemoveCouponAsync(string labClientId, CancellationToken cancellationToken);
}

public interface ICheckoutService
{
    Task<StoreOrderDetailsDto> CheckoutAsync(
        string labClientId,
        PaymentMethod paymentMethod,
        string? notes,
        CancellationToken cancellationToken);
}

public interface IStoreOrderService
{
    Task<PagedResult<StoreOrderDto>> ListMyOrdersAsync(
        string labClientId,
        int page,
        int pageSize,
        CancellationToken cancellationToken);

    Task<StoreOrderDetailsDto> GetMyOrderAsync(
        string labClientId,
        int id,
        CancellationToken cancellationToken);

    Task<PagedResult<StoreOrderDto>> ListOrdersAsync(
        int page,
        int pageSize,
        string? search,
        StoreOrderStatus? status,
        CancellationToken cancellationToken);

    Task<StoreOrderDetailsDto> GetOrderAsync(int id, CancellationToken cancellationToken);
    Task<StoreOrderDetailsDto> UpdateStatusAsync(int id, StoreOrderStatus status, CancellationToken cancellationToken);
}

public interface IStoreAdminService
{
    Task<StoreSettingDto> GetSettingsAsync(CancellationToken cancellationToken);
    Task<StoreSettingDto> UpdateSettingsAsync(StoreSettingDto request, CancellationToken cancellationToken);

    Task<ProductCategoryDto> CreateCategoryAsync(
        string nameAr,
        string nameEn,
        string? description,
        string? imageUrl,
        int? parentCategoryId,
        int displayOrder,
        bool isActive,
        CancellationToken cancellationToken);

    Task<ProductCategoryDto> UpdateCategoryAsync(
        int id,
        string nameAr,
        string nameEn,
        string? description,
        string? imageUrl,
        int? parentCategoryId,
        int displayOrder,
        bool isActive,
        CancellationToken cancellationToken);

    Task DeleteCategoryAsync(int id, CancellationToken cancellationToken);

    Task<ProductDetailsDto> CreateProductAsync(
        int categoryId,
        string nameAr,
        string nameEn,
        string? description,
        string imageUrl,
        string saleUnit,
        decimal price,
        decimal? discountPrice,
        string? topBadge,
        int displayOrder,
        bool isRecommended,
        bool isBestSeller,
        bool isActive,
        CancellationToken cancellationToken);

    Task<ProductDetailsDto> UpdateProductAsync(
        int id,
        int categoryId,
        string nameAr,
        string nameEn,
        string? description,
        string imageUrl,
        string saleUnit,
        decimal price,
        decimal? discountPrice,
        string? topBadge,
        int displayOrder,
        bool isRecommended,
        bool isBestSeller,
        bool isActive,
        CancellationToken cancellationToken);

    Task DeleteProductAsync(int id, CancellationToken cancellationToken);

    Task<IReadOnlyList<StoreSliderDto>> ListSlidersAsync(CancellationToken cancellationToken);
    Task<StoreSliderDto> CreateSliderAsync(string title, StoreSliderType type, int displayOrder, bool isActive, IReadOnlyList<int> productIds, CancellationToken cancellationToken);
    Task<StoreSliderDto> UpdateSliderAsync(int id, string title, StoreSliderType type, int displayOrder, bool isActive, IReadOnlyList<int> productIds, CancellationToken cancellationToken);
    Task DeleteSliderAsync(int id, CancellationToken cancellationToken);

    Task<IReadOnlyList<StoreBannerDto>> ListBannersAsync(CancellationToken cancellationToken);
    Task<StoreBannerDto> CreateBannerAsync(string title, string imageUrl, string? linkUrl, string location, int? categoryId, int displayOrder, bool isActive, DateTime? startsAt, DateTime? endsAt, CancellationToken cancellationToken);
    Task<StoreBannerDto> UpdateBannerAsync(int id, string title, string imageUrl, string? linkUrl, string location, int? categoryId, int displayOrder, bool isActive, DateTime? startsAt, DateTime? endsAt, CancellationToken cancellationToken);
    Task DeleteBannerAsync(int id, CancellationToken cancellationToken);

    Task<IReadOnlyList<CouponDto>> ListCouponsAsync(CancellationToken cancellationToken);
    Task<CouponDto> CreateCouponAsync(string code, DiscountType discountType, decimal amount, decimal? minimumSubtotal, decimal? maximumDiscountAmount, DateTime? startsAt, DateTime? expiresAt, bool isActive, CancellationToken cancellationToken);
    Task<CouponDto> UpdateCouponAsync(int id, string code, DiscountType discountType, decimal amount, decimal? minimumSubtotal, decimal? maximumDiscountAmount, DateTime? startsAt, DateTime? expiresAt, bool isActive, CancellationToken cancellationToken);
    Task DeleteCouponAsync(int id, CancellationToken cancellationToken);
}
