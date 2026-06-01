using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CRM.Medical.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAccountingModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "accounting_page_settings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AnnouncementTextAr = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    AnnouncementTextEn = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    TitleAr = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    TitleEn = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    DescriptionAr = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    DescriptionEn = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedByUserId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_accounting_page_settings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "lab_account_payments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LabClientId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    PaidAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PaymentMethod = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ReferenceNumber = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Notes = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedByUserId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lab_account_payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_lab_account_payments_users_LabClientId",
                        column: x => x.LabClientId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "lab_account_statement_files",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LabClientId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    PeriodFrom = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PeriodTo = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FileUrl = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                    OriginalFileName = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Notes = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedByUserId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lab_account_statement_files", x => x.Id);
                    table.ForeignKey(
                        name: "FK_lab_account_statement_files_users_LabClientId",
                        column: x => x.LabClientId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_accounting_page_settings_IsActive",
                table: "accounting_page_settings",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_lab_account_payments_CreatedAt",
                table: "lab_account_payments",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_lab_account_payments_LabClientId",
                table: "lab_account_payments",
                column: "LabClientId");

            migrationBuilder.CreateIndex(
                name: "IX_lab_account_payments_LabClientId_PaidAt",
                table: "lab_account_payments",
                columns: new[] { "LabClientId", "PaidAt" });

            migrationBuilder.CreateIndex(
                name: "IX_lab_account_payments_PaidAt",
                table: "lab_account_payments",
                column: "PaidAt");

            migrationBuilder.CreateIndex(
                name: "IX_lab_account_statement_files_CreatedAt",
                table: "lab_account_statement_files",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_lab_account_statement_files_LabClientId",
                table: "lab_account_statement_files",
                column: "LabClientId");

            migrationBuilder.CreateIndex(
                name: "IX_lab_account_statement_files_LabClientId_PeriodFrom_PeriodTo",
                table: "lab_account_statement_files",
                columns: new[] { "LabClientId", "PeriodFrom", "PeriodTo" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "accounting_page_settings");

            migrationBuilder.DropTable(
                name: "lab_account_payments");

            migrationBuilder.DropTable(
                name: "lab_account_statement_files");
        }
    }
}
