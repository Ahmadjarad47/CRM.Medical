using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Medical.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSubjectIdToAccessPolicies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_access_policies_Resource_Action_IsEnabled",
                table: "access_policies");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "access_policies",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SubjectId",
                table: "access_policies",
                type: "character varying(450)",
                maxLength: 450,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_access_policies_Priority_Effect",
                table: "access_policies",
                columns: new[] { "Priority", "Effect" });

            migrationBuilder.CreateIndex(
                name: "IX_access_policies_Resource_Action_SubjectType_SubjectId_IsEna~",
                table: "access_policies",
                columns: new[] { "Resource", "Action", "SubjectType", "SubjectId", "IsEnabled" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_access_policies_Priority_Effect",
                table: "access_policies");

            migrationBuilder.DropIndex(
                name: "IX_access_policies_Resource_Action_SubjectType_SubjectId_IsEna~",
                table: "access_policies");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "access_policies");

            migrationBuilder.DropColumn(
                name: "SubjectId",
                table: "access_policies");

            migrationBuilder.CreateIndex(
                name: "IX_access_policies_Resource_Action_IsEnabled",
                table: "access_policies",
                columns: new[] { "Resource", "Action", "IsEnabled" });
        }
    }
}
