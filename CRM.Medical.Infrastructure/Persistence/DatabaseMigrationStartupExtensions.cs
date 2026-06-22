using System.Data;
using CRM.Medical.Application.Configuration.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CRM.Medical.Infrastructure.Persistence;

public static class DatabaseMigrationStartupExtensions
{
    public static async Task ApplyDatabaseMigrationsWithBaselineAsync(this IServiceProvider services, ILogger logger)
    {
        await using var scope = services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<MedicalDbContext>();
        var settings = scope.ServiceProvider.GetRequiredService<IOptions<DatabaseSettings>>().Value;

        if (!settings.AutoMigrate)
        {
            logger.LogInformation("Database auto migration is disabled by configuration.");
            return;
        }

        try
        {
            await EnsureAccessPoliciesCompatibilityColumnsAsync(db, logger);
            await EnsureComplaintCompatibilityColumnsAsync(db, logger);
            await EnsureAppointmentsCompatibilityColumnsAsync(db, logger);
            await EnsureAdsCompatibilitySchemaAsync(db, logger);

            if (settings.BaselineExistingDatabase)
                await BaselineExistingSchemaIfNeededAsync(db, logger);

            await db.Database.MigrateAsync();
            logger.LogInformation("Database migration completed successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Database migration failed. Application startup will continue without crashing.");
        }
    }

    private static async Task EnsureAccessPoliciesCompatibilityColumnsAsync(MedicalDbContext db, ILogger logger)
    {
        const string ensureColumnsSql = """
            ALTER TABLE IF EXISTS "access_policies"
                ADD COLUMN IF NOT EXISTS "SubjectKey" character varying(256) NOT NULL DEFAULT '',
                ADD COLUMN IF NOT EXISTS "ValidFrom" timestamp with time zone NULL,
                ADD COLUMN IF NOT EXISTS "ValidTo" timestamp with time zone NULL,
                ADD COLUMN IF NOT EXISTS "DeletedAt" timestamp with time zone NULL,
                ADD COLUMN IF NOT EXISTS "UpdatedByUserId" character varying(450) NULL;
            """;

        await db.Database.ExecuteSqlRawAsync(ensureColumnsSql);

        const string convertEffectSql = """
            DO $$
            DECLARE effect_data_type text;
            DECLARE subject_type_data_type text;
            DECLARE has_legacy_name_column boolean;
            BEGIN
                SELECT data_type
                INTO effect_data_type
                FROM information_schema.columns
                WHERE table_schema = 'public'
                  AND table_name = 'access_policies'
                  AND column_name = 'Effect';

                IF effect_data_type IN ('smallint', 'integer', 'bigint') THEN
                    ALTER TABLE IF EXISTS "access_policies"
                    ALTER COLUMN "Effect" TYPE character varying(16)
                    USING CASE
                        WHEN "Effect" = 0 THEN 'Deny'
                        WHEN "Effect" = 1 THEN 'Allow'
                        ELSE "Effect"::text
                    END;
                ELSIF effect_data_type IN ('character varying', 'text') THEN
                    UPDATE "access_policies"
                    SET "Effect" = CASE lower("Effect")
                        WHEN 'deny' THEN 'Deny'
                        WHEN 'allow' THEN 'Allow'
                        ELSE "Effect"
                    END
                    WHERE "Effect" IS NOT NULL;
                END IF;

                SELECT data_type
                INTO subject_type_data_type
                FROM information_schema.columns
                WHERE table_schema = 'public'
                  AND table_name = 'access_policies'
                  AND column_name = 'SubjectType';

                IF subject_type_data_type IN ('smallint', 'integer', 'bigint') THEN
                    ALTER TABLE IF EXISTS "access_policies"
                    ALTER COLUMN "SubjectType" TYPE character varying(64)
                    USING CASE
                        WHEN "SubjectType" = 0 THEN 'User'
                        WHEN "SubjectType" = 1 THEN 'Role'
                        ELSE "SubjectType"::text
                    END;
                ELSIF subject_type_data_type IN ('character varying', 'text') THEN
                    UPDATE "access_policies"
                    SET "SubjectType" = CASE lower("SubjectType")
                        WHEN 'user' THEN 'User'
                        WHEN 'role' THEN 'Role'
                        WHEN 'authenticated' THEN 'Authenticated'
                        WHEN 'all' THEN 'All'
                        ELSE "SubjectType"
                    END
                    WHERE "SubjectType" IS NOT NULL;
                END IF;

                SELECT EXISTS(
                    SELECT 1
                    FROM information_schema.columns
                    WHERE table_schema = 'public'
                      AND table_name = 'access_policies'
                      AND column_name = 'Name')
                INTO has_legacy_name_column;

                IF has_legacy_name_column THEN
                    EXECUTE 'UPDATE "access_policies" SET "Name" = '''' WHERE "Name" IS NULL;';
                    EXECUTE 'ALTER TABLE "access_policies" ALTER COLUMN "Name" SET DEFAULT '''';';
                END IF;
            END $$;
            """;
        await db.Database.ExecuteSqlRawAsync(convertEffectSql);
        logger.LogInformation(
            "Ensured access_policies compatibility (columns and Effect type conversion).");
    }

