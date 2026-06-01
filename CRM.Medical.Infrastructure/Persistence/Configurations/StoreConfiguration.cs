using CRM.Medical.Domain.Entities.Store;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Medical.Infrastructure.Persistence.Configurations;

public sealed class ProductCategoryConfiguration : IEntityTypeConfiguration<ProductCategory>
{
    public void Configure(EntityTypeBuilder<ProductCategory> builder)
    {
        builder.ToTable("store_product_categories");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).UseIdentityByDefaultColumn();
        builder.Property(e => e.NameAr).IsRequired().HasMaxLength(500);
        builder.Property(e => e.NameEn).IsRequired().HasMaxLength(500);
        builder.Property(e => e.Description).HasMaxLength(4000);
        builder.Property(e => e.ImageUrl).HasMaxLength(2048);
        builder.Property(e => e.IsActive).IsRequired().HasDefaultValue(true);
        builder.ConfigureAuditColumns();

        builder.HasOne(e => e.ParentCategory)
            .WithMany(e => e.Subcategories)
            .HasForeignKey(e => e.ParentCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => e.ParentCategoryId);
        builder.HasIndex(e => e.IsActive);
        builder.HasIndex(e => e.DisplayOrder);
    }
}

public sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("store_products");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).UseIdentityByDefaultColumn();
        builder.Property(e => e.NameAr).IsRequired().HasMaxLength(500);
        builder.Property(e => e.NameEn).IsRequired().HasMaxLength(500);
        builder.Property(e => e.Description).HasMaxLength(4000);
        builder.Property(e => e.ImageUrl).IsRequired().HasMaxLength(2048);
        builder.Property(e => e.SaleUnit).IsRequired().HasMaxLength(100);
        builder.Property(e => e.Price).IsRequired().HasPrecision(18, 2);
        builder.Property(e => e.DiscountPrice).HasPrecision(18, 2);
        builder.Property(e => e.TopBadge).HasMaxLength(100);
        builder.Property(e => e.IsActive).IsRequired().HasDefaultValue(true);
        builder.ConfigureAuditColumns();

        builder.HasOne(e => e.Category)
            .WithMany(e => e.Products)
            .HasForeignKey(e => e.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => e.CategoryId);
        builder.HasIndex(e => e.IsActive);
        builder.HasIndex(e => e.IsRecommended);
        builder.HasIndex(e => e.IsBestSeller);
        builder.HasIndex(e => e.DisplayOrder);
        builder.HasIndex(e => e.CreatedAt);
    }
}

public sealed class StoreSettingConfiguration : IEntityTypeConfiguration<StoreSetting>
{
    public void Configure(EntityTypeBuilder<StoreSetting> builder)
    {
        builder.ToTable("store_settings");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).UseIdentityByDefaultColumn();
        builder.Property(e => e.AnnouncementHeader).IsRequired().HasMaxLength(500);
        builder.Property(e => e.ServiceTitle).IsRequired().HasMaxLength(500);
        builder.Property(e => e.ServiceDescription).IsRequired().HasMaxLength(4000);
        builder.Property(e => e.DeliveryFee).IsRequired().HasPrecision(18, 2);
        builder.Property(e => e.DeliveryDurationText).IsRequired().HasMaxLength(500);
        builder.Property(e => e.CashOnDeliveryEnabled).IsRequired().HasDefaultValue(true);
        builder.Property(e => e.OnlinePaymentEnabled).IsRequired().HasDefaultValue(false);
        builder.ConfigureAuditColumns();
    }
}

public sealed class StoreBannerConfiguration : IEntityTypeConfiguration<StoreBanner>
{
    public void Configure(EntityTypeBuilder<StoreBanner> builder)
    {
        builder.ToTable("store_banners");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).UseIdentityByDefaultColumn();
        builder.Property(e => e.Title).IsRequired().HasMaxLength(300);
        builder.Property(e => e.ImageUrl).IsRequired().HasMaxLength(2048);
        builder.Property(e => e.LinkUrl).HasMaxLength(2048);
        builder.Property(e => e.Location).IsRequired().HasMaxLength(100);
        builder.Property(e => e.IsActive).IsRequired().HasDefaultValue(true);
        builder.ConfigureAuditColumns();

