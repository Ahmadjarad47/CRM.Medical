using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Medical.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAppointmentPatientVisitLocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "PatientLatitude",
                table: "appointments",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PatientLocationType",
                table: "appointments",
                type: "character varying(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "PatientLongitude",
                table: "appointments",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_Appointment_PatientLatitudeRange",
                table: "appointments",
                sql: "\"PatientLatitude\" IS NULL OR (\"PatientLatitude\" >= -90 AND \"PatientLatitude\" <= 90)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Appointment_PatientLongitudeRange",
                table: "appointments",
                sql: "\"PatientLongitude\" IS NULL OR (\"PatientLongitude\" >= -180 AND \"PatientLongitude\" <= 180)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Appointment_PatientLatitudeRange",
                table: "appointments");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Appointment_PatientLongitudeRange",
                table: "appointments");

            migrationBuilder.DropColumn(
                name: "PatientLatitude",
                table: "appointments");

            migrationBuilder.DropColumn(
                name: "PatientLocationType",
                table: "appointments");

            migrationBuilder.DropColumn(
                name: "PatientLongitude",
                table: "appointments");
        }
    }
}
