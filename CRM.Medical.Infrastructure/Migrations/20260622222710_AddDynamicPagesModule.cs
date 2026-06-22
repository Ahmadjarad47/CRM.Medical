using System;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CRM.Medical.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDynamicPagesModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "pages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TemplateKey = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    ParentId = table.Column<int>(type: "integer", nullable: true),
                    sort_order = table.Column<int>(type: "integer", nullable: false),
                    PublishStatus = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false, defaultValue: "Draft"),
                    PublishScheduledAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PublishedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsVisibleInNav = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedByUserId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    UpdatedByUserId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_pages_pages_ParentId",
                        column: x => x.ParentId,
                        principalTable: "pages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "content_blocks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PageId = table.Column<int>(type: "integer", nullable: false),
                    BlockType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false),
                    CustomCssClass = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CustomStyles = table.Column<JsonDocument>(type: "jsonb", nullable: true),
                    Animation = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    VisibilityRules = table.Column<JsonDocument>(type: "jsonb", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_content_blocks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_content_blocks_pages_PageId",
                        column: x => x.PageId,
                        principalTable: "pages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "content_versions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PageId = table.Column<int>(type: "integer", nullable: false),
                    SnapshotData = table.Column<JsonDocument>(type: "jsonb", nullable: false),
                    VersionNumber = table.Column<int>(type: "integer", nullable: false),
                    ChangeNotes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    CreatedByUserId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_content_versions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_content_versions_pages_PageId",
                        column: x => x.PageId,
                        principalTable: "pages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "page_translations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PageId = table.Column<int>(type: "integer", nullable: false),
                    Language = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Title = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Slug = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    MetaTitle = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    MetaDescription = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    MetaKeywords = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    OpenGraphImageUrl = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    CanonicalUrl = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    BreadcrumbTitle = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_page_translations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_page_translations_pages_PageId",
                        column: x => x.PageId,
                        principalTable: "pages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "block_localizations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ContentBlockId = table.Column<int>(type: "integer", nullable: false),
                    Language = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Heading = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    Subheading = table.Column<string>(type: "character varying(600)", maxLength: 600, nullable: true),
                    Description = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    ContentData = table.Column<JsonDocument>(type: "jsonb", nullable: true),
                    MediaUrl = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    MediaAltText = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ButtonText = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    ButtonLink = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    ButtonStyle = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_block_localizations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_block_localizations_content_blocks_ContentBlockId",
                        column: x => x.ContentBlockId,
                        principalTable: "content_blocks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_block_localizations_ContentBlockId",
                table: "block_localizations",
                column: "ContentBlockId");

            migrationBuilder.CreateIndex(
                name: "IX_block_localizations_ContentBlockId_Language",
                table: "block_localizations",
                columns: new[] { "ContentBlockId", "Language" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_block_localizations_Language",
                table: "block_localizations",
                column: "Language");

            migrationBuilder.CreateIndex(
                name: "IX_content_blocks_IsActive",
                table: "content_blocks",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_content_blocks_PageId",
                table: "content_blocks",
                column: "PageId");

            migrationBuilder.CreateIndex(
                name: "IX_content_blocks_PageId_sort_order",
                table: "content_blocks",
                columns: new[] { "PageId", "sort_order" });

            migrationBuilder.CreateIndex(
                name: "IX_content_versions_CreatedAt",
                table: "content_versions",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_content_versions_PageId",
                table: "content_versions",
                column: "PageId");

            migrationBuilder.CreateIndex(
                name: "IX_content_versions_PageId_VersionNumber",
                table: "content_versions",
                columns: new[] { "PageId", "VersionNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_page_translations_Language_Slug",
                table: "page_translations",
                columns: new[] { "Language", "Slug" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_page_translations_PageId_Language",
                table: "page_translations",
                columns: new[] { "PageId", "Language" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_page_translations_Slug",
                table: "page_translations",
                column: "Slug");

            migrationBuilder.CreateIndex(
                name: "IX_pages_CreatedAt",
                table: "pages",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_pages_IsActive",
                table: "pages",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_pages_IsVisibleInNav",
                table: "pages",
                column: "IsVisibleInNav");

            migrationBuilder.CreateIndex(
                name: "IX_pages_ParentId",
                table: "pages",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_pages_PublishStatus",
                table: "pages",
                column: "PublishStatus");

            migrationBuilder.CreateIndex(
                name: "IX_pages_TemplateKey",
                table: "pages",
                column: "TemplateKey",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "block_localizations");

            migrationBuilder.DropTable(
                name: "content_versions");

            migrationBuilder.DropTable(
                name: "page_translations");

            migrationBuilder.DropTable(
                name: "content_blocks");

            migrationBuilder.DropTable(
                name: "pages");
        }
    }
}
