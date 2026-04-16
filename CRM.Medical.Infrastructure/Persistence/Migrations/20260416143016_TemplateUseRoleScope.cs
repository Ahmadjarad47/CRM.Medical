using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Medical.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class TemplateUseRoleScope : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_templates_users_UserId",
                table: "templates");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "templates",
                newName: "Role");

            migrationBuilder.RenameIndex(
                name: "IX_templates_UserId",
                table: "templates",
                newName: "IX_templates_Role");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Role",
                table: "templates",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_templates_Role",
                table: "templates",
                newName: "IX_templates_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_templates_users_UserId",
                table: "templates",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
