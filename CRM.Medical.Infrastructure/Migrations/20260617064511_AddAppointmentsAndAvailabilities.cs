using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CRM.Medical.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAppointmentsAndAvailabilities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "appointments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TestRequestId = table.Column<int>(type: "integer", nullable: false),
                    ProviderUserId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    StartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Notes = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    MedicalTestCompletionStatus = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedByUserId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_appointments", x => x.Id);
                    table.CheckConstraint("CK_Appointment_TimeRange", "\"StartTime\" < \"EndTime\"");
                    table.ForeignKey(
                        name: "FK_appointments_test_requests_TestRequestId",
                        column: x => x.TestRequestId,
                        principalTable: "test_requests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_appointments_users_ProviderUserId",
                        column: x => x.ProviderUserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "availabilities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    DayOfWeek = table.Column<int>(type: "integer", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    SlotDuration = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedByUserId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_availabilities", x => x.Id);
                    table.CheckConstraint("CK_Availability_DayOfWeek", "\"DayOfWeek\" >= 0 AND \"DayOfWeek\" <= 6");
                    table.CheckConstraint("CK_Availability_SlotDuration", "\"SlotDuration\" > 0");
                    table.CheckConstraint("CK_Availability_TimeRange", "\"StartTime\" < \"EndTime\"");
                    table.ForeignKey(
                        name: "FK_availabilities_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_appointments_ProviderUserId",
                table: "appointments",
                column: "ProviderUserId");

            migrationBuilder.CreateIndex(
                name: "IX_appointments_ProviderUserId_StartTime_EndTime",
                table: "appointments",
                columns: new[] { "ProviderUserId", "StartTime", "EndTime" });

            migrationBuilder.CreateIndex(
                name: "IX_appointments_StartTime",
                table: "appointments",
                column: "StartTime");

            migrationBuilder.CreateIndex(
                name: "IX_appointments_Status",
                table: "appointments",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_appointments_TestRequestId",
                table: "appointments",
                column: "TestRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_availabilities_UserId",
                table: "availabilities",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_availabilities_UserId_DayOfWeek_IsActive",
                table: "availabilities",
                columns: new[] { "UserId", "DayOfWeek", "IsActive" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "appointments");

            migrationBuilder.DropTable(
                name: "availabilities");
        }
    }
}
