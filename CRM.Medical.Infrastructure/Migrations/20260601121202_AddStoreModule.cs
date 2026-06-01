using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CRM.Medical.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddStoreModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "store_coupons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    DiscountType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    MinimumSubtotal = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    MaximumDiscountAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    StartsAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedByUserId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_store_coupons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "store_product_categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NameAr = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    NameEn = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    ImageUrl = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    ParentCategoryId = table.Column<int>(type: "integer", nullable: true),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedByUserId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_store_product_categories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_store_product_categories_store_product_categories_ParentCat~",
                        column: x => x.ParentCategoryId,
                        principalTable: "store_product_categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "store_settings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AnnouncementHeader = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ServiceTitle = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ServiceDescription = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    DeliveryFee = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    DeliveryDurationText = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    CashOnDeliveryEnabled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    OnlinePaymentEnabled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedByUserId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_store_settings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "store_sliders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedByUserId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_store_sliders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "store_carts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LabClientId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    CouponId = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedByUserId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_store_carts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_store_carts_store_coupons_CouponId",
                        column: x => x.CouponId,
                        principalTable: "store_coupons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_store_carts_users_LabClientId",
                        column: x => x.LabClientId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "store_orders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrderNumber = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    LabClientId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PaymentMethod = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PaymentStatus = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Subtotal = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    DeliveryFee = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Total = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    CouponId = table.Column<int>(type: "integer", nullable: true),
                    CouponCodeSnapshot = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    DeliveryDurationSnapshot = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Notes = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    OrderedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedByUserId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_store_orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_store_orders_store_coupons_CouponId",
                        column: x => x.CouponId,
                        principalTable: "store_coupons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_store_orders_users_LabClientId",
                        column: x => x.LabClientId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "store_banners",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    ImageUrl = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                    LinkUrl = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    Location = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CategoryId = table.Column<int>(type: "integer", nullable: true),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    StartsAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EndsAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedByUserId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_store_banners", x => x.Id);
                    table.ForeignKey(
                        name: "FK_store_banners_store_product_categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "store_product_categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "store_products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CategoryId = table.Column<int>(type: "integer", nullable: false),
                    NameAr = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    NameEn = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    ImageUrl = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                    SaleUnit = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Price = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    DiscountPrice = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    TopBadge = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    IsRecommended = table.Column<bool>(type: "boolean", nullable: false),
                    IsBestSeller = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedByUserId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_store_products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_store_products_store_product_categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "store_product_categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "store_payment_transactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StoreOrderId = table.Column<int>(type: "integer", nullable: false),
                    PaymentMethod = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Provider = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ProviderTransactionId = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    CreatedOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedByUserId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_store_payment_transactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_store_payment_transactions_store_orders_StoreOrderId",
                        column: x => x.StoreOrderId,
                        principalTable: "store_orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "store_cart_items",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CartId = table.Column<int>(type: "integer", nullable: false),
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedByUserId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_store_cart_items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_store_cart_items_store_carts_CartId",
                        column: x => x.CartId,
                        principalTable: "store_carts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_store_cart_items_store_products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "store_products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "store_order_items",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StoreOrderId = table.Column<int>(type: "integer", nullable: false),
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    ProductNameSnapshot = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    SaleUnitSnapshot = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ImageSnapshot = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                    UnitPriceSnapshot = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    DiscountPriceSnapshot = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    LineTotal = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedByUserId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_store_order_items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_store_order_items_store_orders_StoreOrderId",
                        column: x => x.StoreOrderId,
                        principalTable: "store_orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_store_order_items_store_products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "store_products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "store_slider_products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StoreSliderId = table.Column<int>(type: "integer", nullable: false),
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_store_slider_products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_store_slider_products_store_products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "store_products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_store_slider_products_store_sliders_StoreSliderId",
                        column: x => x.StoreSliderId,
                        principalTable: "store_sliders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_store_banners_CategoryId",
                table: "store_banners",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_store_banners_DisplayOrder",
                table: "store_banners",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_store_banners_IsActive",
                table: "store_banners",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_store_banners_Location",
                table: "store_banners",
                column: "Location");

            migrationBuilder.CreateIndex(
                name: "IX_store_cart_items_CartId_ProductId",
                table: "store_cart_items",
                columns: new[] { "CartId", "ProductId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_store_cart_items_ProductId",
                table: "store_cart_items",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_store_carts_CouponId",
                table: "store_carts",
                column: "CouponId");

            migrationBuilder.CreateIndex(
                name: "IX_store_carts_LabClientId",
                table: "store_carts",
                column: "LabClientId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_store_coupons_Code",
                table: "store_coupons",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_store_coupons_ExpiresAt",
                table: "store_coupons",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_store_coupons_IsActive",
                table: "store_coupons",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_store_order_items_ProductId",
                table: "store_order_items",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_store_order_items_StoreOrderId",
                table: "store_order_items",
                column: "StoreOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_store_orders_CouponId",
                table: "store_orders",
                column: "CouponId");

            migrationBuilder.CreateIndex(
                name: "IX_store_orders_LabClientId",
                table: "store_orders",
                column: "LabClientId");

            migrationBuilder.CreateIndex(
                name: "IX_store_orders_OrderedAt",
                table: "store_orders",
                column: "OrderedAt");

            migrationBuilder.CreateIndex(
                name: "IX_store_orders_OrderNumber",
                table: "store_orders",
                column: "OrderNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_store_orders_Status",
                table: "store_orders",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_store_payment_transactions_ProviderTransactionId",
                table: "store_payment_transactions",
                column: "ProviderTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_store_payment_transactions_StoreOrderId",
                table: "store_payment_transactions",
                column: "StoreOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_store_product_categories_DisplayOrder",
                table: "store_product_categories",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_store_product_categories_IsActive",
                table: "store_product_categories",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_store_product_categories_ParentCategoryId",
                table: "store_product_categories",
                column: "ParentCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_store_products_CategoryId",
                table: "store_products",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_store_products_CreatedAt",
                table: "store_products",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_store_products_DisplayOrder",
                table: "store_products",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_store_products_IsActive",
                table: "store_products",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_store_products_IsBestSeller",
                table: "store_products",
                column: "IsBestSeller");

            migrationBuilder.CreateIndex(
                name: "IX_store_products_IsRecommended",
                table: "store_products",
                column: "IsRecommended");

            migrationBuilder.CreateIndex(
                name: "IX_store_slider_products_DisplayOrder",
                table: "store_slider_products",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_store_slider_products_ProductId",
                table: "store_slider_products",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_store_slider_products_StoreSliderId_ProductId",
                table: "store_slider_products",
                columns: new[] { "StoreSliderId", "ProductId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_store_sliders_DisplayOrder",
                table: "store_sliders",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_store_sliders_IsActive",
                table: "store_sliders",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_store_sliders_Type",
                table: "store_sliders",
                column: "Type");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "store_banners");

            migrationBuilder.DropTable(
                name: "store_cart_items");

            migrationBuilder.DropTable(
                name: "store_order_items");

            migrationBuilder.DropTable(
                name: "store_payment_transactions");

            migrationBuilder.DropTable(
                name: "store_settings");

            migrationBuilder.DropTable(
                name: "store_slider_products");

            migrationBuilder.DropTable(
                name: "store_carts");

            migrationBuilder.DropTable(
                name: "store_orders");

            migrationBuilder.DropTable(
                name: "store_products");

            migrationBuilder.DropTable(
                name: "store_sliders");

            migrationBuilder.DropTable(
                name: "store_coupons");

            migrationBuilder.DropTable(
                name: "store_product_categories");
        }
    }
}
