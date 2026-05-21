using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Medical.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddComplaintNote : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "complaints",
                type: "character varying(4000)",
                maxLength: 4000,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Note",
                table: "complaints");
        }
    }
}