        builder.HasOne(e => e.Category)
            .WithMany(e => e.Banners)
            .HasForeignKey(e => e.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => e.Location);
        builder.HasIndex(e => e.CategoryId);
        builder.HasIndex(e => e.IsActive);
        builder.HasIndex(e => e.DisplayOrder);
    }
}

public sealed class StoreSliderConfiguration : IEntityTypeConfiguration<StoreSlider>
{
    public void Configure(EntityTypeBuilder<StoreSlider> builder)
    {
        builder.ToTable("store_sliders");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).UseIdentityByDefaultColumn();
        builder.Property(e => e.Title).IsRequired().HasMaxLength(300);
        builder.Property(e => e.Type).HasConversion<string>().HasMaxLength(50).IsRequired();
        builder.Property(e => e.IsActive).IsRequired().HasDefaultValue(true);
        builder.ConfigureAuditColumns();

        builder.HasIndex(e => e.Type);
        builder.HasIndex(e => e.IsActive);
        builder.HasIndex(e => e.DisplayOrder);
    }
}

public sealed class StoreSliderProductConfiguration : IEntityTypeConfiguration<StoreSliderProduct>
{
    public void Configure(EntityTypeBuilder<StoreSliderProduct> builder)
    {
        builder.ToTable("store_slider_products");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).UseIdentityByDefaultColumn();

        builder.HasOne(e => e.StoreSlider)
            .WithMany(e => e.Products)
            .HasForeignKey(e => e.StoreSliderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Product)
            .WithMany(e => e.SliderProducts)
            .HasForeignKey(e => e.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => new { e.StoreSliderId, e.ProductId }).IsUnique();
        builder.HasIndex(e => e.DisplayOrder);
    }
}

public sealed class CartConfiguration : IEntityTypeConfiguration<Cart>
{
    public void Configure(EntityTypeBuilder<Cart> builder)
    {
        builder.ToTable("store_carts");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).UseIdentityByDefaultColumn();
        builder.Property(e => e.LabClientId).IsRequired().HasMaxLength(450);
        builder.ConfigureAuditColumns();

        builder.HasOne(e => e.LabClient)
            .WithMany()
            .HasForeignKey(e => e.LabClientId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Coupon)
            .WithMany(e => e.Carts)
            .HasForeignKey(e => e.CouponId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(e => e.LabClientId).IsUnique();
    }
}

public sealed class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        builder.ToTable("store_cart_items");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).UseIdentityByDefaultColumn();
        builder.Property(e => e.Quantity).IsRequired();
        builder.ConfigureAuditColumns();

        builder.HasOne(e => e.Cart)
            .WithMany(e => e.Items)
            .HasForeignKey(e => e.CartId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Product)
            .WithMany(e => e.CartItems)
            .HasForeignKey(e => e.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => new { e.CartId, e.ProductId }).IsUnique();
    }
}

public sealed class CouponConfiguration : IEntityTypeConfiguration<Coupon>
{
    public void Configure(EntityTypeBuilder<Coupon> builder)
    {
        builder.ToTable("store_coupons");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).UseIdentityByDefaultColumn();
        builder.Property(e => e.Code).IsRequired().HasMaxLength(64);
        builder.Property(e => e.DiscountType).HasConversion<string>().HasMaxLength(50).IsRequired();
        builder.Property(e => e.Amount).IsRequired().HasPrecision(18, 2);
        builder.Property(e => e.MinimumSubtotal).HasPrecision(18, 2);
        builder.Property(e => e.MaximumDiscountAmount).HasPrecision(18, 2);
        builder.Property(e => e.IsActive).IsRequired().HasDefaultValue(true);
        builder.ConfigureAuditColumns();

        builder.HasIndex(e => e.Code).IsUnique();
        builder.HasIndex(e => e.IsActive);
        builder.HasIndex(e => e.ExpiresAt);
    }
}

