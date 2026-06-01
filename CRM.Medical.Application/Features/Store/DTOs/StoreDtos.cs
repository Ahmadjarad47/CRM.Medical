using CRM.Medical.Domain.Enums;

namespace CRM.Medical.Application.Features.Store.DTOs;

public sealed record ProductCategoryDto(
    int Id,
    string NameAr,
    string NameEn,
    string? Description,
    string? ImageUrl,
    int? ParentCategoryId,
    int DisplayOrder,
    bool IsActive,
    IReadOnlyList<ProductCategoryDto> Subcategories);

public sealed record ProductCardDto(
    int Id,
    int CategoryId,
    string NameAr,
    string NameEn,
    string ImageUrl,
    string SaleUnit,
    decimal Price,
    decimal? DiscountPrice,
    decimal SavedAmount,
    string? TopBadge);

public sealed record ProductDetailsDto(
    int Id,
    int CategoryId,
    string CategoryNameAr,
    string CategoryNameEn,
    string NameAr,
    string NameEn,
    string? Description,
    string ImageUrl,
    string SaleUnit,
    decimal Price,
    decimal? DiscountPrice,
    decimal SavedAmount,
    string? TopBadge,
    bool IsRecommended,
    bool IsBestSeller,
    bool IsActive);

public sealed record StoreBannerDto(
    int Id,
    string Title,
    string ImageUrl,
    string? LinkUrl,
    string Location,
    int? CategoryId,
    int DisplayOrder);

public sealed record StoreSliderDto(
    int Id,
    string Title,
    StoreSliderType Type,
    int DisplayOrder,
    IReadOnlyList<ProductCardDto> Products);

public sealed record StoreSettingDto(
    int Id,
    string AnnouncementHeader,
    string ServiceTitle,
    string ServiceDescription,
    decimal DeliveryFee,
    string DeliveryDurationText,
    bool CashOnDeliveryEnabled,
    bool OnlinePaymentEnabled);

public sealed record StoreHomeDto(
    StoreSettingDto Settings,
    IReadOnlyList<ProductCategoryDto> Categories,
    IReadOnlyList<StoreBannerDto> Banners,
    IReadOnlyList<StoreSliderDto> Sliders);

public sealed record CategoryPageDto(
    ProductCategoryDto Category,
    IReadOnlyList<ProductCategoryDto> Subcategories,
    IReadOnlyList<ProductCardDto> Products,
    IReadOnlyList<StoreBannerDto> Banners,
    IReadOnlyList<StoreSliderDto> Sliders);

public sealed record CartItemDto(
    int Id,
    int ProductId,
    string ProductNameAr,
    string ProductNameEn,
    string ImageUrl,
    string SaleUnit,
    decimal UnitPrice,
    decimal? DiscountPrice,
    decimal EffectiveUnitPrice,
    int Quantity,
    decimal LineTotal);

public sealed record CartDto(
    int Id,
    IReadOnlyList<CartItemDto> Items,
    string? CouponCode,
    decimal Subtotal,
    decimal DiscountAmount,
    decimal DeliveryFee,
    decimal Total,
    string DeliveryDurationText);

public sealed record CheckoutRequestDto(PaymentMethod PaymentMethod, string? Notes);

public sealed record StoreOrderItemDto(
    int Id,
    int ProductId,
    string ProductNameSnapshot,
    string SaleUnitSnapshot,
    string ImageSnapshot,
    decimal UnitPriceSnapshot,
    decimal? DiscountPriceSnapshot,
    int Quantity,
    decimal LineTotal);

public sealed record StoreOrderDto(
    int Id,
    string OrderNumber,
    StoreOrderStatus Status,
    PaymentMethod PaymentMethod,
    PaymentStatus PaymentStatus,
    decimal Subtotal,
    decimal DiscountAmount,
    decimal DeliveryFee,
    decimal Total,
    string? CouponCodeSnapshot,
    string DeliveryDurationSnapshot,
    DateTime OrderedAt);

public sealed record StoreOrderDetailsDto(
    int Id,
    string OrderNumber,
    string LabClientId,
    StoreOrderStatus Status,
    PaymentMethod PaymentMethod,
    PaymentStatus PaymentStatus,
    decimal Subtotal,
    decimal DiscountAmount,
    decimal DeliveryFee,
    decimal Total,
    string? CouponCodeSnapshot,
    string DeliveryDurationSnapshot,
    string? Notes,
    DateTime OrderedAt,
    IReadOnlyList<StoreOrderItemDto> Items);

public sealed record CouponDto(
    int Id,
    string Code,
    DiscountType DiscountType,
    decimal Amount,
    decimal? MinimumSubtotal,
    decimal? MaximumDiscountAmount,
    DateTime? StartsAt,
    DateTime? ExpiresAt,
    bool IsActive);