    private static async Task EnsureComplaintCompatibilityColumnsAsync(MedicalDbContext db, ILogger logger)
    {
        const string ensureColumnsSql = """
            ALTER TABLE IF EXISTS "complaints"
                ADD COLUMN IF NOT EXISTS "Note" character varying(4000) NULL;
            """;

        await db.Database.ExecuteSqlRawAsync(ensureColumnsSql);
        logger.LogInformation("Ensured complaints compatibility columns.");
    }

    private static async Task EnsureAppointmentsCompatibilityColumnsAsync(MedicalDbContext db, ILogger logger)
    {
        const string ensureColumnsSql = """
            ALTER TABLE IF EXISTS "appointments"
                ADD COLUMN IF NOT EXISTS "AvailabilityId" integer NULL;
            """;
        await db.Database.ExecuteSqlRawAsync(ensureColumnsSql);

        const string ensureIndexSql = """
            CREATE INDEX IF NOT EXISTS "IX_appointments_AvailabilityId"
                ON "appointments" ("AvailabilityId");
            """;
        await db.Database.ExecuteSqlRawAsync(ensureIndexSql);

        const string ensureForeignKeySql = """
            DO $$
            BEGIN
                IF EXISTS (
                    SELECT 1
                    FROM information_schema.tables
                    WHERE table_schema = 'public' AND table_name = 'appointments'
                )
                AND EXISTS (
                    SELECT 1
                    FROM information_schema.tables
                    WHERE table_schema = 'public' AND table_name = 'availabilities'
                )
                AND NOT EXISTS (
                    SELECT 1
                    FROM information_schema.table_constraints
                    WHERE table_schema = 'public'
                      AND table_name = 'appointments'
                      AND constraint_name = 'FK_appointments_availabilities_AvailabilityId'
                ) THEN
                    ALTER TABLE "appointments"
                        ADD CONSTRAINT "FK_appointments_availabilities_AvailabilityId"
                        FOREIGN KEY ("AvailabilityId")
                        REFERENCES "availabilities" ("Id")
                        ON DELETE RESTRICT;
                END IF;
            END $$;
            """;
        await db.Database.ExecuteSqlRawAsync(ensureForeignKeySql);

        const string ensureTestRequestNullableSql = """
            DO $$
            BEGIN
                IF EXISTS (
                    SELECT 1
                    FROM information_schema.columns
                    WHERE table_schema = 'public'
                      AND table_name = 'appointments'
                      AND column_name = 'TestRequestId'
                      AND is_nullable = 'NO'
                ) THEN
                    ALTER TABLE "appointments"
                        ALTER COLUMN "TestRequestId" DROP NOT NULL;
                END IF;
            END $$;
            """;
        await db.Database.ExecuteSqlRawAsync(ensureTestRequestNullableSql);

        logger.LogInformation("Ensured appointments compatibility columns.");
    }

