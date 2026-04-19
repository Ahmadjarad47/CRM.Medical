using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Medical.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUserCreatedByUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "users",
                type: "character varying(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_CreatedByUserId",
                table: "users",
                column: "CreatedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_users_users_CreatedByUserId",
                table: "users",
                column: "CreatedByUserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_users_users_CreatedByUserId",
                table: "users");

            migrationBuilder.DropIndex(
                name: "IX_users_CreatedByUserId",
                table: "users");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "users");
        }
    }
}
