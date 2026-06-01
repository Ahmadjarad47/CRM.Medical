using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CRM.Medical.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddInsuranceApprovalRequests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "insurance_approval_requests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PatientId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    InsuredName = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    InsuranceNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    MobileNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    InsuranceCardImageUrl = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                    InsuranceCardOriginalFileName = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    PrescriptionImageUrl = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                    PrescriptionOriginalFileName = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Status = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Notes = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    RejectionReason = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedByUserId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_insurance_approval_requests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_insurance_approval_requests_users_PatientId",
                        column: x => x.PatientId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_insurance_approval_requests_CreatedAt",
                table: "insurance_approval_requests",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_insurance_approval_requests_InsuranceNumber",
                table: "insurance_approval_requests",
                column: "InsuranceNumber");

            migrationBuilder.CreateIndex(
                name: "IX_insurance_approval_requests_PatientId",
                table: "insurance_approval_requests",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_insurance_approval_requests_Status",
                table: "insurance_approval_requests",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "insurance_approval_requests");
        }
    }
}