    private static async Task EnsureAdsCompatibilitySchemaAsync(MedicalDbContext db, ILogger logger)
    {
        const string ensureTableSql = """
            CREATE TABLE IF NOT EXISTS "ads" (
                "Id" integer GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY,
                "Name" character varying(200) NOT NULL,
                "Description" character varying(4000) NOT NULL,
                "MediaType" integer NOT NULL,
                "MediaUrl" character varying(2048) NOT NULL,
                "Latitude" double precision NULL,
                "Longitude" double precision NULL,
                "AddressName" character varying(300) NOT NULL DEFAULT '',
                "CreatedAt" timestamp with time zone NOT NULL,
                "UpdatedAt" timestamp with time zone NULL,
                "CreatedByUserId" character varying(450) NULL,
                "UpdatedByUserId" character varying(450) NULL,
                "DeletedAt" timestamp with time zone NULL
            );
            """;
        await db.Database.ExecuteSqlRawAsync(ensureTableSql);

        const string ensureColumnsSql = """
            ALTER TABLE IF EXISTS "ads"
                ADD COLUMN IF NOT EXISTS "Latitude" double precision NULL,
                ADD COLUMN IF NOT EXISTS "Longitude" double precision NULL,
                ADD COLUMN IF NOT EXISTS "AddressName" character varying(300) NOT NULL DEFAULT '',
                ADD COLUMN IF NOT EXISTS "UpdatedByUserId" character varying(450) NULL,
                ADD COLUMN IF NOT EXISTS "DeletedAt" timestamp with time zone NULL;
            """;
        await db.Database.ExecuteSqlRawAsync(ensureColumnsSql);

        const string ensureAddressNameDefaultSql = """
            UPDATE "ads"
            SET "AddressName" = ''
            WHERE "AddressName" IS NULL;
            """;
        await db.Database.ExecuteSqlRawAsync(ensureAddressNameDefaultSql);

        const string ensureIndexesSql = """
            CREATE INDEX IF NOT EXISTS "IX_ads_CreatedAt"
                ON "ads" ("CreatedAt");
            CREATE INDEX IF NOT EXISTS "IX_ads_MediaType"
                ON "ads" ("MediaType");
            """;
        await db.Database.ExecuteSqlRawAsync(ensureIndexesSql);

        logger.LogInformation("Ensured ads compatibility schema.");
    }

    private static async Task BaselineExistingSchemaIfNeededAsync(MedicalDbContext db, ILogger logger)
    {
        var pendingMigrations = (await db.Database.GetPendingMigrationsAsync()).ToList();
        if (pendingMigrations.Count == 0)
            return;

        var migrationsAssembly = db.GetService<IMigrationsAssembly>();
        await EnsureMigrationsHistoryTableExistsAsync(db, logger);
        var appliedMigrationIds = await GetAppliedMigrationIdsAsync(db);
        var baselineCount = 0;

        foreach (var migrationId in pendingMigrations)
        {
            if (appliedMigrationIds.Contains(migrationId))
                continue;

            if (!migrationsAssembly.Migrations.TryGetValue(migrationId, out var migrationType))
                continue;

            var migration = migrationsAssembly.CreateMigration(migrationType, db.Database.ProviderName!);
            var createTableOperations = migration.UpOperations.OfType<CreateTableOperation>().ToList();
            var addColumnOperations = migration.UpOperations.OfType<AddColumnOperation>().ToList();
            if (createTableOperations.Count == 0 && addColumnOperations.Count == 0)
                continue;

            var migrationAlreadyAppliedToSchema = true;
            foreach (var operation in createTableOperations)
            {
                if (await TableExistsAsync(db, operation.Name, operation.Schema))
                    continue;

                migrationAlreadyAppliedToSchema = false;
                break;
            }

            if (!migrationAlreadyAppliedToSchema)
                continue;

            foreach (var operation in addColumnOperations)
            {
                if (await ColumnExistsAsync(db, operation.Table, operation.Name, operation.Schema))
                    continue;

                migrationAlreadyAppliedToSchema = false;
                break;
            }

            if (!migrationAlreadyAppliedToSchema)
                continue;

            await InsertMigrationHistoryRowAsync(db, migrationId, ProductInfo.GetVersion());
            appliedMigrationIds.Add(migrationId);
            baselineCount++;
        }

        if (baselineCount > 0)
        {
            logger.LogWarning(
                "Baselined {BaselineCount} migration(s) into __EFMigrationsHistory for existing schema.",
                baselineCount);
        }
    }

