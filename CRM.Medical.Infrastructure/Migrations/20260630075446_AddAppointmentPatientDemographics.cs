using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Medical.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAppointmentPatientDemographics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Age",
                table: "appointments",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Gender",
                table: "appointments",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PatientPosition",
                table: "appointments",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Age",
                table: "appointments");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "appointments");

            migrationBuilder.DropColumn(
                name: "PatientPosition",
                table: "appointments");
        }
    }
}
