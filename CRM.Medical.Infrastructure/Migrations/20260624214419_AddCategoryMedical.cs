using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CRM.Medical.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoryMedical : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "category_medical",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NameAr = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    NameEn = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedByUserId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_category_medical", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_category_medical_DisplayOrder",
                table: "category_medical",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_category_medical_IsActive",
                table: "category_medical",
                column: "IsActive");

            migrationBuilder.Sql("""
                INSERT INTO category_medical ("NameAr", "NameEn", "DisplayOrder", "IsActive", "CreatedAt")
                SELECT DISTINCT
                    NULLIF(BTRIM("Category"), ''),
                    NULLIF(BTRIM("Category"), ''),
                    0,
                    TRUE,
                    NOW() AT TIME ZONE 'UTC'
                FROM medical_tests
                WHERE "Category" IS NOT NULL AND BTRIM("Category") <> '';
                """);

            migrationBuilder.Sql("""
                INSERT INTO category_medical ("NameAr", "NameEn", "DisplayOrder", "IsActive", "CreatedAt")
                SELECT 'غير مصنف', 'Uncategorized', 0, TRUE, NOW() AT TIME ZONE 'UTC'
                WHERE NOT EXISTS (SELECT 1 FROM category_medical);
                """);

            migrationBuilder.AddColumn<int>(
                name: "CategoryMedicalId",
                table: "medical_tests",
                type: "integer",
                nullable: true);

            migrationBuilder.Sql("""
                UPDATE medical_tests mt
                SET "CategoryMedicalId" = cm."Id"
                FROM category_medical cm
                WHERE BTRIM(mt."Category") <> ''
                  AND cm."NameEn" = BTRIM(mt."Category");
                """);

            migrationBuilder.Sql("""
                UPDATE medical_tests
                SET "CategoryMedicalId" = (SELECT "Id" FROM category_medical ORDER BY "Id" LIMIT 1)
                WHERE "CategoryMedicalId" IS NULL;
                """);

            migrationBuilder.DropColumn(
                name: "Category",
                table: "medical_tests");

            migrationBuilder.AlterColumn<int>(
                name: "CategoryMedicalId",
                table: "medical_tests",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_medical_tests_CategoryMedicalId",
                table: "medical_tests",
                column: "CategoryMedicalId");

            migrationBuilder.AddForeignKey(
                name: "FK_medical_tests_category_medical_CategoryMedicalId",
                table: "medical_tests",
                column: "CategoryMedicalId",
                principalTable: "category_medical",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_medical_tests_category_medical_CategoryMedicalId",
                table: "medical_tests");

            migrationBuilder.DropIndex(
                name: "IX_medical_tests_CategoryMedicalId",
                table: "medical_tests");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "medical_tests",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql("""
                UPDATE medical_tests mt
                SET "Category" = cm."NameEn"
                FROM category_medical cm
                WHERE mt."CategoryMedicalId" = cm."Id";
                """);

            migrationBuilder.DropColumn(
                name: "CategoryMedicalId",
                table: "medical_tests");

            migrationBuilder.DropTable(
                name: "category_medical");
        }
    }
}