    private static async Task EnsureMigrationsHistoryTableExistsAsync(MedicalDbContext db, ILogger logger)
    {
        const string sql = """
            CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
                "MigrationId" character varying(150) NOT NULL,
                "ProductVersion" character varying(32) NOT NULL,
                CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
            );
            """;
        await db.Database.ExecuteSqlRawAsync(sql);
        logger.LogDebug("Ensured __EFMigrationsHistory table exists.");
    }

    private static async Task<HashSet<string>> GetAppliedMigrationIdsAsync(MedicalDbContext db)
    {
        await using var connection = db.Database.GetDbConnection();
        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = """SELECT "MigrationId" FROM "__EFMigrationsHistory";""";
        await using var reader = await command.ExecuteReaderAsync();
        var result = new HashSet<string>(StringComparer.Ordinal);
        while (await reader.ReadAsync())
            result.Add(reader.GetString(0));
        return result;
    }

    private static async Task<bool> TableExistsAsync(MedicalDbContext db, string tableName, string? schema)
    {
        await using var connection = db.Database.GetDbConnection();
        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT 1
            FROM information_schema.tables
            WHERE table_schema = @schema
              AND table_name = @table
            LIMIT 1;
            """;

        var schemaParameter = command.CreateParameter();
        schemaParameter.ParameterName = "@schema";
        schemaParameter.Value = schema ?? "public";
        command.Parameters.Add(schemaParameter);

        var tableParameter = command.CreateParameter();
        tableParameter.ParameterName = "@table";
        tableParameter.Value = tableName;
        command.Parameters.Add(tableParameter);

        var scalar = await command.ExecuteScalarAsync();
        return scalar is not null;
    }

    private static async Task<bool> ColumnExistsAsync(
        MedicalDbContext db,
        string tableName,
        string columnName,
        string? schema)
    {
        await using var connection = db.Database.GetDbConnection();
        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT 1
            FROM information_schema.columns
            WHERE table_schema = @schema
              AND table_name = @table
              AND column_name = @column
            LIMIT 1;
            """;

        var schemaParameter = command.CreateParameter();
        schemaParameter.ParameterName = "@schema";
        schemaParameter.Value = schema ?? "public";
        command.Parameters.Add(schemaParameter);

        var tableParameter = command.CreateParameter();
        tableParameter.ParameterName = "@table";
        tableParameter.Value = tableName;
        command.Parameters.Add(tableParameter);

        var columnParameter = command.CreateParameter();
        columnParameter.ParameterName = "@column";
        columnParameter.Value = columnName;
        command.Parameters.Add(columnParameter);

        var scalar = await command.ExecuteScalarAsync();
        return scalar is not null;
    }

    private static async Task InsertMigrationHistoryRowAsync(MedicalDbContext db, string migrationId, string productVersion)
    {
        await using var connection = db.Database.GetDbConnection();
        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = """
            INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
            VALUES (@migrationId, @productVersion)
            ON CONFLICT ("MigrationId") DO NOTHING;
            """;

        var migrationIdParameter = command.CreateParameter();
        migrationIdParameter.ParameterName = "@migrationId";
        migrationIdParameter.Value = migrationId;
        command.Parameters.Add(migrationIdParameter);

        var productVersionParameter = command.CreateParameter();
        productVersionParameter.ParameterName = "@productVersion";
        productVersionParameter.Value = productVersion;
        command.Parameters.Add(productVersionParameter);

        await command.ExecuteNonQueryAsync();
    }
}
