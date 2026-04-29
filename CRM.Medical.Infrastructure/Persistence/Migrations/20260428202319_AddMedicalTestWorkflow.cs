using System;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CRM.Medical.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddMedicalTestWorkflow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "medical_tests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NameAr = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    NameEn = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Price = table.Column<double>(type: "double precision", nullable: false),
                    Category = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    SampleType = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ParameterSchema = table.Column<JsonDocument>(type: "jsonb", nullable: true),
                    Status = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedByUserId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_medical_tests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "test_requests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MedicalTestId = table.Column<int>(type: "integer", nullable: false),
                    DoctorId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true),
                    LabClientId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true),
                    DirectPatientId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true),
                    RequestDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    TotalAmount = table.Column<double>(type: "double precision", nullable: false),
                    Notes = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    Metadata = table.Column<JsonDocument>(type: "jsonb", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedByUserId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_test_requests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_test_requests_medical_tests_MedicalTestId",
                        column: x => x.MedicalTestId,
                        principalTable: "medical_tests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_test_requests_users_DirectPatientId",
                        column: x => x.DirectPatientId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_test_requests_users_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_test_requests_users_LabClientId",
                        column: x => x.LabClientId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "test_results",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TestRequestId = table.Column<int>(type: "integer", nullable: false),
                    ResultDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ResultData = table.Column<JsonDocument>(type: "jsonb", nullable: true),
                    PdfUrl = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Status = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedByUserId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_test_results", x => x.Id);
                    table.ForeignKey(
                        name: "FK_test_results_test_requests_TestRequestId",
                        column: x => x.TestRequestId,
                        principalTable: "test_requests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_medical_tests_Status",
                table: "medical_tests",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_test_requests_CreatedByUserId",
                table: "test_requests",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_test_requests_DirectPatientId",
                table: "test_requests",
                column: "DirectPatientId");

            migrationBuilder.CreateIndex(
                name: "IX_test_requests_DoctorId",
                table: "test_requests",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_test_requests_LabClientId",
                table: "test_requests",
                column: "LabClientId");

            migrationBuilder.CreateIndex(
                name: "IX_test_requests_MedicalTestId",
                table: "test_requests",
                column: "MedicalTestId");

            migrationBuilder.CreateIndex(
                name: "IX_test_requests_Status",
                table: "test_requests",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_test_results_Status",
                table: "test_results",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_test_results_TestRequestId",
                table: "test_results",
                column: "TestRequestId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "test_results");

            migrationBuilder.DropTable(
                name: "test_requests");

            migrationBuilder.DropTable(
                name: "medical_tests");
        }
    }
}
