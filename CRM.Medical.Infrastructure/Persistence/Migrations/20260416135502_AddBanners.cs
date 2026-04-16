using System;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CRM.Medical.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddBanners : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "banners",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    MediaUrl = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                    InternalLink = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                    ExternalLink = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                    TargetType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Location = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    VisibilityRules = table.Column<JsonDocument>(type: "jsonb", nullable: true),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ViewsCount = table.Column<int>(type: "integer", nullable: false),
                    ClicksCount = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_banners", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_banners_CreatedAt",
                table: "banners",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_banners_DisplayOrder",
                table: "banners",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_banners_EndDate",
                table: "banners",
                column: "EndDate");

            migrationBuilder.CreateIndex(
                name: "IX_banners_IsActive",
                table: "banners",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_banners_Location",
                table: "banners",
                column: "Location");

            migrationBuilder.CreateIndex(
                name: "IX_banners_StartDate",
                table: "banners",
                column: "StartDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "banners");
        }
    }
}
