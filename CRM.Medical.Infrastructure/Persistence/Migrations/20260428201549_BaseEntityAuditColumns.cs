using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Medical.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class BaseEntityAuditColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "user_permissions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "user_permissions",
                type: "character varying(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "user_permissions",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "templates",
                type: "character varying(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "subscription_packages",
                type: "character varying(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "slide_cards",
                type: "character varying(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "slide_cards",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "refresh_tokens",
                type: "character varying(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "refresh_tokens",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "permissions",
                type: "character varying(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "permissions",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "complaints",
                type: "character varying(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "banners",
                type: "character varying(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "banners",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "user_permissions");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "user_permissions");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "user_permissions");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "templates");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "subscription_packages");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "slide_cards");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "slide_cards");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "refresh_tokens");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "refresh_tokens");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "permissions");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "permissions");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "complaints");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "banners");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "banners");
        }
    }
}
