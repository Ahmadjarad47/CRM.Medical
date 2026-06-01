using CRM.Medical.Domain.Enums;

namespace CRM.Medical.API.Contracts.Admin.Store;

public sealed record UpdateStoreSettingsRequest(
    string AnnouncementHeader,
    string ServiceTitle,
    string ServiceDescription,
    decimal DeliveryFee,
    string DeliveryDurationText,
    bool CashOnDeliveryEnabled,
    bool OnlinePaymentEnabled);

public sealed record SaveProductCategoryRequest(
    string NameAr,
    string NameEn,
    string? Description,
    string? ImageUrl,
    int? ParentCategoryId,
    int DisplayOrder,
    bool IsActive);

public sealed record SaveProductRequest(
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
    bool IsActive);

public sealed record SaveStoreSliderRequest(
    string Title,
    StoreSliderType Type,
    int DisplayOrder,
    bool IsActive,
    IReadOnlyList<int> ProductIds);

public sealed record SaveStoreBannerRequest(
    string Title,
    string ImageUrl,
    string? LinkUrl,
    string Location,
    int? CategoryId,
    int DisplayOrder,
    bool IsActive,
    DateTime? StartsAt,
    DateTime? EndsAt);

public sealed record SaveCouponRequest(
    string Code,
    DiscountType DiscountType,
    decimal Amount,
    decimal? MinimumSubtotal,
    decimal? MaximumDiscountAmount,
    DateTime? StartsAt,
    DateTime? ExpiresAt,
    bool IsActive);

public sealed record UpdateStoreOrderStatusRequest(StoreOrderStatus Status);
