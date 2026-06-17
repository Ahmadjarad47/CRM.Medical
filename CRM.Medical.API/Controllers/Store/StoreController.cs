using CRM.Medical.API.Contracts.Admin.Store;
using CRM.Medical.API.Contracts.Common;
using CRM.Medical.API.Contracts.Store;
using CRM.Medical.API.Extensions;
using CRM.Medical.Application.Common.Responses;
using CRM.Medical.Application.Common.Storage;
using CRM.Medical.Application.Exceptions;
using CRM.Medical.Application.Features.Store.Commands.AddStoreCartItem;
using CRM.Medical.Application.Features.Store.Commands.ApplyStoreCoupon;
using CRM.Medical.Application.Features.Store.Commands.CreateStoreCategory;
using CRM.Medical.Application.Features.Store.Commands.DeleteStoreBanner;
using CRM.Medical.Application.Features.Store.Commands.DeleteStoreCategory;
using CRM.Medical.Application.Features.Store.Commands.DeleteStoreCoupon;
using CRM.Medical.Application.Features.Store.Commands.DeleteStoreProduct;
using CRM.Medical.Application.Features.Store.Commands.DeleteStoreSlider;
using CRM.Medical.Application.Features.Store.Commands.RemoveStoreCartItem;
using CRM.Medical.Application.Features.Store.Commands.RemoveStoreCoupon;
using CRM.Medical.Application.Features.Store.Commands.SaveStoreBanner;
using CRM.Medical.Application.Features.Store.Commands.SaveStoreCoupon;
using CRM.Medical.Application.Features.Store.Commands.SaveStoreProduct;
using CRM.Medical.Application.Features.Store.Commands.SaveStoreSlider;
using CRM.Medical.Application.Features.Store.Commands.StoreCheckout;
using CRM.Medical.Application.Features.Store.Commands.UpdateStoreCartItem;
using CRM.Medical.Application.Features.Store.Commands.UpdateStoreCategory;
using CRM.Medical.Application.Features.Store.Commands.UpdateStoreOrderStatus;
using CRM.Medical.Application.Features.Store.Commands.UpdateStoreSettings;
using CRM.Medical.Application.Features.Store.DTOs;
using CRM.Medical.Application.Features.Store.Queries.GetMyStoreOrder;
using CRM.Medical.Application.Features.Store.Queries.GetStoreCart;
using CRM.Medical.Application.Features.Store.Queries.GetStoreCategory;
using CRM.Medical.Application.Features.Store.Queries.GetStoreCategoryPage;
using CRM.Medical.Application.Features.Store.Queries.GetStoreHome;
using CRM.Medical.Application.Features.Store.Queries.GetStoreOrder;
using CRM.Medical.Application.Features.Store.Queries.GetStoreProduct;
using CRM.Medical.Application.Features.Store.Queries.GetStoreSettings;
using CRM.Medical.Application.Features.Store.Queries.ListMyStoreOrders;
using CRM.Medical.Application.Features.Store.Queries.ListStoreBanners;
using CRM.Medical.Application.Features.Store.Queries.ListStoreCategories;
using CRM.Medical.Application.Features.Store.Queries.ListStoreCoupons;
using CRM.Medical.Application.Features.Store.Queries.ListStoreOrders;
using CRM.Medical.Application.Features.Store.Queries.ListStoreProducts;
using CRM.Medical.Application.Features.Store.Queries.ListStoreSliders;
using CRM.Medical.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Medical.API.Controllers.Store;

[ApiController]
[Route("api/store")]
public sealed class StoreController(ISender mediator) : ControllerBase
{
    [AllowAnonymous]
    [HttpGet("home")]
    [ProducesResponseType(typeof(StoreHomeDto), StatusCodes.Status200OK)]
    public Task<StoreHomeDto> Home(CancellationToken ct) =>
        mediator.Send(new GetStoreHomeQuery(), ct);

    [AllowAnonymous]
    [HttpGet("categories")]
    [ProducesResponseType(typeof(IReadOnlyList<ProductCategoryDto>), StatusCodes.Status200OK)]
    public Task<IReadOnlyList<ProductCategoryDto>> Categories(
        [FromQuery] bool includeInactive = false,
        CancellationToken ct = default) =>
        mediator.Send(new ListStoreCategoriesQuery(!includeInactive), ct);

