using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Medical.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPermissionsCatalog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                CREATE TABLE IF NOT EXISTS permissions (
                    "Id" uuid NOT NULL,
                    "Name" character varying(128) NOT NULL,
                    "Description" character varying(500),
                    "CreatedAt" timestamp with time zone NOT NULL,
                    CONSTRAINT "PK_permissions" PRIMARY KEY ("Id")
                );
                """);

            migrationBuilder.Sql(
                """CREATE UNIQUE INDEX IF NOT EXISTS "IX_permissions_Name" ON permissions ("Name");""");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "permissions");
        }
    }
}
