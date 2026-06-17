using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Medical.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SetAppointmentLocationTypeDefault : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PatientLocationType",
                table: "appointments",
                type: "character varying(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "ComeToUs",
                oldClrType: typeof(string),
                oldType: "character varying(32)",
                oldMaxLength: 32);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PatientLocationType",
                table: "appointments",
                type: "character varying(32)",
                maxLength: 32,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(32)",
                oldMaxLength: 32,
                oldDefaultValue: "ComeToUs");
        }
    }
}
