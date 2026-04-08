using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Medical.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixSubscriptionPackagesLegacyColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // subscription_packages could exist from before the full model (CREATE TABLE IF NOT EXISTS skipped the full DDL).
            migrationBuilder.Sql(
                """
                ALTER TABLE subscription_packages ADD COLUMN IF NOT EXISTS "IncludedTests" jsonb;

                ALTER TABLE subscription_packages ADD COLUMN IF NOT EXISTS "IsActive" boolean;
                UPDATE subscription_packages SET "IsActive" = true WHERE "IsActive" IS NULL;
                ALTER TABLE subscription_packages ALTER COLUMN "IsActive" SET DEFAULT TRUE;
                ALTER TABLE subscription_packages ALTER COLUMN "IsActive" SET NOT NULL;

                ALTER TABLE subscription_packages ADD COLUMN IF NOT EXISTS "TargetAudience" character varying(32);
                UPDATE subscription_packages SET "TargetAudience" = 'All' WHERE "TargetAudience" IS NULL;
                ALTER TABLE subscription_packages ALTER COLUMN "TargetAudience" SET NOT NULL;
                """);

            migrationBuilder.Sql(
                """CREATE INDEX IF NOT EXISTS "IX_subscription_packages_TargetAudience" ON subscription_packages ("TargetAudience");""");

            migrationBuilder.Sql(
                """CREATE INDEX IF NOT EXISTS "IX_subscription_packages_IsActive" ON subscription_packages ("IsActive");""");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
