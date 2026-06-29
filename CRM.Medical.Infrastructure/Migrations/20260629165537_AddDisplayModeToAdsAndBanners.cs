using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Medical.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDisplayModeToAdsAndBanners : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DisplayMode",
                table: "banners",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.Sql("""
                UPDATE "banners"
                SET "DisplayMode" = CASE lower(trim("Type"))
                    WHEN 'full' THEN 1
                    WHEN 'large' THEN 2
                    WHEN 'larg' THEN 2
                    WHEN 'small' THEN 3
                    WHEN 'xsmall' THEN 4
                    ELSE 1
                END
                WHERE "Type" IS NOT NULL AND trim("Type") <> '';
                """);

            migrationBuilder.DropColumn(
                name: "Type",
                table: "banners");

            migrationBuilder.AddColumn<int>(
                name: "DisplayMode",
                table: "ads",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateIndex(
                name: "IX_banners_DisplayMode",
                table: "banners",
                column: "DisplayMode");

            migrationBuilder.CreateIndex(
                name: "IX_ads_DisplayMode",
                table: "ads",
                column: "DisplayMode");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_banners_DisplayMode",
                table: "banners");

            migrationBuilder.DropIndex(
                name: "IX_ads_DisplayMode",
                table: "ads");

            migrationBuilder.DropColumn(
                name: "DisplayMode",
                table: "banners");

            migrationBuilder.DropColumn(
                name: "DisplayMode",
                table: "ads");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "banners",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "full");
        }
    }
}