    [AllowAnonymous]
    [HttpGet("categories/{id:int}")]
    [ProducesResponseType(typeof(ProductCategoryDto), StatusCodes.Status200OK)]
    public Task<ProductCategoryDto> Category(
        int id,
        [FromQuery] bool includeInactive = false,
        CancellationToken ct = default) =>
        mediator.Send(new GetStoreCategoryQuery(id, !includeInactive), ct);

    [AllowAnonymous]
    [HttpGet("categories/{id:int}/page")]
    [ProducesResponseType(typeof(CategoryPageDto), StatusCodes.Status200OK)]
    public Task<CategoryPageDto> CategoryPage(int id, CancellationToken ct) =>
        mediator.Send(new GetStoreCategoryPageQuery(id), ct);

    [AllowAnonymous]
    [HttpPost("categories")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(ProductCategoryDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateCategory(
        [FromForm] SaveProductCategoryRequest request,
        [FromServices] IFileStorageService fileStorage,
        CancellationToken ct)
    {
        var imageUrl = await ResolveOptionalImageUrlAsync(request.Image, existingUrl: null, fileStorage, ct);
        var dto = await mediator.Send(new CreateStoreCategoryCommand(
            request.NameAr,
            request.NameEn,
            request.Description,
            imageUrl,
            request.ParentCategoryId,
            request.DisplayOrder,
            request.IsActive), ct);
        return StatusCode(StatusCodes.Status201Created, dto);
    }

    [AllowAnonymous]
    [HttpPut("categories/{id:int}")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(ProductCategoryDto), StatusCodes.Status200OK)]
    public async Task<ProductCategoryDto> UpdateCategory(
        int id,
        [FromForm] SaveProductCategoryRequest request,
        [FromServices] IFileStorageService fileStorage,
        CancellationToken ct)
    {
        var existing = await mediator.Send(new GetStoreCategoryQuery(id, ActiveOnly: false), ct);
        var imageUrl = await ResolveOptionalImageUrlAsync(request.Image, existing.ImageUrl, fileStorage, ct);
        return await mediator.Send(new UpdateStoreCategoryCommand(
            id,
            request.NameAr,
            request.NameEn,
            request.Description,
            imageUrl,
            request.ParentCategoryId,
            request.DisplayOrder,
            request.IsActive), ct);
    }

    [AllowAnonymous]
    [HttpDelete("categories/{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteCategory(int id, CancellationToken ct)
    {
        await mediator.Send(new DeleteStoreCategoryCommand(id), ct);
        return NoContent();
    }

    [AllowAnonymous]
    [HttpGet("products")]
    [ProducesResponseType(typeof(PagedResult<ProductCardDto>), StatusCodes.Status200OK)]
    public Task<PagedResult<ProductCardDto>> Products(
        [FromQuery] PagedSearchRequest request,
        [FromQuery] int? categoryId,
        [FromQuery] bool includeInactive = false,
        CancellationToken ct = default) =>
        mediator.Send(new ListStoreProductsQuery(request.Page, request.PageSize, request.Search, categoryId, !includeInactive), ct);

    [AllowAnonymous]
    [HttpGet("products/{id:int}")]
    [ProducesResponseType(typeof(ProductDetailsDto), StatusCodes.Status200OK)]
    public Task<ProductDetailsDto> Product(
        int id,
        [FromQuery] bool includeInactive = false,
        CancellationToken ct = default) =>
        mediator.Send(new GetStoreProductQuery(id, !includeInactive), ct);

    [AllowAnonymous]
    [HttpPost("products")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(ProductDetailsDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateProduct(
        [FromForm] SaveProductRequest request,
        [FromServices] IFileStorageService fileStorage,
        CancellationToken ct)
    {
        var imageUrl = await ResolveRequiredImageUrlAsync(request.Image, existingUrl: null, fileStorage, ct);
        var dto = await mediator.Send(ToSaveProductCommand(null, request, imageUrl), ct);
        return StatusCode(StatusCodes.Status201Created, dto);
    }

    [AllowAnonymous]
    [HttpPut("products/{id:int}")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(ProductDetailsDto), StatusCodes.Status200OK)]
    public async Task<ProductDetailsDto> UpdateProduct(
        int id,
        [FromForm] SaveProductRequest request,
        [FromServices] IFileStorageService fileStorage,
        CancellationToken ct)
    {
        var existing = await mediator.Send(new GetStoreProductQuery(id, ActiveOnly: false), ct);
        var imageUrl = await ResolveRequiredImageUrlAsync(request.Image, existing.ImageUrl, fileStorage, ct);
        return await mediator.Send(ToSaveProductCommand(id, request, imageUrl), ct);
    }

    [AllowAnonymous]
    [HttpDelete("products/{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteProduct(int id, CancellationToken ct)
    {
        await mediator.Send(new DeleteStoreProductCommand(id), ct);
        return NoContent();
    }

    [Authorize]
    [HttpGet("cart")]
    [ProducesResponseType(typeof(CartDto), StatusCodes.Status200OK)]
    public Task<CartDto> GetCart(CancellationToken ct) =>
        mediator.Send(new GetStoreCartQuery(User.GetRequiredUserId()), ct);

    [Authorize]
    [HttpPost("cart/items")]
    [ProducesResponseType(typeof(CartDto), StatusCodes.Status200OK)]
    public Task<CartDto> AddCartItem([FromBody] AddCartItemRequest request, CancellationToken ct) =>
        mediator.Send(new AddStoreCartItemCommand(User.GetRequiredUserId(), request.ProductId, request.Quantity), ct);

    [Authorize]
    [HttpPut("cart/items/{id:int}")]
    [ProducesResponseType(typeof(CartDto), StatusCodes.Status200OK)]
    public Task<CartDto> UpdateCartItem(int id, [FromBody] UpdateCartItemRequest request, CancellationToken ct) =>
        mediator.Send(new UpdateStoreCartItemCommand(User.GetRequiredUserId(), id, request.Quantity), ct);

    [Authorize]
    [HttpDelete("cart/items/{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> RemoveCartItem(int id, CancellationToken ct)
    {
        await mediator.Send(new RemoveStoreCartItemCommand(User.GetRequiredUserId(), id), ct);
        return NoContent();
    }

    [Authorize]
    [HttpPost("cart/apply-coupon")]
    [ProducesResponseType(typeof(CartDto), StatusCodes.Status200OK)]
    public Task<CartDto> ApplyCoupon([FromBody] ApplyCouponRequest request, CancellationToken ct) =>
        mediator.Send(new ApplyStoreCouponCommand(User.GetRequiredUserId(), request.Code), ct);

    [Authorize]
    [HttpDelete("cart/coupon")]
    [ProducesResponseType(typeof(CartDto), StatusCodes.Status200OK)]
    public Task<CartDto> RemoveCoupon(CancellationToken ct) =>
        mediator.Send(new RemoveStoreCouponCommand(User.GetRequiredUserId()), ct);

    [Authorize]
    [HttpPost("checkout")]
    [ProducesResponseType(typeof(StoreOrderDetailsDto), StatusCodes.Status200OK)]
    public Task<StoreOrderDetailsDto> Checkout([FromBody] CheckoutRequest request, CancellationToken ct) =>
        mediator.Send(new StoreCheckoutCommand(User.GetRequiredUserId(), request.PaymentMethod, request.Notes), ct);

    [Authorize]
    [HttpGet("orders/my")]
    [ProducesResponseType(typeof(PagedResult<StoreOrderDto>), StatusCodes.Status200OK)]
    public Task<PagedResult<StoreOrderDto>> MyOrders(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default) =>
        mediator.Send(new ListMyStoreOrdersQuery(User.GetRequiredUserId(), page, pageSize), ct);

    [Authorize]
    [HttpGet("orders/my/{id:int}")]
    [ProducesResponseType(typeof(StoreOrderDetailsDto), StatusCodes.Status200OK)]
    public Task<StoreOrderDetailsDto> MyOrder(int id, CancellationToken ct) =>
        mediator.Send(new GetMyStoreOrderQuery(User.GetRequiredUserId(), id), ct);

    [AllowAnonymous]
    [HttpGet("settings")]
    [ProducesResponseType(typeof(StoreSettingDto), StatusCodes.Status200OK)]
    public Task<StoreSettingDto> GetSettings(CancellationToken ct) =>
        mediator.Send(new GetStoreSettingsQuery(), ct);

    [AllowAnonymous]
    [HttpPut("settings")]
    [ProducesResponseType(typeof(StoreSettingDto), StatusCodes.Status200OK)]
    public Task<StoreSettingDto> UpdateSettings([FromBody] UpdateStoreSettingsRequest request, CancellationToken ct) =>
        mediator.Send(new UpdateStoreSettingsCommand(new StoreSettingDto(
            0,
            request.AnnouncementHeader,
            request.ServiceTitle,
            request.ServiceDescription,
            request.DeliveryFee,
            request.DeliveryDurationText,
            request.CashOnDeliveryEnabled,
            request.OnlinePaymentEnabled)), ct);

    [AllowAnonymous]
    [HttpGet("sliders")]
    [ProducesResponseType(typeof(IReadOnlyList<StoreSliderDto>), StatusCodes.Status200OK)]
    public Task<IReadOnlyList<StoreSliderDto>> Sliders(CancellationToken ct) =>
        mediator.Send(new ListStoreSlidersQuery(), ct);

    [AllowAnonymous]
    [HttpPost("sliders")]
    [ProducesResponseType(typeof(StoreSliderDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateSlider([FromBody] SaveStoreSliderRequest request, CancellationToken ct)
    {
        var dto = await mediator.Send(new SaveStoreSliderCommand(null, request.Title, request.Type, request.DisplayOrder, request.IsActive, request.ProductIds), ct);
        return StatusCode(StatusCodes.Status201Created, dto);
    }

    [AllowAnonymous]
    [HttpPut("sliders/{id:int}")]
    [ProducesResponseType(typeof(StoreSliderDto), StatusCodes.Status200OK)]
    public Task<StoreSliderDto> UpdateSlider(int id, [FromBody] SaveStoreSliderRequest request, CancellationToken ct) =>
        mediator.Send(new SaveStoreSliderCommand(id, request.Title, request.Type, request.DisplayOrder, request.IsActive, request.ProductIds), ct);

    [AllowAnonymous]
    [HttpDelete("sliders/{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteSlider(int id, CancellationToken ct)
    {
        await mediator.Send(new DeleteStoreSliderCommand(id), ct);
        return NoContent();
    }

    [AllowAnonymous]
    [HttpGet("banners")]
    [ProducesResponseType(typeof(IReadOnlyList<StoreBannerDto>), StatusCodes.Status200OK)]
    public Task<IReadOnlyList<StoreBannerDto>> Banners(CancellationToken ct) =>
        mediator.Send(new ListStoreBannersQuery(), ct);

    [AllowAnonymous]
    [HttpPost("banners")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(StoreBannerDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateBanner(
        [FromForm] SaveStoreBannerRequest request,
        [FromServices] IFileStorageService fileStorage,
        CancellationToken ct)
    {
        var imageUrl = await ResolveRequiredBannerMediaUrlAsync(request.Image, existingUrl: null, fileStorage, ct);
        var dto = await mediator.Send(ToSaveBannerCommand(null, request, imageUrl), ct);
        return StatusCode(StatusCodes.Status201Created, dto);
    }

    [AllowAnonymous]
    [HttpPut("banners/{id:int}")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(StoreBannerDto), StatusCodes.Status200OK)]
    public async Task<StoreBannerDto> UpdateBanner(
        int id,
        [FromForm] SaveStoreBannerRequest request,
        [FromServices] IFileStorageService fileStorage,
        CancellationToken ct)
    {
        var banners = await mediator.Send(new ListStoreBannersQuery(), ct);
        var existing = banners.FirstOrDefault(b => b.Id == id)
            ?? throw new ApplicationNotFoundException($"Store banner '{id}' was not found.");
        var imageUrl = await ResolveRequiredBannerMediaUrlAsync(request.Image, existing.ImageUrl, fileStorage, ct);
        return await mediator.Send(ToSaveBannerCommand(id, request, imageUrl), ct);
    }

    [AllowAnonymous]
    [HttpDelete("banners/{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteBanner(int id, CancellationToken ct)
    {
        await mediator.Send(new DeleteStoreBannerCommand(id), ct);
        return NoContent();
    }

    [AllowAnonymous]
    [HttpGet("coupons")]
    [ProducesResponseType(typeof(IReadOnlyList<CouponDto>), StatusCodes.Status200OK)]
    public Task<IReadOnlyList<CouponDto>> Coupons(CancellationToken ct) =>
        mediator.Send(new ListStoreCouponsQuery(), ct);

    [AllowAnonymous]
    [HttpPost("coupons")]
    [ProducesResponseType(typeof(CouponDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateCoupon([FromBody] SaveCouponRequest request, CancellationToken ct)
    {
        var dto = await mediator.Send(ToSaveCouponCommand(null, request), ct);
        return StatusCode(StatusCodes.Status201Created, dto);
    }

    [AllowAnonymous]
    [HttpPut("coupons/{id:int}")]
    [ProducesResponseType(typeof(CouponDto), StatusCodes.Status200OK)]
    public Task<CouponDto> UpdateCoupon(int id, [FromBody] SaveCouponRequest request, CancellationToken ct) =>
        mediator.Send(ToSaveCouponCommand(id, request), ct);

    [AllowAnonymous]
    [HttpDelete("coupons/{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteCoupon(int id, CancellationToken ct)
    {
        await mediator.Send(new DeleteStoreCouponCommand(id), ct);
        return NoContent();
    }

    [AllowAnonymous]
    [HttpGet("orders")]
    [ProducesResponseType(typeof(PagedResult<StoreOrderDto>), StatusCodes.Status200OK)]
    public Task<PagedResult<StoreOrderDto>> Orders(
        [FromQuery] PagedSearchRequest request,
        [FromQuery] StoreOrderStatus? status,
        CancellationToken ct) =>
        mediator.Send(new ListStoreOrdersQuery(request.Page, request.PageSize, request.Search, status), ct);

    [AllowAnonymous]
    [HttpGet("orders/{id:int}")]
    [ProducesResponseType(typeof(StoreOrderDetailsDto), StatusCodes.Status200OK)]
    public Task<StoreOrderDetailsDto> Order(int id, CancellationToken ct) =>
        mediator.Send(new GetStoreOrderQuery(id), ct);

    [AllowAnonymous]
    [HttpPut("orders/{id:int}/status")]
    [ProducesResponseType(typeof(StoreOrderDetailsDto), StatusCodes.Status200OK)]
    public Task<StoreOrderDetailsDto> UpdateOrderStatus(int id, [FromBody] UpdateStoreOrderStatusRequest request, CancellationToken ct) =>
        mediator.Send(new UpdateStoreOrderStatusCommand(id, request.Status), ct);

    private static SaveStoreProductCommand ToSaveProductCommand(int? id, SaveProductRequest request, string imageUrl) =>
        new(
            id,
            request.CategoryId,
            request.NameAr,
            request.NameEn,
            request.Description,
            imageUrl,
            request.SaleUnit,
            request.Price,
            request.DiscountPrice,
            request.TopBadge,
            request.DisplayOrder,
            request.IsRecommended,
            request.IsBestSeller,
            request.IsActive);

    private static SaveStoreBannerCommand ToSaveBannerCommand(int? id, SaveStoreBannerRequest request, string imageUrl) =>
        new(
            id,
            request.Title,
            imageUrl,
            request.LinkUrl,
            request.Location,
            request.CategoryId,
            request.DisplayOrder,
            request.IsActive,
            request.StartsAt,
            request.EndsAt);

    private static async Task<string?> ResolveOptionalImageUrlAsync(
        IFormFile? image,
        string? existingUrl,
        IFileStorageService fileStorage,
        CancellationToken cancellationToken) =>
        image is { Length: > 0 }
            ? await fileStorage.UploadImageAsync(image, cancellationToken)
            : existingUrl;

    private static async Task<string> ResolveRequiredImageUrlAsync(
        IFormFile? image,
        string? existingUrl,
        IFileStorageService fileStorage,
        CancellationToken cancellationToken)
    {
        if (image is { Length: > 0 })
            return await fileStorage.UploadImageAsync(image, cancellationToken);

        if (!string.IsNullOrWhiteSpace(existingUrl))
            return existingUrl.Trim();

        throw new ApplicationBadRequestException("Image file is required.");
    }

    private static async Task<string> ResolveRequiredBannerMediaUrlAsync(
        IFormFile? image,
        string? existingUrl,
        IFileStorageService fileStorage,
        CancellationToken cancellationToken)
    {
        if (image is { Length: > 0 })
            return await fileStorage.UploadFileAsync(image, "banners", cancellationToken);

        if (!string.IsNullOrWhiteSpace(existingUrl))
            return existingUrl.Trim();

        throw new ApplicationBadRequestException("Banner image or media file is required.");
    }

    private static SaveStoreCouponCommand ToSaveCouponCommand(int? id, SaveCouponRequest request) =>
        new(
            id,
            request.Code,
            request.DiscountType,
            request.Amount,
            request.MinimumSubtotal,
            request.MaximumDiscountAmount,
            request.StartsAt,
            request.ExpiresAt,
            request.IsActive);
}
