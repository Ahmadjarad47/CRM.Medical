using CRM.Medical.Application.Common.Queries;
using CRM.Medical.Application.Common.Responses;
using CRM.Medical.Application.Common.Time;
using CRM.Medical.Application.Authorization;
using CRM.Medical.Application.Exceptions;
using CRM.Medical.Application.Features.Store.DTOs;
using CRM.Medical.Application.Features.Store.Services;
using CRM.Medical.Domain.Entities.Store;
using CRM.Medical.Domain.Enums;
using CRM.Medical.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CRM.Medical.Infrastructure.Store;

public sealed class StoreServices(MedicalDbContext db, IDateTimeProvider clock, IAccessPolicyEvaluator accessPolicyEvaluator)
    : IStoreHomeService,
        IProductCatalogService,
        ICartService,
        ICheckoutService,
        IStoreOrderService,
        IStoreAdminService
{
    public async Task<StoreHomeDto> GetHomeAsync(CancellationToken cancellationToken)
    {
        var settings = await GetOrCreateSettingsAsync(cancellationToken);
        var categories = await BuildCategoryTreeAsync(activeOnly: true, cancellationToken);
        var banners = await ActiveBanners().Where(b => b.CategoryId == null)
            .OrderBy(b => b.DisplayOrder)
            .Select(b => MapBanner(b))
            .ToListAsync(cancellationToken);
        var sliders = await ListActiveSlidersAsync(cancellationToken);

        return new StoreHomeDto(MapSetting(settings), categories, banners, sliders);
    }

    public async Task<CategoryPageDto> GetCategoryPageAsync(int categoryId, CancellationToken cancellationToken)
    {
        var category = await db.ProductCategories
            .AsNoTracking()
            .Include(c => c.Subcategories.Where(s => s.IsActive))
            .FirstOrDefaultAsync(c => c.Id == categoryId && c.IsActive, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Store category '{categoryId}' was not found.");

        var products = await db.Products
            .AsNoTracking()
            .Where(p => p.CategoryId == categoryId && p.IsActive && p.Category.IsActive)
            .OrderBy(p => p.DisplayOrder)
            .ThenBy(p => p.Id)
            .Select(p => MapProductCard(p))
            .ToListAsync(cancellationToken);

        var banners = await ActiveBanners()
            .Where(b => b.CategoryId == categoryId)
            .OrderBy(b => b.DisplayOrder)
            .Select(b => MapBanner(b))
            .ToListAsync(cancellationToken);

        return new CategoryPageDto(
            MapCategory(category, category.Subcategories.Where(s => s.IsActive).Select(s => MapCategory(s)).ToList()),
            category.Subcategories.Where(s => s.IsActive).OrderBy(s => s.DisplayOrder).Select(s => MapCategory(s)).ToList(),
            products,
            banners,
            await ListActiveSlidersAsync(cancellationToken));
    }

    public Task<IReadOnlyList<ProductCategoryDto>> ListCategoriesAsync(
        bool activeOnly,
        CancellationToken cancellationToken) =>
        BuildCategoryTreeAsync(activeOnly, cancellationToken);

    public async Task<ProductCategoryDto> GetCategoryAsync(int id, bool activeOnly, CancellationToken cancellationToken)
    {
        var query = db.ProductCategories
            .AsNoTracking()
            .Include(c => c.Subcategories)
            .AsQueryable();
        if (activeOnly)
            query = query.Where(c => c.IsActive);
        else
            query = await accessPolicyEvaluator.ApplyAsync(query, "store_product_categories", "read", cancellationToken);

        var category = await query.FirstOrDefaultAsync(c => c.Id == id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Store category '{id}' was not found.");

        return MapCategory(
            category,
            category.Subcategories
                .Where(s => !activeOnly || s.IsActive)
                .OrderBy(s => s.DisplayOrder)
                .Select(s => MapCategory(s))
                .ToList());
    }

    public async Task<PagedResult<ProductCardDto>> ListProductsAsync(
        int page,
        int pageSize,
        string? search,
        int? categoryId,
        bool activeOnly,
        CancellationToken cancellationToken)
    {
        var (normalizedPage, normalizedPageSize) = PaginationDefaults.Normalize(page, pageSize);
        var query = db.Products.AsNoTracking().Include(p => p.Category).AsQueryable();

        if (activeOnly)
            query = query.Where(p => p.IsActive && p.Category.IsActive);
        else
            query = await accessPolicyEvaluator.ApplyAsync(query, "store_products", "read", cancellationToken);

        if (categoryId is not null)
            query = query.Where(p => p.CategoryId == categoryId.Value);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            query = query.Where(p =>
                p.NameAr.ToLower().Contains(term) ||
                p.NameEn.ToLower().Contains(term) ||
                p.SaleUnit.ToLower().Contains(term));
        }

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(p => p.DisplayOrder)
            .ThenBy(p => p.Id)
            .ApplyPagination(normalizedPage, normalizedPageSize)
            .Select(p => MapProductCard(p))
            .ToListAsync(cancellationToken);

        return new PagedResult<ProductCardDto>
        {
            Items = items,
            Page = normalizedPage,
            PageSize = normalizedPageSize,
            TotalCount = total
        };
    }

    public async Task<ProductDetailsDto> GetProductAsync(int id, bool activeOnly, CancellationToken cancellationToken)
    {
        var query = db.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .AsQueryable();
        if (activeOnly)
            query = query.Where(p => p.IsActive && p.Category.IsActive);
        else
            query = await accessPolicyEvaluator.ApplyAsync(query, "store_products", "read", cancellationToken);

        var product = await query.FirstOrDefaultAsync(p => p.Id == id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Store product '{id}' was not found.");

        return MapProductDetails(product);
    }

    public async Task<CartDto> GetAsync(string labClientId, CancellationToken cancellationToken)
    {
        var cart = await GetOrCreateCartAsync(labClientId, cancellationToken);
        return await BuildCartDtoAsync(cart, cancellationToken);
    }

    public async Task<CartDto> AddItemAsync(
        string labClientId,
        int productId,
        int quantity,
        CancellationToken cancellationToken)
    {
        if (quantity <= 0)
            throw new ApplicationBadRequestException("Quantity must be greater than zero.");

        var product = await db.Products.Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == productId, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Store product '{productId}' was not found.");

        EnsureProductCanBeSold(product);

        var cart = await GetOrCreateCartAsync(labClientId, cancellationToken);
        var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
        if (item is null)
        {
            cart.Items.Add(new CartItem { ProductId = productId, Product = product, Quantity = quantity });
        }
        else
        {
            item.Quantity += quantity;
        }

        await db.SaveChangesAsync(cancellationToken);
        return await BuildCartDtoAsync(cart, cancellationToken);
    }

    public async Task<CartDto> UpdateItemAsync(
        string labClientId,
        int cartItemId,
        int quantity,
        CancellationToken cancellationToken)
    {
        if (quantity <= 0)
            throw new ApplicationBadRequestException("Quantity must be greater than zero.");

        var cart = await GetCartWithItemsAsync(labClientId, cancellationToken);
        var item = cart.Items.FirstOrDefault(i => i.Id == cartItemId)
            ?? throw new ApplicationNotFoundException($"Cart item '{cartItemId}' was not found.");

        EnsureProductCanBeSold(item.Product);
        item.Quantity = quantity;

        await db.SaveChangesAsync(cancellationToken);
        return await BuildCartDtoAsync(cart, cancellationToken);
    }

    public async Task RemoveItemAsync(string labClientId, int cartItemId, CancellationToken cancellationToken)
    {
        var cart = await GetCartWithItemsAsync(labClientId, cancellationToken);
        var item = cart.Items.FirstOrDefault(i => i.Id == cartItemId)
            ?? throw new ApplicationNotFoundException($"Cart item '{cartItemId}' was not found.");

        db.CartItems.Remove(item);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task<CartDto> ApplyCouponAsync(string labClientId, string code, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ApplicationBadRequestException("Coupon code is required.");

        var cart = await GetCartWithItemsAsync(labClientId, cancellationToken);
        if (cart.Items.Count == 0)
            throw new ApplicationBadRequestException("Cart is empty.");

        var coupon = await GetValidCouponAsync(code, cancellationToken);
        var subtotal = cart.Items.Sum(i => EffectivePrice(i.Product) * i.Quantity);
        ValidateCouponForSubtotal(coupon, subtotal);

        cart.CouponId = coupon.Id;
        await db.SaveChangesAsync(cancellationToken);
        return await BuildCartDtoAsync(cart, cancellationToken);
    }

    public async Task<CartDto> RemoveCouponAsync(string labClientId, CancellationToken cancellationToken)
    {
        var cart = await GetOrCreateCartAsync(labClientId, cancellationToken);
        cart.CouponId = null;
        await db.SaveChangesAsync(cancellationToken);
        return await BuildCartDtoAsync(cart, cancellationToken);
    }

    public async Task<StoreOrderDetailsDto> CheckoutAsync(
        string labClientId,
        PaymentMethod paymentMethod,
        string? notes,
        CancellationToken cancellationToken)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);

        var cart = await GetCartWithItemsAsync(labClientId, cancellationToken);
        if (cart.Items.Count == 0)
            throw new ApplicationBadRequestException("Cart is empty.");

        foreach (var item in cart.Items)
            EnsureProductCanBeSold(item.Product);

        var settings = await GetOrCreateSettingsAsync(cancellationToken);
        if (paymentMethod == PaymentMethod.Online && !settings.OnlinePaymentEnabled)
            throw new ApplicationBadRequestException("Online payment is currently disabled.");
        if (paymentMethod == PaymentMethod.CashOnDelivery && !settings.CashOnDeliveryEnabled)
            throw new ApplicationBadRequestException("Cash on delivery is currently disabled.");

        var subtotal = cart.Items.Sum(i => EffectivePrice(i.Product) * i.Quantity);
        var discount = 0m;
        Coupon? coupon = null;
        if (cart.CouponId is not null)
        {
            coupon = await db.Coupons.FirstOrDefaultAsync(c => c.Id == cart.CouponId.Value, cancellationToken)
                ?? throw new ApplicationBadRequestException("Coupon is no longer available.");
            EnsureCouponIsValid(coupon);
            ValidateCouponForSubtotal(coupon, subtotal);
            discount = CalculateCouponDiscount(coupon, subtotal);
        }

        var order = new StoreOrder
        {
            OrderNumber = CreateOrderNumber(),
            LabClientId = labClientId,
            Status = StoreOrderStatus.Pending,
            PaymentMethod = paymentMethod,
            PaymentStatus = paymentMethod == PaymentMethod.CashOnDelivery ? PaymentStatus.Pending : PaymentStatus.Pending,
            Subtotal = subtotal,
            DiscountAmount = discount,
            DeliveryFee = settings.DeliveryFee,
            Total = Math.Max(0, subtotal - discount + settings.DeliveryFee),
            CouponId = coupon?.Id,
            CouponCodeSnapshot = coupon?.Code,
            DeliveryDurationSnapshot = settings.DeliveryDurationText,
            Notes = notes?.Trim(),
            OrderedAt = clock.UtcNow
        };

        foreach (var item in cart.Items)
        {
            order.Items.Add(new StoreOrderItem
            {
                ProductId = item.ProductId,
                ProductNameSnapshot = item.Product.NameEn,
                SaleUnitSnapshot = item.Product.SaleUnit,
                ImageSnapshot = item.Product.ImageUrl,
                UnitPriceSnapshot = item.Product.Price,
                DiscountPriceSnapshot = item.Product.DiscountPrice,
                Quantity = item.Quantity,
                LineTotal = EffectivePrice(item.Product) * item.Quantity
            });
        }

        db.StoreOrders.Add(order);
        db.CartItems.RemoveRange(cart.Items);
        cart.CouponId = null;
        await db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return await GetOrderAsync(order.Id, cancellationToken);
    }

    public async Task<PagedResult<StoreOrderDto>> ListMyOrdersAsync(
        string labClientId,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var query = db.StoreOrders.AsNoTracking().Where(o => o.LabClientId == labClientId);
        return await PageOrdersAsync(query, page, pageSize, cancellationToken);
    }

    public async Task<StoreOrderDetailsDto> GetMyOrderAsync(
        string labClientId,
        int id,
        CancellationToken cancellationToken)
    {
        var order = await OrderDetailsQuery()
            .FirstOrDefaultAsync(o => o.Id == id && o.LabClientId == labClientId, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Store order '{id}' was not found.");

        return MapOrderDetails(order);
    }

    public async Task<PagedResult<StoreOrderDto>> ListOrdersAsync(
        int page,
        int pageSize,
        string? search,
        StoreOrderStatus? status,
        CancellationToken cancellationToken)
    {
        var query = db.StoreOrders.AsNoTracking().AsQueryable();
        query = await accessPolicyEvaluator.ApplyAsync(query, "store_orders", "read", cancellationToken);
        if (status is not null)
            query = query.Where(o => o.Status == status.Value);
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            query = query.Where(o =>
                o.OrderNumber.ToLower().Contains(term) ||
                o.LabClientId.ToLower().Contains(term) ||
                (o.CouponCodeSnapshot != null && o.CouponCodeSnapshot.ToLower().Contains(term)));
        }

        return await PageOrdersAsync(query, page, pageSize, cancellationToken);
    }

    public async Task<StoreOrderDetailsDto> GetOrderAsync(int id, CancellationToken cancellationToken)
    {
        var query = await accessPolicyEvaluator.ApplyAsync(OrderDetailsQuery(), "store_orders", "read", cancellationToken);
        var order = await query
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Store order '{id}' was not found.");

        return MapOrderDetails(order);
    }

    public async Task<StoreOrderDetailsDto> UpdateStatusAsync(
        int id,
        StoreOrderStatus status,
        CancellationToken cancellationToken)
    {
        var order = await db.StoreOrders
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Store order '{id}' was not found.");

        await RequireAccessAsync(order, "store_orders", "update", cancellationToken);
        order.Status = status;
        await db.SaveChangesAsync(cancellationToken);
        return await GetOrderAsync(id, cancellationToken);
    }

    public async Task<StoreSettingDto> GetSettingsAsync(CancellationToken cancellationToken)
    {
        var settings = await GetOrCreateSettingsAsync(cancellationToken);
        await RequireAccessAsync(settings, "store_settings", "read", cancellationToken);
        return MapSetting(settings);
    }

    public async Task<StoreSettingDto> UpdateSettingsAsync(
        StoreSettingDto request,
        CancellationToken cancellationToken)
    {
        if (request.DeliveryFee < 0)
            throw new ApplicationBadRequestException("Delivery fee cannot be negative.");

        var settings = await GetOrCreateSettingsAsync(cancellationToken);
        await RequireAccessAsync(settings, "store_settings", "update", cancellationToken);
        settings.AnnouncementHeader = request.AnnouncementHeader.Trim();
        settings.ServiceTitle = request.ServiceTitle.Trim();
        settings.ServiceDescription = request.ServiceDescription.Trim();
        settings.DeliveryFee = request.DeliveryFee;
        settings.DeliveryDurationText = request.DeliveryDurationText.Trim();
        settings.CashOnDeliveryEnabled = request.CashOnDeliveryEnabled;
        settings.OnlinePaymentEnabled = request.OnlinePaymentEnabled;

        await db.SaveChangesAsync(cancellationToken);
        return MapSetting(settings);
    }

    public async Task<ProductCategoryDto> CreateCategoryAsync(
        string nameAr,
        string nameEn,
        string? description,
        string? imageUrl,
        int? parentCategoryId,
        int displayOrder,
        bool isActive,
        CancellationToken cancellationToken)
    {
        await EnsureCategoryParentAllowedAsync(parentCategoryId, exceptCategoryId: null, cancellationToken);
        var entity = new ProductCategory
        {
            NameAr = nameAr.Trim(),
            NameEn = nameEn.Trim(),
            Description = description?.Trim(),
            ImageUrl = imageUrl?.Trim(),
            ParentCategoryId = parentCategoryId,
            DisplayOrder = displayOrder,
            IsActive = isActive
        };

        await RequireAccessAsync(entity, "store_product_categories", "create", cancellationToken);
        db.ProductCategories.Add(entity);
        await db.SaveChangesAsync(cancellationToken);
        return MapCategory(entity);
    }

    public async Task<ProductCategoryDto> UpdateCategoryAsync(
        int id,
        string nameAr,
        string nameEn,
        string? description,
        string? imageUrl,
        int? parentCategoryId,
        int displayOrder,
        bool isActive,
        CancellationToken cancellationToken)
    {
        var entity = await db.ProductCategories.FirstOrDefaultAsync(c => c.Id == id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Store category '{id}' was not found.");

        await RequireAccessAsync(entity, "store_product_categories", "update", cancellationToken);
        await EnsureCategoryParentAllowedAsync(parentCategoryId, id, cancellationToken);
        entity.NameAr = nameAr.Trim();
        entity.NameEn = nameEn.Trim();
        entity.Description = description?.Trim();
        entity.ImageUrl = imageUrl?.Trim();
        entity.ParentCategoryId = parentCategoryId;
        entity.DisplayOrder = displayOrder;
        entity.IsActive = isActive;

        await db.SaveChangesAsync(cancellationToken);
        return MapCategory(entity);
    }

    public async Task DeleteCategoryAsync(int id, CancellationToken cancellationToken)
    {
        var entity = await db.ProductCategories.FirstOrDefaultAsync(c => c.Id == id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Store category '{id}' was not found.");

        await RequireAccessAsync(entity, "store_product_categories", "delete", cancellationToken);
        var hasChildren = await db.ProductCategories.AnyAsync(c => c.ParentCategoryId == id, cancellationToken);
        var hasProducts = await db.Products.AnyAsync(p => p.CategoryId == id, cancellationToken);
        if (hasChildren || hasProducts)
            throw new ApplicationConflictException("Cannot delete a category that has subcategories or products.");

        db.ProductCategories.Remove(entity);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task<ProductDetailsDto> CreateProductAsync(
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
        CancellationToken cancellationToken)
    {
        await EnsureActiveCategoryExistsAsync(categoryId, cancellationToken);
        ValidateProductPrices(price, discountPrice);

        var entity = new Product
        {
            CategoryId = categoryId,
            NameAr = nameAr.Trim(),
            NameEn = nameEn.Trim(),
            Description = description?.Trim(),
            ImageUrl = imageUrl.Trim(),
            SaleUnit = saleUnit.Trim(),
            Price = price,
            DiscountPrice = discountPrice,
            TopBadge = topBadge?.Trim(),
            DisplayOrder = displayOrder,
            IsRecommended = isRecommended,
            IsBestSeller = isBestSeller,
            IsActive = isActive
        };

        await RequireAccessAsync(entity, "store_products", "create", cancellationToken);
        db.Products.Add(entity);
        await db.SaveChangesAsync(cancellationToken);
        return await GetProductAsync(entity.Id, activeOnly: false, cancellationToken);
    }

    public async Task<ProductDetailsDto> UpdateProductAsync(
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
        CancellationToken cancellationToken)
    {
        var entity = await db.Products.FirstOrDefaultAsync(p => p.Id == id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Store product '{id}' was not found.");

        await RequireAccessAsync(entity, "store_products", "update", cancellationToken);
        await EnsureActiveCategoryExistsAsync(categoryId, cancellationToken);
        ValidateProductPrices(price, discountPrice);

        entity.CategoryId = categoryId;
        entity.NameAr = nameAr.Trim();
        entity.NameEn = nameEn.Trim();
        entity.Description = description?.Trim();
        entity.ImageUrl = imageUrl.Trim();
        entity.SaleUnit = saleUnit.Trim();
        entity.Price = price;
        entity.DiscountPrice = discountPrice;
        entity.TopBadge = topBadge?.Trim();
        entity.DisplayOrder = displayOrder;
        entity.IsRecommended = isRecommended;
        entity.IsBestSeller = isBestSeller;
        entity.IsActive = isActive;

        await db.SaveChangesAsync(cancellationToken);
        return await GetProductAsync(id, activeOnly: false, cancellationToken);
    }

    public async Task DeleteProductAsync(int id, CancellationToken cancellationToken)
    {
        var entity = await db.Products.FirstOrDefaultAsync(p => p.Id == id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Store product '{id}' was not found.");

        await RequireAccessAsync(entity, "store_products", "delete", cancellationToken);
        var hasReferences =
            await db.StoreOrderItems.AnyAsync(i => i.ProductId == id, cancellationToken) ||
            await db.CartItems.AnyAsync(i => i.ProductId == id, cancellationToken) ||
            await db.StoreSliderProducts.AnyAsync(i => i.ProductId == id, cancellationToken);
        if (hasReferences)
        {
            // Preserve existing carts, sliders, and order history by deactivating referenced products.
            entity.IsActive = false;
        }
        else
        {
            db.Products.Remove(entity);
        }

        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<StoreSliderDto>> ListSlidersAsync(CancellationToken cancellationToken)
    {
        var query = await accessPolicyEvaluator.ApplyAsync(db.StoreSliders.AsNoTracking(), "store_sliders", "read", cancellationToken);
        var sliders = await query
            .Include(s => s.Products.OrderBy(sp => sp.DisplayOrder))
            .ThenInclude(sp => sp.Product)
            .OrderBy(s => s.DisplayOrder)
            .ToListAsync(cancellationToken);

        return sliders.Select(MapSlider).ToList();
    }

    public async Task<StoreSliderDto> CreateSliderAsync(
        string title,
        StoreSliderType type,
        int displayOrder,
        bool isActive,
        IReadOnlyList<int> productIds,
        CancellationToken cancellationToken)
    {
        await EnsureProductsExistAsync(productIds, cancellationToken);
        var entity = new StoreSlider
        {
            Title = title.Trim(),
            Type = type,
            DisplayOrder = displayOrder,
            IsActive = isActive
        };
        ApplySliderProducts(entity, productIds);
        await RequireAccessAsync(entity, "store_sliders", "create", cancellationToken);
        db.StoreSliders.Add(entity);
        await db.SaveChangesAsync(cancellationToken);
        return (await ListSlidersAsync(cancellationToken)).First(s => s.Id == entity.Id);
    }

    public async Task<StoreSliderDto> UpdateSliderAsync(
        int id,
        string title,
        StoreSliderType type,
        int displayOrder,
        bool isActive,
        IReadOnlyList<int> productIds,
        CancellationToken cancellationToken)
    {
        await EnsureProductsExistAsync(productIds, cancellationToken);
        var entity = await db.StoreSliders.Include(s => s.Products)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Store slider '{id}' was not found.");

        await RequireAccessAsync(entity, "store_sliders", "update", cancellationToken);
        entity.Title = title.Trim();
        entity.Type = type;
        entity.DisplayOrder = displayOrder;
        entity.IsActive = isActive;
        db.StoreSliderProducts.RemoveRange(entity.Products);
        entity.Products.Clear();
        ApplySliderProducts(entity, productIds);

        await db.SaveChangesAsync(cancellationToken);
        return (await ListSlidersAsync(cancellationToken)).First(s => s.Id == entity.Id);
    }

    public async Task DeleteSliderAsync(int id, CancellationToken cancellationToken)
    {
        var entity = await db.StoreSliders.FirstOrDefaultAsync(s => s.Id == id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Store slider '{id}' was not found.");
        await RequireAccessAsync(entity, "store_sliders", "delete", cancellationToken);
        db.StoreSliders.Remove(entity);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<StoreBannerDto>> ListBannersAsync(CancellationToken cancellationToken)
    {
        var query = await accessPolicyEvaluator.ApplyAsync(db.StoreBanners.AsNoTracking(), "store_banners", "read", cancellationToken);
        return await query
            .OrderBy(b => b.DisplayOrder)
            .Select(b => MapBanner(b))
            .ToListAsync(cancellationToken);
    }

    public async Task<StoreBannerDto> CreateBannerAsync(
        string title,
        string imageUrl,
        string? linkUrl,
        string location,
        int? categoryId,
        int displayOrder,
        bool isActive,
        DateTime? startsAt,
        DateTime? endsAt,
        CancellationToken cancellationToken)
    {
        await EnsureCategoryExistsIfProvidedAsync(categoryId, cancellationToken);
        var entity = new StoreBanner
        {
            Title = title.Trim(),
            ImageUrl = imageUrl.Trim(),
            LinkUrl = linkUrl?.Trim(),
            Location = location.Trim(),
            CategoryId = categoryId,
            DisplayOrder = displayOrder,
            IsActive = isActive,
            StartsAt = startsAt,
            EndsAt = endsAt
        };
        await RequireAccessAsync(entity, "store_banners", "create", cancellationToken);
        db.StoreBanners.Add(entity);
        await db.SaveChangesAsync(cancellationToken);
        return MapBanner(entity);
    }

    public async Task<StoreBannerDto> UpdateBannerAsync(
        int id,
        string title,
        string imageUrl,
        string? linkUrl,
        string location,
        int? categoryId,
        int displayOrder,
        bool isActive,
        DateTime? startsAt,
        DateTime? endsAt,
        CancellationToken cancellationToken)
    {
        await EnsureCategoryExistsIfProvidedAsync(categoryId, cancellationToken);
        var entity = await db.StoreBanners.FirstOrDefaultAsync(b => b.Id == id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Store banner '{id}' was not found.");
        await RequireAccessAsync(entity, "store_banners", "update", cancellationToken);
        entity.Title = title.Trim();
        entity.ImageUrl = imageUrl.Trim();
        entity.LinkUrl = linkUrl?.Trim();
        entity.Location = location.Trim();
        entity.CategoryId = categoryId;
        entity.DisplayOrder = displayOrder;
        entity.IsActive = isActive;
        entity.StartsAt = startsAt;
        entity.EndsAt = endsAt;
        await db.SaveChangesAsync(cancellationToken);
        return MapBanner(entity);
    }

    public async Task DeleteBannerAsync(int id, CancellationToken cancellationToken)
    {
        var entity = await db.StoreBanners.FirstOrDefaultAsync(b => b.Id == id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Store banner '{id}' was not found.");
        await RequireAccessAsync(entity, "store_banners", "delete", cancellationToken);
        db.StoreBanners.Remove(entity);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<CouponDto>> ListCouponsAsync(CancellationToken cancellationToken)
    {
        var query = await accessPolicyEvaluator.ApplyAsync(db.Coupons.AsNoTracking(), "store_coupons", "read", cancellationToken);
        return await query
            .OrderBy(c => c.Code)
            .Select(c => MapCoupon(c))
            .ToListAsync(cancellationToken);
    }

    public async Task<CouponDto> CreateCouponAsync(
        string code,
        DiscountType discountType,
        decimal amount,
        decimal? minimumSubtotal,
        decimal? maximumDiscountAmount,
        DateTime? startsAt,
        DateTime? expiresAt,
        bool isActive,
        CancellationToken cancellationToken)
    {
        ValidateCouponShape(discountType, amount, minimumSubtotal, maximumDiscountAmount, startsAt, expiresAt);
        var normalizedCode = NormalizeCouponCode(code);
        if (await db.Coupons.AnyAsync(c => c.Code == normalizedCode, cancellationToken))
            throw new ApplicationConflictException("Coupon code already exists.");

        var entity = new Coupon
        {
            Code = normalizedCode,
            DiscountType = discountType,
            Amount = amount,
            MinimumSubtotal = minimumSubtotal,
            MaximumDiscountAmount = maximumDiscountAmount,
            StartsAt = startsAt,
            ExpiresAt = expiresAt,
            IsActive = isActive
        };
        await RequireAccessAsync(entity, "store_coupons", "create", cancellationToken);
        db.Coupons.Add(entity);
        await db.SaveChangesAsync(cancellationToken);
        return MapCoupon(entity);
    }

    public async Task<CouponDto> UpdateCouponAsync(
        int id,
        string code,
        DiscountType discountType,
        decimal amount,
        decimal? minimumSubtotal,
        decimal? maximumDiscountAmount,
        DateTime? startsAt,
        DateTime? expiresAt,
        bool isActive,
        CancellationToken cancellationToken)
    {
        ValidateCouponShape(discountType, amount, minimumSubtotal, maximumDiscountAmount, startsAt, expiresAt);
        var normalizedCode = NormalizeCouponCode(code);
        if (await db.Coupons.AnyAsync(c => c.Code == normalizedCode && c.Id != id, cancellationToken))
            throw new ApplicationConflictException("Coupon code already exists.");

        var entity = await db.Coupons.FirstOrDefaultAsync(c => c.Id == id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Coupon '{id}' was not found.");
        await RequireAccessAsync(entity, "store_coupons", "update", cancellationToken);
        entity.Code = normalizedCode;
        entity.DiscountType = discountType;
        entity.Amount = amount;
        entity.MinimumSubtotal = minimumSubtotal;
        entity.MaximumDiscountAmount = maximumDiscountAmount;
        entity.StartsAt = startsAt;
        entity.ExpiresAt = expiresAt;
        entity.IsActive = isActive;
        await db.SaveChangesAsync(cancellationToken);
        return MapCoupon(entity);
    }

    public async Task DeleteCouponAsync(int id, CancellationToken cancellationToken)
    {
        var entity = await db.Coupons.FirstOrDefaultAsync(c => c.Id == id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Coupon '{id}' was not found.");

        await RequireAccessAsync(entity, "store_coupons", "delete", cancellationToken);
        var inUse = await db.StoreOrders.AnyAsync(o => o.CouponId == id, cancellationToken);
        if (inUse)
        {
            // Keep historical order coupon links intact; inactive coupons can no longer be applied.
            entity.IsActive = false;
        }
        else
            db.Coupons.Remove(entity);

        await db.SaveChangesAsync(cancellationToken);
    }

    private async Task<Cart> GetOrCreateCartAsync(string labClientId, CancellationToken cancellationToken)
    {
        var cart = await GetCartWithItemsOrDefaultAsync(labClientId, cancellationToken);
        if (cart is not null)
            return cart;

        cart = new Cart { LabClientId = labClientId };
        db.Carts.Add(cart);
        await db.SaveChangesAsync(cancellationToken);
        return cart;
    }

    private async Task<Cart> GetCartWithItemsAsync(string labClientId, CancellationToken cancellationToken) =>
        await GetCartWithItemsOrDefaultAsync(labClientId, cancellationToken)
        ?? throw new ApplicationBadRequestException("Cart is empty.");

    private Task<Cart?> GetCartWithItemsOrDefaultAsync(string labClientId, CancellationToken cancellationToken) =>
        db.Carts
            .Include(c => c.Coupon)
            .Include(c => c.Items)
            .ThenInclude(i => i.Product)
            .ThenInclude(p => p.Category)
            .FirstOrDefaultAsync(c => c.LabClientId == labClientId, cancellationToken);

    private async Task<CartDto> BuildCartDtoAsync(Cart cart, CancellationToken cancellationToken)
    {
        var settings = await GetOrCreateSettingsAsync(cancellationToken);
        await db.Entry(cart).Collection(c => c.Items).Query()
            .Include(i => i.Product)
            .ThenInclude(p => p.Category)
            .LoadAsync(cancellationToken);
        if (cart.CouponId is not null)
            await db.Entry(cart).Reference(c => c.Coupon).LoadAsync(cancellationToken);

        var items = cart.Items.OrderBy(i => i.Id).Select(MapCartItem).ToList();
        var subtotal = items.Sum(i => i.LineTotal);
        var discount = cart.Coupon is null ? 0m : CalculateCouponDiscountIfValid(cart.Coupon, subtotal);
        var deliveryFee = items.Count == 0 ? 0m : settings.DeliveryFee;
        return new CartDto(
            cart.Id,
            items,
            cart.Coupon?.Code,
            subtotal,
            discount,
            deliveryFee,
            Math.Max(0, subtotal - discount + deliveryFee),
            settings.DeliveryDurationText);
    }

    private async Task<StoreSetting> GetOrCreateSettingsAsync(CancellationToken cancellationToken)
    {
        var settings = await db.StoreSettings.FirstOrDefaultAsync(cancellationToken);
        if (settings is not null)
            return settings;

        settings = new StoreSetting
        {
            AnnouncementHeader = string.Empty,
            ServiceTitle = string.Empty,
            ServiceDescription = string.Empty,
            DeliveryFee = 0,
            DeliveryDurationText = string.Empty,
            CashOnDeliveryEnabled = true,
            OnlinePaymentEnabled = false
        };
        db.StoreSettings.Add(settings);
        await db.SaveChangesAsync(cancellationToken);
        return settings;
    }

    private async Task<IReadOnlyList<ProductCategoryDto>> BuildCategoryTreeAsync(
        bool activeOnly,
        CancellationToken cancellationToken)
    {
        var query = db.ProductCategories.AsNoTracking().AsQueryable();
        if (activeOnly)
            query = query.Where(c => c.IsActive);
        else
            query = await accessPolicyEvaluator.ApplyAsync(query, "store_product_categories", "read", cancellationToken);

        var categories = await query.OrderBy(c => c.DisplayOrder).ThenBy(c => c.Id).ToListAsync(cancellationToken);
        var children = categories
            .Where(c => c.ParentCategoryId is not null)
            .GroupBy(c => c.ParentCategoryId!.Value)
            .ToDictionary(g => g.Key, g => g.Select(c => MapCategory(c)).ToList());

        return categories
            .Where(c => c.ParentCategoryId is null)
            .Select(c => MapCategory(c, children.GetValueOrDefault(c.Id) ?? []))
            .ToList();
    }

    private async Task<IReadOnlyList<StoreSliderDto>> ListActiveSlidersAsync(CancellationToken cancellationToken)
    {
        var sliders = await db.StoreSliders
            .AsNoTracking()
            .Where(s => s.IsActive)
            .Include(s => s.Products.OrderBy(sp => sp.DisplayOrder))
            .ThenInclude(sp => sp.Product)
            .ThenInclude(p => p.Category)
            .OrderBy(s => s.DisplayOrder)
            .ToListAsync(cancellationToken);

        return sliders.Select(MapSlider).ToList();
    }

    private IQueryable<StoreBanner> ActiveBanners()
    {
        var now = clock.UtcNow;
        return db.StoreBanners.AsNoTracking()
            .Where(b => b.IsActive &&
                        (b.StartsAt == null || b.StartsAt <= now) &&
                        (b.EndsAt == null || b.EndsAt >= now));
    }

    private async Task<Coupon> GetValidCouponAsync(string code, CancellationToken cancellationToken)
    {
        var normalizedCode = NormalizeCouponCode(code);
        var coupon = await db.Coupons.FirstOrDefaultAsync(c => c.Code == normalizedCode, cancellationToken)
            ?? throw new ApplicationNotFoundException("Coupon was not found.");
        EnsureCouponIsValid(coupon);
        return coupon;
    }

    private async Task RequireAccessAsync<TEntity>(
        TEntity entity,
        string resource,
        string action,
        CancellationToken cancellationToken)
    {
        if (!await accessPolicyEvaluator.CanAccessAsync(entity, resource, action, cancellationToken))
            throw new ApplicationForbiddenException($"You cannot {action} this store resource.");
    }

    private void EnsureCouponIsValid(Coupon coupon)
    {
        var now = clock.UtcNow;
        if (!coupon.IsActive)
            throw new ApplicationBadRequestException("Coupon is inactive.");
        if (coupon.StartsAt is not null && coupon.StartsAt > now)
            throw new ApplicationBadRequestException("Coupon is not active yet.");
        if (coupon.ExpiresAt is not null && coupon.ExpiresAt < now)
            throw new ApplicationBadRequestException("Coupon has expired.");
    }

    private static void ValidateCouponForSubtotal(Coupon coupon, decimal subtotal)
    {
        if (coupon.MinimumSubtotal is not null && subtotal < coupon.MinimumSubtotal.Value)
            throw new ApplicationBadRequestException("Cart subtotal does not meet the coupon minimum.");
    }

    private void EnsureProductCanBeSold(Product product)
    {
        if (!product.IsActive)
            throw new ApplicationBadRequestException($"Product '{product.Id}' is inactive.");
        if (!product.Category.IsActive)
            throw new ApplicationBadRequestException($"Product '{product.Id}' category is inactive.");
    }

    private async Task EnsureCategoryParentAllowedAsync(int? parentCategoryId, int? exceptCategoryId, CancellationToken cancellationToken)
    {
        if (parentCategoryId is null)
            return;
        if (exceptCategoryId == parentCategoryId)
            throw new ApplicationBadRequestException("A category cannot be its own parent.");

        var parent = await db.ProductCategories.FirstOrDefaultAsync(c => c.Id == parentCategoryId.Value, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Parent category '{parentCategoryId}' was not found.");
        if (parent.ParentCategoryId is not null)
            throw new ApplicationBadRequestException("Category nesting cannot exceed two levels.");

        if (exceptCategoryId is not null)
        {
            var hasChildren = await db.ProductCategories.AnyAsync(c => c.ParentCategoryId == exceptCategoryId.Value, cancellationToken);
            if (hasChildren)
                throw new ApplicationBadRequestException("A category with subcategories cannot be moved under another category.");
        }
    }

    private async Task EnsureActiveCategoryExistsAsync(int categoryId, CancellationToken cancellationToken)
    {
        var exists = await db.ProductCategories.AnyAsync(c => c.Id == categoryId && c.IsActive, cancellationToken);
        if (!exists)
            throw new ApplicationBadRequestException($"Active category '{categoryId}' was not found.");
    }

    private async Task EnsureCategoryExistsIfProvidedAsync(int? categoryId, CancellationToken cancellationToken)
    {
        if (categoryId is null)
            return;
        var exists = await db.ProductCategories.AnyAsync(c => c.Id == categoryId.Value, cancellationToken);
        if (!exists)
            throw new ApplicationNotFoundException($"Store category '{categoryId}' was not found.");
    }

    private async Task EnsureProductsExistAsync(IReadOnlyList<int> productIds, CancellationToken cancellationToken)
    {
        if (productIds.Count == 0)
            return;
        var distinctIds = productIds.Distinct().ToList();
        var found = await db.Products.CountAsync(p => distinctIds.Contains(p.Id), cancellationToken);
        if (found != distinctIds.Count)
            throw new ApplicationBadRequestException("One or more products were not found.");
    }

    private static void ApplySliderProducts(StoreSlider slider, IReadOnlyList<int> productIds)
    {
        foreach (var productId in productIds.Distinct().Select((id, index) => new { id, index }))
        {
            slider.Products.Add(new StoreSliderProduct
            {
                ProductId = productId.id,
                DisplayOrder = productId.index
            });
        }
    }

    private static void ValidateProductPrices(decimal price, decimal? discountPrice)
    {
        if (price < 0)
            throw new ApplicationBadRequestException("Product price cannot be negative.");
        if (discountPrice is < 0)
            throw new ApplicationBadRequestException("Product discount price cannot be negative.");
        if (discountPrice is not null && discountPrice > price)
            throw new ApplicationBadRequestException("Product discount price cannot exceed price.");
    }

    private static void ValidateCouponShape(
        DiscountType discountType,
        decimal amount,
        decimal? minimumSubtotal,
        decimal? maximumDiscountAmount,
        DateTime? startsAt,
        DateTime? expiresAt)
    {
        if (amount <= 0)
            throw new ApplicationBadRequestException("Coupon amount must be greater than zero.");
        if (discountType == DiscountType.Percentage && amount > 100)
            throw new ApplicationBadRequestException("Percentage coupon amount cannot exceed 100.");
        if (minimumSubtotal is < 0)
            throw new ApplicationBadRequestException("Minimum subtotal cannot be negative.");
        if (maximumDiscountAmount is < 0)
            throw new ApplicationBadRequestException("Maximum discount amount cannot be negative.");
        if (startsAt is not null && expiresAt is not null && startsAt > expiresAt)
            throw new ApplicationBadRequestException("Coupon start date must be before expiry date.");
    }

    private static decimal EffectivePrice(Product product) => product.DiscountPrice ?? product.Price;

    private static decimal CalculateCouponDiscountIfValid(Coupon coupon, decimal subtotal)
    {
        if (!coupon.IsActive || (coupon.MinimumSubtotal is not null && subtotal < coupon.MinimumSubtotal.Value))
            return 0m;
        return CalculateCouponDiscount(coupon, subtotal);
    }

    private static decimal CalculateCouponDiscount(Coupon coupon, decimal subtotal)
    {
        var discount = coupon.DiscountType == DiscountType.Percentage
            ? subtotal * coupon.Amount / 100m
            : coupon.Amount;
        if (coupon.MaximumDiscountAmount is not null)
            discount = Math.Min(discount, coupon.MaximumDiscountAmount.Value);
        return Math.Min(discount, subtotal);
    }

    private static string NormalizeCouponCode(string code) => code.Trim().ToUpperInvariant();

    private static string CreateOrderNumber() =>
        $"SO-{DateTime.UtcNow:yyyyMMddHHmmssfff}-{Random.Shared.Next(1000, 9999)}";

    private async Task<PagedResult<StoreOrderDto>> PageOrdersAsync(
        IQueryable<StoreOrder> query,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var (normalizedPage, normalizedPageSize) = PaginationDefaults.Normalize(page, pageSize);
        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(o => o.OrderedAt)
            .ApplyPagination(normalizedPage, normalizedPageSize)
            .Select(o => MapOrder(o))
            .ToListAsync(cancellationToken);

        return new PagedResult<StoreOrderDto>
        {
            Items = items,
            Page = normalizedPage,
            PageSize = normalizedPageSize,
            TotalCount = total
        };
    }

    private IQueryable<StoreOrder> OrderDetailsQuery() =>
        db.StoreOrders.AsNoTracking()
            .Include(o => o.Items.OrderBy(i => i.Id));

    private static StoreSettingDto MapSetting(StoreSetting settings) =>
        new(
            settings.Id,
            settings.AnnouncementHeader,
            settings.ServiceTitle,
            settings.ServiceDescription,
            settings.DeliveryFee,
            settings.DeliveryDurationText,
            settings.CashOnDeliveryEnabled,
            settings.OnlinePaymentEnabled);

    private static ProductCategoryDto MapCategory(ProductCategory category, IReadOnlyList<ProductCategoryDto>? subcategories = null) =>
        new(
            category.Id,
            category.NameAr,
            category.NameEn,
            category.Description,
            category.ImageUrl,
            category.ParentCategoryId,
            category.DisplayOrder,
            category.IsActive,
            subcategories ?? []);

    private static ProductCardDto MapProductCard(Product product)
    {
        var saved = product.DiscountPrice is not null ? Math.Max(0, product.Price - product.DiscountPrice.Value) : 0m;
        return new ProductCardDto(
            product.Id,
            product.CategoryId,
            product.NameAr,
            product.NameEn,
            product.ImageUrl,
            product.SaleUnit,
            product.Price,
            product.DiscountPrice,
            saved,
            product.TopBadge);
    }

    private static ProductDetailsDto MapProductDetails(Product product)
    {
        var saved = product.DiscountPrice is not null ? Math.Max(0, product.Price - product.DiscountPrice.Value) : 0m;
        return new ProductDetailsDto(
            product.Id,
            product.CategoryId,
            product.Category.NameAr,
            product.Category.NameEn,
            product.NameAr,
            product.NameEn,
            product.Description,
            product.ImageUrl,
            product.SaleUnit,
            product.Price,
            product.DiscountPrice,
            saved,
            product.TopBadge,
            product.IsRecommended,
            product.IsBestSeller,
            product.IsActive);
    }

    private static StoreBannerDto MapBanner(StoreBanner banner) =>
        new(
            banner.Id,
            banner.Title,
            banner.ImageUrl,
            banner.LinkUrl,
            banner.Location,
            banner.CategoryId,
            banner.DisplayOrder);

    private static StoreSliderDto MapSlider(StoreSlider slider) =>
        new(
            slider.Id,
            slider.Title,
            slider.Type,
            slider.DisplayOrder,
            slider.Products
                .Where(sp => sp.Product.IsActive && (sp.Product.Category is null || sp.Product.Category.IsActive))
                .OrderBy(sp => sp.DisplayOrder)
                .Select(sp => MapProductCard(sp.Product))
                .ToList());

    private static CartItemDto MapCartItem(CartItem item)
    {
        var effective = EffectivePrice(item.Product);
        return new CartItemDto(
            item.Id,
            item.ProductId,
            item.Product.NameAr,
            item.Product.NameEn,
            item.Product.ImageUrl,
            item.Product.SaleUnit,
            item.Product.Price,
            item.Product.DiscountPrice,
            effective,
            item.Quantity,
            effective * item.Quantity);
    }

    private static StoreOrderDto MapOrder(StoreOrder order) =>
        new(
            order.Id,
            order.OrderNumber,
            order.Status,
            order.PaymentMethod,
            order.PaymentStatus,
            order.Subtotal,
            order.DiscountAmount,
            order.DeliveryFee,
            order.Total,
            order.CouponCodeSnapshot,
            order.DeliveryDurationSnapshot,
            order.OrderedAt);

    private static StoreOrderDetailsDto MapOrderDetails(StoreOrder order) =>
        new(
            order.Id,
            order.OrderNumber,
            order.LabClientId,
            order.Status,
            order.PaymentMethod,
            order.PaymentStatus,
            order.Subtotal,
            order.DiscountAmount,
            order.DeliveryFee,
            order.Total,
            order.CouponCodeSnapshot,
            order.DeliveryDurationSnapshot,
            order.Notes,
            order.OrderedAt,
            order.Items.Select(MapOrderItem).ToList());

    private static StoreOrderItemDto MapOrderItem(StoreOrderItem item) =>
        new(
            item.Id,
            item.ProductId,
            item.ProductNameSnapshot,
            item.SaleUnitSnapshot,
            item.ImageSnapshot,
            item.UnitPriceSnapshot,
            item.DiscountPriceSnapshot,
            item.Quantity,
            item.LineTotal);

    private static CouponDto MapCoupon(Coupon coupon) =>
        new(
            coupon.Id,
            coupon.Code,
            coupon.DiscountType,
            coupon.Amount,
            coupon.MinimumSubtotal,
            coupon.MaximumDiscountAmount,
            coupon.StartsAt,
            coupon.ExpiresAt,
            coupon.IsActive);
}
