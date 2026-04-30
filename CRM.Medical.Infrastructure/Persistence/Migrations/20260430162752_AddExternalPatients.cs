using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CRM.Medical.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddExternalPatients : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ExternalPatientId",
                table: "test_requests",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "external_patients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FullName = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Age = table.Column<int>(type: "integer", nullable: true),
                    Gender = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    PhoneNumber = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    ExternalId = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    LinkedDirectPatientId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedByUserId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_external_patients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_external_patients_users_LinkedDirectPatientId",
                        column: x => x.LinkedDirectPatientId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_test_requests_ExternalPatientId",
                table: "test_requests",
                column: "ExternalPatientId");

            migrationBuilder.CreateIndex(
                name: "IX_external_patients_ExternalId",
                table: "external_patients",
                column: "ExternalId");

            migrationBuilder.CreateIndex(
                name: "IX_external_patients_LinkedDirectPatientId",
                table: "external_patients",
                column: "LinkedDirectPatientId");

            migrationBuilder.AddForeignKey(
                name: "FK_test_requests_external_patients_ExternalPatientId",
                table: "test_requests",
                column: "ExternalPatientId",
                principalTable: "external_patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_test_requests_external_patients_ExternalPatientId",
                table: "test_requests");

            migrationBuilder.DropTable(
                name: "external_patients");

            migrationBuilder.DropIndex(
                name: "IX_test_requests_ExternalPatientId",
                table: "test_requests");

            migrationBuilder.DropColumn(
                name: "ExternalPatientId",
                table: "test_requests");
        }
    }
}
