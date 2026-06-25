using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Medical.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NormalizeMedicalTestStatusEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                UPDATE medical_tests
                SET "Status" = CASE
                    WHEN LOWER(BTRIM("Status")) IN ('confirm', 'confirmed', 'active') THEN 'Confirm'
                    WHEN LOWER(BTRIM("Status")) IN ('cancel', 'cancelled', 'canceled', 'archived') THEN 'Cancel'
                    ELSE 'Pending'
                END;
                """);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "medical_tests",
                type: "character varying(32)",
                maxLength: 32,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldMaxLength: 64);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                UPDATE medical_tests
                SET "Status" = CASE
                    WHEN LOWER(BTRIM("Status")) = 'confirm' THEN 'Active'
                    WHEN LOWER(BTRIM("Status")) = 'cancel' THEN 'Cancelled'
                    ELSE 'Draft'
                END;
                """);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "medical_tests",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(32)",
                oldMaxLength: 32);
        }
    }
}
