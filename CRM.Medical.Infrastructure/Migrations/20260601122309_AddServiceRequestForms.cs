using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CRM.Medical.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddServiceRequestForms : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "client_join_requests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ManagerName = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    LabName = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    MobileNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Address = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    AdditionalInfo = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    Status = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Notes = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedByUserId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_client_join_requests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "contract_service_requests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ContractType = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    ResponsibleName = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    OrganizationName = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    ExpectedSubscribersCount = table.Column<int>(type: "integer", nullable: false),
                    ContactNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Address = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    ContractDuration = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    AdditionalInfo = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    Status = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Notes = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedByUserId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contract_service_requests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "service_request_page_settings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PageType = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
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
                    table.PrimaryKey("PK_service_request_page_settings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "vacant_jobs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TitleAr = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    TitleEn = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    DescriptionAr = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    DescriptionEn = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedByUserId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vacant_jobs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "employment_application_requests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FullName = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    ResidencePlace = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    MobileNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    AcademicDegree = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    PreviousExperience = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    YearsOfExperience = table.Column<int>(type: "integer", nullable: false),
                    Skills = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    AdditionalCertificates = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    VacantJobId = table.Column<int>(type: "integer", nullable: false),
                    CvFileUrl = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                    CvOriginalFileName = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Status = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Notes = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedByUserId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_employment_application_requests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_employment_application_requests_vacant_jobs_VacantJobId",
                        column: x => x.VacantJobId,
                        principalTable: "vacant_jobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_client_join_requests_CreatedAt",
                table: "client_join_requests",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_client_join_requests_Status",
                table: "client_join_requests",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_contract_service_requests_CreatedAt",
                table: "contract_service_requests",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_contract_service_requests_Status",
                table: "contract_service_requests",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_employment_application_requests_CreatedAt",
                table: "employment_application_requests",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_employment_application_requests_Status",
                table: "employment_application_requests",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_employment_application_requests_VacantJobId",
                table: "employment_application_requests",
                column: "VacantJobId");

            migrationBuilder.CreateIndex(
                name: "IX_service_request_page_settings_CreatedAt",
                table: "service_request_page_settings",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_service_request_page_settings_IsActive",
                table: "service_request_page_settings",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_service_request_page_settings_PageType",
                table: "service_request_page_settings",
                column: "PageType",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_vacant_jobs_CreatedAt",
                table: "vacant_jobs",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_vacant_jobs_IsActive",
                table: "vacant_jobs",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_vacant_jobs_SortOrder",
                table: "vacant_jobs",
                column: "SortOrder");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "client_join_requests");

            migrationBuilder.DropTable(
                name: "contract_service_requests");

            migrationBuilder.DropTable(
                name: "employment_application_requests");

            migrationBuilder.DropTable(
                name: "service_request_page_settings");

            migrationBuilder.DropTable(
                name: "vacant_jobs");
        }
    }
}