public sealed class StoreOrderConfiguration : IEntityTypeConfiguration<StoreOrder>
{
    public void Configure(EntityTypeBuilder<StoreOrder> builder)
    {
        builder.ToTable("store_orders");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).UseIdentityByDefaultColumn();
        builder.Property(e => e.OrderNumber).IsRequired().HasMaxLength(64);
        builder.Property(e => e.LabClientId).IsRequired().HasMaxLength(450);
        builder.Property(e => e.Status).HasConversion<string>().HasMaxLength(50).IsRequired();
        builder.Property(e => e.PaymentMethod).HasConversion<string>().HasMaxLength(50).IsRequired();
        builder.Property(e => e.PaymentStatus).HasConversion<string>().HasMaxLength(50).IsRequired();
        builder.Property(e => e.Subtotal).IsRequired().HasPrecision(18, 2);
        builder.Property(e => e.DiscountAmount).IsRequired().HasPrecision(18, 2);
        builder.Property(e => e.DeliveryFee).IsRequired().HasPrecision(18, 2);
        builder.Property(e => e.Total).IsRequired().HasPrecision(18, 2);
        builder.Property(e => e.CouponCodeSnapshot).HasMaxLength(64);
        builder.Property(e => e.DeliveryDurationSnapshot).IsRequired().HasMaxLength(500);
        builder.Property(e => e.Notes).HasMaxLength(4000);
        builder.ConfigureAuditColumns();

        builder.HasOne(e => e.LabClient)
            .WithMany()
            .HasForeignKey(e => e.LabClientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Coupon)
            .WithMany(e => e.Orders)
            .HasForeignKey(e => e.CouponId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(e => e.OrderNumber).IsUnique();
        builder.HasIndex(e => e.LabClientId);
        builder.HasIndex(e => e.Status);
        builder.HasIndex(e => e.OrderedAt);
    }
}

public sealed class StoreOrderItemConfiguration : IEntityTypeConfiguration<StoreOrderItem>
{
    public void Configure(EntityTypeBuilder<StoreOrderItem> builder)
    {
        builder.ToTable("store_order_items");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).UseIdentityByDefaultColumn();
        builder.Property(e => e.ProductNameSnapshot).IsRequired().HasMaxLength(500);
        builder.Property(e => e.SaleUnitSnapshot).IsRequired().HasMaxLength(100);
        builder.Property(e => e.ImageSnapshot).IsRequired().HasMaxLength(2048);
        builder.Property(e => e.UnitPriceSnapshot).IsRequired().HasPrecision(18, 2);
        builder.Property(e => e.DiscountPriceSnapshot).HasPrecision(18, 2);
        builder.Property(e => e.LineTotal).IsRequired().HasPrecision(18, 2);
        builder.ConfigureAuditColumns();

        builder.HasOne(e => e.StoreOrder)
            .WithMany(e => e.Items)
            .HasForeignKey(e => e.StoreOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Product)
            .WithMany()
            .HasForeignKey(e => e.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => e.StoreOrderId);
        builder.HasIndex(e => e.ProductId);
    }
}

public sealed class PaymentTransactionConfiguration : IEntityTypeConfiguration<PaymentTransaction>
{
    public void Configure(EntityTypeBuilder<PaymentTransaction> builder)
    {
        builder.ToTable("store_payment_transactions");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).UseIdentityByDefaultColumn();
        builder.Property(e => e.PaymentMethod).HasConversion<string>().HasMaxLength(50).IsRequired();
        builder.Property(e => e.Status).HasConversion<string>().HasMaxLength(50).IsRequired();
        builder.Property(e => e.Amount).IsRequired().HasPrecision(18, 2);
        builder.Property(e => e.Provider).HasMaxLength(100);
        builder.Property(e => e.ProviderTransactionId).HasMaxLength(256);
        builder.ConfigureAuditColumns();

        builder.HasOne(e => e.StoreOrder)
            .WithMany(e => e.PaymentTransactions)
            .HasForeignKey(e => e.StoreOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => e.StoreOrderId);
        builder.HasIndex(e => e.ProviderTransactionId);
    }
}
