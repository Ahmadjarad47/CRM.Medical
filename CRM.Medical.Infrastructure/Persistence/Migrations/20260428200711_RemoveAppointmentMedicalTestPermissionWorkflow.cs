using System;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CRM.Medical.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveAppointmentMedicalTestPermissionWorkflow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "appointments");

            migrationBuilder.DropTable(
                name: "permissions");

            migrationBuilder.DropTable(
                name: "test_results");

            migrationBuilder.DropTable(
                name: "appointment_types");

            migrationBuilder.DropTable(
                name: "test_requests");

            migrationBuilder.DropTable(
                name: "medical_tests");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "appointment_types",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_appointment_types", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "medical_tests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedByUserId = table.Column<string>(type: "text", nullable: false),
                    Category = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    NameAr = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    NameEn = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ParameterSchema = table.Column<JsonDocument>(type: "jsonb", nullable: true),
                    Price = table.Column<double>(type: "double precision", nullable: false),
                    SampleType = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Status = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_medical_tests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_medical_tests_users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "permissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_permissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "appointments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AppointmentTypeId = table.Column<int>(type: "integer", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "text", nullable: false),
                    DoctorId = table.Column<string>(type: "text", nullable: true),
                    LabPartnerId = table.Column<string>(type: "text", nullable: true),
                    MedicalTestId = table.Column<int>(type: "integer", nullable: true),
                    PatientId = table.Column<string>(type: "text", nullable: false),
                    Address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    Latitude = table.Column<double>(type: "double precision", nullable: true),
                    LocationType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Longitude = table.Column<double>(type: "double precision", nullable: true),
                    MedicalTestCompletionStatus = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    Name = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Notes = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    Slot = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_appointments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_appointments_appointment_types_AppointmentTypeId",
                        column: x => x.AppointmentTypeId,
                        principalTable: "appointment_types",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_appointments_medical_tests_MedicalTestId",
                        column: x => x.MedicalTestId,
                        principalTable: "medical_tests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_appointments_users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_appointments_users_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_appointments_users_LabPartnerId",
                        column: x => x.LabPartnerId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_appointments_users_PatientId",
                        column: x => x.PatientId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "test_requests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedByUserId = table.Column<string>(type: "text", nullable: false),
                    MedicalTestId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Metadata = table.Column<JsonDocument>(type: "jsonb", nullable: true),
                    Notes = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    RequestDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    TotalAmount = table.Column<double>(type: "double precision", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
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
                        name: "FK_test_requests_users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "test_results",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedByUserId = table.Column<string>(type: "text", nullable: false),
                    TestRequestId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PdfUrl = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    ResultData = table.Column<JsonDocument>(type: "jsonb", nullable: true),
                    ResultDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_test_results", x => x.Id);
                    table.ForeignKey(
                        name: "FK_test_results_test_requests_TestRequestId",
                        column: x => x.TestRequestId,
                        principalTable: "test_requests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_test_results_users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_appointment_types_IsActive",
                table: "appointment_types",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_appointment_types_Name",
                table: "appointment_types",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_appointments_AppointmentTypeId",
                table: "appointments",
                column: "AppointmentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_appointments_CreatedByUserId",
                table: "appointments",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_appointments_DoctorId",
                table: "appointments",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_appointments_LabPartnerId",
                table: "appointments",
                column: "LabPartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_appointments_MedicalTestCompletionStatus",
                table: "appointments",
                column: "MedicalTestCompletionStatus");

            migrationBuilder.CreateIndex(
                name: "IX_appointments_MedicalTestId",
                table: "appointments",
                column: "MedicalTestId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_appointments_PatientId",
                table: "appointments",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_appointments_Slot",
                table: "appointments",
                column: "Slot");

            migrationBuilder.CreateIndex(
                name: "IX_appointments_Status",
                table: "appointments",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_medical_tests_Category",
                table: "medical_tests",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_medical_tests_CreatedByUserId",
                table: "medical_tests",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_medical_tests_Status",
                table: "medical_tests",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_permissions_Name",
                table: "permissions",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_test_requests_CreatedByUserId",
                table: "test_requests",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_test_requests_MedicalTestId",
                table: "test_requests",
                column: "MedicalTestId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_test_requests_RequestDate",
                table: "test_requests",
                column: "RequestDate");

            migrationBuilder.CreateIndex(
                name: "IX_test_requests_Status",
                table: "test_requests",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_test_results_CreatedByUserId",
                table: "test_results",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_test_results_ResultDate",
                table: "test_results",
                column: "ResultDate");

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
    }
}
