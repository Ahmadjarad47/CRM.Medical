using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Medical.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAppointmentAvailabilityId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AvailabilityId",
                table: "appointments",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_appointments_AvailabilityId",
                table: "appointments",
                column: "AvailabilityId");

            migrationBuilder.AddForeignKey(
                name: "FK_appointments_availabilities_AvailabilityId",
                table: "appointments",
                column: "AvailabilityId",
                principalTable: "availabilities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_appointments_availabilities_AvailabilityId",
                table: "appointments");

            migrationBuilder.DropIndex(
                name: "IX_appointments_AvailabilityId",
                table: "appointments");

            migrationBuilder.DropColumn(
                name: "AvailabilityId",
                table: "appointments");
        }
    }
}
