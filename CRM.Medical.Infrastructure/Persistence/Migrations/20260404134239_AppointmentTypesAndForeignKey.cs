using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CRM.Medical.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AppointmentTypesAndForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "appointment_types",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_appointment_types", x => x.Id);
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

            migrationBuilder.Sql(
                """
                INSERT INTO appointment_types ("Name", "IsActive", "CreatedAt")
                VALUES ('General', TRUE, TIMESTAMPTZ '2026-01-01T00:00:00Z');
                """);

            migrationBuilder.AddColumn<int>(
                name: "AppointmentTypeId",
                table: "appointments",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateIndex(
                name: "IX_appointments_AppointmentTypeId",
                table: "appointments",
                column: "AppointmentTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_appointments_appointment_types_AppointmentTypeId",
                table: "appointments",
                column: "AppointmentTypeId",
                principalTable: "appointment_types",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_appointments_appointment_types_AppointmentTypeId",
                table: "appointments");

            migrationBuilder.DropIndex(
                name: "IX_appointments_AppointmentTypeId",
                table: "appointments");

            migrationBuilder.DropColumn(
                name: "AppointmentTypeId",
                table: "appointments");

            migrationBuilder.DropTable(
                name: "appointment_types");
        }
    }
}
