using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CRM.Medical.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSlideCards : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "slide_cards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    ImageUrl = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                    Price = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Discount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Badge = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    DetailPageLink = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_slide_cards", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_slide_cards_CreatedAt",
                table: "slide_cards",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_slide_cards_DisplayOrder",
                table: "slide_cards",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_slide_cards_ExpiryDate",
                table: "slide_cards",
                column: "ExpiryDate");

            migrationBuilder.CreateIndex(
                name: "IX_slide_cards_IsActive",
                table: "slide_cards",
                column: "IsActive");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "slide_cards");
        }
    }
}
