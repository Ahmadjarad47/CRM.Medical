using System.Data;
using CRM.Medical.Application.Configuration.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;

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
            await EnsureBannersCompatibilitySchemaAsync(db, logger);
            await EnsureWelcomePagesCompatibilitySchemaAsync(db, logger);
            await EnsureDynamicPagesCompatibilitySchemaAsync(db, logger);
            await EnsureCategoryMedicalCompatibilitySchemaAsync(db, logger);

            await MigrateWithBaselineRetryAsync(db, settings, logger);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Database migration failed. Application startup will continue without crashing.");
        }
    }

    private static async Task MigrateWithBaselineRetryAsync(
        MedicalDbContext db,
        DatabaseSettings settings,
        ILogger logger)
    {
        const int maxAttempts = 10;

        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                if (settings.BaselineExistingDatabase)
                    await BaselineExistingSchemaIfNeededAsync(db, logger);

                await db.Database.MigrateAsync();
                logger.LogInformation("Database migration completed successfully.");
                return;
            }
            catch (Exception ex) when (attempt < maxAttempts && settings.BaselineExistingDatabase && IsMigrationConflict(ex))
            {
                logger.LogWarning(
                    ex,
                    "Migration conflict on attempt {Attempt}, re-baselining pending migrations and retrying.",
                    attempt);
            }
        }
    }

    private static bool IsMigrationConflict(Exception ex)
    {
        var postgres = ex as PostgresException ?? ex.InnerException as PostgresException;
        return postgres?.SqlState is "42701" or "42P07" or "42710";
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
                ADD COLUMN IF NOT EXISTS "AvailabilityId" integer NULL,
                ADD COLUMN IF NOT EXISTS "AttachmentUrl" character varying(2048) NULL,
                ADD COLUMN IF NOT EXISTS "Age" integer NULL,
                ADD COLUMN IF NOT EXISTS "Gender" character varying(64) NULL,
                ADD COLUMN IF NOT EXISTS "PatientLatitude" double precision NULL,
                ADD COLUMN IF NOT EXISTS "PatientLongitude" double precision NULL,
                ADD COLUMN IF NOT EXISTS "PatientLocationType" character varying(32) NOT NULL DEFAULT 'ComeToUs',
                ADD COLUMN IF NOT EXISTS "MedicalTestCompletionStatus" character varying(64) NULL;
            """;
        await db.Database.ExecuteSqlRawAsync(ensureColumnsSql);

        const string normalizePatientLocationTypeSql = """
            DO $$
            BEGIN
                IF EXISTS (
                    SELECT 1
                    FROM information_schema.columns
                    WHERE table_schema = 'public'
                      AND table_name = 'appointments'
                      AND column_name = 'PatientLocationType'
                ) THEN
                    UPDATE "appointments"
                    SET "PatientLocationType" = 'ComeToUs'
                    WHERE "PatientLocationType" IS NULL OR BTRIM("PatientLocationType") = '';
                END IF;
            END $$;
            """;
        await db.Database.ExecuteSqlRawAsync(normalizePatientLocationTypeSql);

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
                "DisplayMode" integer NOT NULL DEFAULT 1,
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
                ADD COLUMN IF NOT EXISTS "DisplayMode" integer NOT NULL DEFAULT 1,
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
            CREATE INDEX IF NOT EXISTS "IX_ads_DisplayMode"
                ON "ads" ("DisplayMode");
            """;
        await db.Database.ExecuteSqlRawAsync(ensureIndexesSql);

        logger.LogInformation("Ensured ads compatibility schema.");
    }

    private static async Task EnsureBannersCompatibilitySchemaAsync(MedicalDbContext db, ILogger logger)
    {
        const string ensureDisplayModeColumnSql = """
            ALTER TABLE IF EXISTS "banners"
                ADD COLUMN IF NOT EXISTS "DisplayMode" integer NOT NULL DEFAULT 1;
            """;
        await db.Database.ExecuteSqlRawAsync(ensureDisplayModeColumnSql);

        const string migrateTypeToDisplayModeSql = """
            DO $$
            BEGIN
                IF EXISTS (
                    SELECT 1
                    FROM information_schema.columns
                    WHERE table_schema = 'public'
                      AND table_name = 'banners'
                      AND column_name = 'Type'
                ) THEN
                    UPDATE "banners"
                    SET "DisplayMode" = CASE lower(trim("Type"))
                        WHEN 'full' THEN 1
                        WHEN 'large' THEN 2
                        WHEN 'larg' THEN 2
                        WHEN 'small' THEN 3
                        WHEN 'xsmall' THEN 4
                        ELSE 1
                    END
                    WHERE "Type" IS NOT NULL AND trim("Type") <> '';

                    ALTER TABLE "banners" DROP COLUMN "Type";
                END IF;
            END $$;
            """;
        await db.Database.ExecuteSqlRawAsync(migrateTypeToDisplayModeSql);

        const string ensureIndexesSql = """
            CREATE INDEX IF NOT EXISTS "IX_banners_DisplayMode"
                ON "banners" ("DisplayMode");
            """;
        await db.Database.ExecuteSqlRawAsync(ensureIndexesSql);

        logger.LogInformation("Ensured banners compatibility schema.");
    }

    private static async Task EnsureWelcomePagesCompatibilitySchemaAsync(MedicalDbContext db, ILogger logger)
    {
        const string ensureTableSql = """
            CREATE TABLE IF NOT EXISTS "welcome_pages" (
                "Id" integer GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY,
                "Name" character varying(200) NOT NULL,
                "Description" character varying(4000) NOT NULL,
                "MediaType" integer NOT NULL,
                "MediaUrl" character varying(2048) NOT NULL,
                "IsActive" boolean NOT NULL DEFAULT TRUE,
                "CreatedAt" timestamp with time zone NOT NULL,
                "UpdatedAt" timestamp with time zone NULL,
                "CreatedByUserId" character varying(450) NULL
            );
            """;
        await db.Database.ExecuteSqlRawAsync(ensureTableSql);

        const string ensureColumnsSql = """
            ALTER TABLE IF EXISTS "welcome_pages"
                ADD COLUMN IF NOT EXISTS "Name" character varying(200) NOT NULL DEFAULT '',
                ADD COLUMN IF NOT EXISTS "Description" character varying(4000) NOT NULL DEFAULT '',
                ADD COLUMN IF NOT EXISTS "MediaType" integer NOT NULL DEFAULT 0,
                ADD COLUMN IF NOT EXISTS "MediaUrl" character varying(2048) NOT NULL DEFAULT '',
                ADD COLUMN IF NOT EXISTS "IsActive" boolean NOT NULL DEFAULT TRUE,
                ADD COLUMN IF NOT EXISTS "CreatedAt" timestamp with time zone NOT NULL DEFAULT NOW(),
                ADD COLUMN IF NOT EXISTS "UpdatedAt" timestamp with time zone NULL,
                ADD COLUMN IF NOT EXISTS "CreatedByUserId" character varying(450) NULL;
            """;
        await db.Database.ExecuteSqlRawAsync(ensureColumnsSql);

        const string ensureNotNullSafeDefaultsSql = """
            UPDATE "welcome_pages"
            SET
                "Name" = COALESCE("Name", ''),
                "Description" = COALESCE("Description", ''),
                "MediaUrl" = COALESCE("MediaUrl", ''),
                "IsActive" = COALESCE("IsActive", TRUE),
                "CreatedAt" = COALESCE("CreatedAt", NOW())
            WHERE
                "Name" IS NULL
                OR "Description" IS NULL
                OR "MediaUrl" IS NULL
                OR "IsActive" IS NULL
                OR "CreatedAt" IS NULL;
            """;
        await db.Database.ExecuteSqlRawAsync(ensureNotNullSafeDefaultsSql);

        const string ensureIndexesSql = """
            CREATE INDEX IF NOT EXISTS "IX_welcome_pages_CreatedAt"
                ON "welcome_pages" ("CreatedAt");
            CREATE INDEX IF NOT EXISTS "IX_welcome_pages_IsActive"
                ON "welcome_pages" ("IsActive");
            """;
        await db.Database.ExecuteSqlRawAsync(ensureIndexesSql);

        logger.LogInformation("Ensured welcome_pages compatibility schema.");
    }

    private static async Task EnsureDynamicPagesCompatibilitySchemaAsync(MedicalDbContext db, ILogger logger)
    {
        const string ensurePagesTableSql = """
            CREATE TABLE IF NOT EXISTS "pages" (
                "Id" integer GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY,
                "TemplateKey" character varying(120) NOT NULL,
                "ParentId" integer NULL,
                sort_order integer NOT NULL DEFAULT 0,
                "PublishStatus" character varying(32) NOT NULL DEFAULT 'Draft',
                "PublishScheduledAt" timestamp with time zone NULL,
                "PublishedAt" timestamp with time zone NULL,
                "IsVisibleInNav" boolean NOT NULL DEFAULT TRUE,
                "IsActive" boolean NOT NULL DEFAULT TRUE,
                visible_to_roles jsonb NOT NULL DEFAULT '[]'::jsonb,
                "CreatedByUserId" character varying(450) NOT NULL DEFAULT '',
                "UpdatedByUserId" character varying(450) NULL,
                "CreatedAt" timestamp with time zone NOT NULL DEFAULT NOW(),
                "UpdatedAt" timestamp with time zone NULL
            );
            """;
        await db.Database.ExecuteSqlRawAsync(ensurePagesTableSql);

        const string ensurePagesColumnsSql = """
            ALTER TABLE IF EXISTS "pages"
                ADD COLUMN IF NOT EXISTS "TemplateKey" character varying(120) NOT NULL DEFAULT '',
                ADD COLUMN IF NOT EXISTS "ParentId" integer NULL,
                ADD COLUMN IF NOT EXISTS sort_order integer NOT NULL DEFAULT 0,
                ADD COLUMN IF NOT EXISTS "PublishStatus" character varying(32) NOT NULL DEFAULT 'Draft',
                ADD COLUMN IF NOT EXISTS "PublishScheduledAt" timestamp with time zone NULL,
                ADD COLUMN IF NOT EXISTS "PublishedAt" timestamp with time zone NULL,
                ADD COLUMN IF NOT EXISTS "IsVisibleInNav" boolean NOT NULL DEFAULT TRUE,
                ADD COLUMN IF NOT EXISTS "IsActive" boolean NOT NULL DEFAULT TRUE,
                ADD COLUMN IF NOT EXISTS visible_to_roles jsonb NOT NULL DEFAULT '[]'::jsonb,
                ADD COLUMN IF NOT EXISTS "CreatedByUserId" character varying(450) NOT NULL DEFAULT '',
                ADD COLUMN IF NOT EXISTS "UpdatedByUserId" character varying(450) NULL,
                ADD COLUMN IF NOT EXISTS "CreatedAt" timestamp with time zone NOT NULL DEFAULT NOW(),
                ADD COLUMN IF NOT EXISTS "UpdatedAt" timestamp with time zone NULL;
            """;
        await db.Database.ExecuteSqlRawAsync(ensurePagesColumnsSql);

        const string ensurePagesDataSql = """
            UPDATE "pages"
            SET
                "TemplateKey" = COALESCE("TemplateKey", ''),
                "PublishStatus" = COALESCE("PublishStatus", 'Draft'),
                "IsVisibleInNav" = COALESCE("IsVisibleInNav", TRUE),
                "IsActive" = COALESCE("IsActive", TRUE),
                visible_to_roles = CASE
                    WHEN visible_to_roles IS NULL OR visible_to_roles::text IN ('""', '') THEN '[]'::jsonb
                    ELSE visible_to_roles
                END,
                "CreatedByUserId" = COALESCE("CreatedByUserId", ''),
                "CreatedAt" = COALESCE("CreatedAt", NOW())
            WHERE
                "TemplateKey" IS NULL
                OR "PublishStatus" IS NULL
                OR "IsVisibleInNav" IS NULL
                OR "IsActive" IS NULL
                OR visible_to_roles IS NULL
                OR visible_to_roles::text IN ('""', '')
                OR "CreatedByUserId" IS NULL
                OR "CreatedAt" IS NULL;
            """;
        await db.Database.ExecuteSqlRawAsync(ensurePagesDataSql);

        const string ensurePagesIndexesAndFkSql = """
            CREATE UNIQUE INDEX IF NOT EXISTS "IX_pages_TemplateKey"
                ON "pages" ("TemplateKey");
            CREATE INDEX IF NOT EXISTS "IX_pages_ParentId"
                ON "pages" ("ParentId");
            CREATE INDEX IF NOT EXISTS "IX_pages_PublishStatus"
                ON "pages" ("PublishStatus");
            CREATE INDEX IF NOT EXISTS "IX_pages_IsVisibleInNav"
                ON "pages" ("IsVisibleInNav");
            CREATE INDEX IF NOT EXISTS "IX_pages_IsActive"
                ON "pages" ("IsActive");
            CREATE INDEX IF NOT EXISTS "IX_pages_CreatedAt"
                ON "pages" ("CreatedAt");
            CREATE INDEX IF NOT EXISTS "IX_pages_sort_order_Id"
                ON "pages" (sort_order, "Id");

            DO $$
            BEGIN
                IF NOT EXISTS (
                    SELECT 1
                    FROM information_schema.table_constraints
                    WHERE table_schema = 'public'
                      AND table_name = 'pages'
                      AND constraint_name = 'FK_pages_pages_ParentId'
                ) THEN
                    ALTER TABLE "pages"
                        ADD CONSTRAINT "FK_pages_pages_ParentId"
                        FOREIGN KEY ("ParentId")
                        REFERENCES "pages" ("Id")
                        ON DELETE RESTRICT;
                END IF;
            END $$;
            """;
        await db.Database.ExecuteSqlRawAsync(ensurePagesIndexesAndFkSql);

        const string ensurePageTranslationsTableSql = """
            CREATE TABLE IF NOT EXISTS "page_translations" (
                "Id" integer GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY,
                "PageId" integer NOT NULL,
                "Language" character varying(10) NOT NULL,
                "Title" character varying(300) NOT NULL DEFAULT '',
                "Slug" character varying(300) NOT NULL DEFAULT '',
                "MetaTitle" character varying(300) NULL,
                "MetaDescription" character varying(1000) NULL,
                "MetaKeywords" character varying(1000) NULL,
                "OpenGraphImageUrl" character varying(2048) NULL,
                "CanonicalUrl" character varying(2048) NULL,
                "BreadcrumbTitle" character varying(300) NULL
            );
            """;
        await db.Database.ExecuteSqlRawAsync(ensurePageTranslationsTableSql);

        const string ensurePageTranslationsColumnsSql = """
            ALTER TABLE IF EXISTS "page_translations"
                ADD COLUMN IF NOT EXISTS "PageId" integer NULL,
                ADD COLUMN IF NOT EXISTS "Language" character varying(10) NOT NULL DEFAULT '',
                ADD COLUMN IF NOT EXISTS "Title" character varying(300) NOT NULL DEFAULT '',
                ADD COLUMN IF NOT EXISTS "Slug" character varying(300) NOT NULL DEFAULT '',
                ADD COLUMN IF NOT EXISTS "MetaTitle" character varying(300) NULL,
                ADD COLUMN IF NOT EXISTS "MetaDescription" character varying(1000) NULL,
                ADD COLUMN IF NOT EXISTS "MetaKeywords" character varying(1000) NULL,
                ADD COLUMN IF NOT EXISTS "OpenGraphImageUrl" character varying(2048) NULL,
                ADD COLUMN IF NOT EXISTS "CanonicalUrl" character varying(2048) NULL,
                ADD COLUMN IF NOT EXISTS "BreadcrumbTitle" character varying(300) NULL;
            """;
        await db.Database.ExecuteSqlRawAsync(ensurePageTranslationsColumnsSql);

        const string ensurePageTranslationsIndexesAndFkSql = """
            CREATE UNIQUE INDEX IF NOT EXISTS "IX_page_translations_PageId_Language"
                ON "page_translations" ("PageId", "Language");
            CREATE UNIQUE INDEX IF NOT EXISTS "IX_page_translations_Language_Slug"
                ON "page_translations" ("Language", "Slug");
            CREATE INDEX IF NOT EXISTS "IX_page_translations_Slug"
                ON "page_translations" ("Slug");

            DO $$
            BEGIN
                IF EXISTS (
                    SELECT 1
                    FROM information_schema.columns
                    WHERE table_schema = 'public'
                      AND table_name = 'page_translations'
                      AND column_name = 'PageId'
                )
                AND EXISTS (
                    SELECT 1
                    FROM information_schema.tables
                    WHERE table_schema = 'public'
                      AND table_name = 'pages'
                )
                AND NOT EXISTS (
                    SELECT 1
                    FROM "page_translations" pt
                    LEFT JOIN "pages" p ON p."Id" = pt."PageId"
                    WHERE pt."PageId" IS NOT NULL
                      AND p."Id" IS NULL
                    LIMIT 1
                )
                AND NOT EXISTS (
                    SELECT 1
                    FROM information_schema.table_constraints
                    WHERE table_schema = 'public'
                      AND table_name = 'page_translations'
                      AND constraint_name = 'FK_page_translations_pages_PageId'
                ) THEN
                    ALTER TABLE "page_translations"
                        ADD CONSTRAINT "FK_page_translations_pages_PageId"
                        FOREIGN KEY ("PageId")
                        REFERENCES "pages" ("Id")
                        ON DELETE CASCADE;
                END IF;
            END $$;
            """;
        await db.Database.ExecuteSqlRawAsync(ensurePageTranslationsIndexesAndFkSql);

        const string ensureContentBlocksTableSql = """
            CREATE TABLE IF NOT EXISTS "content_blocks" (
                "Id" integer GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY,
                "PageId" integer NOT NULL,
                "BlockType" character varying(100) NOT NULL,
                sort_order integer NOT NULL DEFAULT 0,
                "CustomCssClass" character varying(200) NULL,
                "CustomStyles" jsonb NULL,
                "Animation" character varying(100) NULL,
                "VisibilityRules" jsonb NULL,
                "IsActive" boolean NOT NULL DEFAULT TRUE,
                "CreatedAt" timestamp with time zone NOT NULL DEFAULT NOW(),
                "UpdatedAt" timestamp with time zone NULL
            );
            """;
        await db.Database.ExecuteSqlRawAsync(ensureContentBlocksTableSql);

        const string ensureContentBlocksColumnsSql = """
            ALTER TABLE IF EXISTS "content_blocks"
                ADD COLUMN IF NOT EXISTS "PageId" integer NULL,
                ADD COLUMN IF NOT EXISTS "BlockType" character varying(100) NOT NULL DEFAULT '',
                ADD COLUMN IF NOT EXISTS sort_order integer NOT NULL DEFAULT 0,
                ADD COLUMN IF NOT EXISTS "CustomCssClass" character varying(200) NULL,
                ADD COLUMN IF NOT EXISTS "CustomStyles" jsonb NULL,
                ADD COLUMN IF NOT EXISTS "Animation" character varying(100) NULL,
                ADD COLUMN IF NOT EXISTS "VisibilityRules" jsonb NULL,
                ADD COLUMN IF NOT EXISTS "IsActive" boolean NOT NULL DEFAULT TRUE,
                ADD COLUMN IF NOT EXISTS "CreatedAt" timestamp with time zone NOT NULL DEFAULT NOW(),
                ADD COLUMN IF NOT EXISTS "UpdatedAt" timestamp with time zone NULL;
            """;
        await db.Database.ExecuteSqlRawAsync(ensureContentBlocksColumnsSql);

        const string ensureContentBlocksIndexesAndFkSql = """
            CREATE INDEX IF NOT EXISTS "IX_content_blocks_PageId"
                ON "content_blocks" ("PageId");
            CREATE INDEX IF NOT EXISTS "IX_content_blocks_IsActive"
                ON "content_blocks" ("IsActive");
            CREATE INDEX IF NOT EXISTS "IX_content_blocks_PageId_sort_order"
                ON "content_blocks" ("PageId", sort_order);

            DO $$
            BEGIN
                IF EXISTS (
                    SELECT 1
                    FROM information_schema.columns
                    WHERE table_schema = 'public'
                      AND table_name = 'content_blocks'
                      AND column_name = 'PageId'
                )
                AND EXISTS (
                    SELECT 1
                    FROM information_schema.tables
                    WHERE table_schema = 'public'
                      AND table_name = 'pages'
                )
                AND NOT EXISTS (
                    SELECT 1
                    FROM information_schema.table_constraints
                    WHERE table_schema = 'public'
                      AND table_name = 'content_blocks'
                      AND constraint_name = 'FK_content_blocks_pages_PageId'
                ) THEN
                    ALTER TABLE "content_blocks"
                        ADD CONSTRAINT "FK_content_blocks_pages_PageId"
                        FOREIGN KEY ("PageId")
                        REFERENCES "pages" ("Id")
                        ON DELETE CASCADE;
                END IF;
            END $$;
            """;
        await db.Database.ExecuteSqlRawAsync(ensureContentBlocksIndexesAndFkSql);

        const string ensureContentVersionsTableSql = """
            CREATE TABLE IF NOT EXISTS "content_versions" (
                "Id" integer GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY,
                "PageId" integer NOT NULL,
                "SnapshotData" jsonb NOT NULL DEFAULT '{{}}'::jsonb,
                "VersionNumber" integer NOT NULL DEFAULT 1,
                "ChangeNotes" character varying(2000) NULL,
                "CreatedByUserId" character varying(450) NOT NULL DEFAULT '',
                "CreatedAt" timestamp with time zone NOT NULL DEFAULT NOW()
            );
            """;
        await db.Database.ExecuteSqlRawAsync(ensureContentVersionsTableSql);

        const string ensureContentVersionsColumnsSql = """
            ALTER TABLE IF EXISTS "content_versions"
                ADD COLUMN IF NOT EXISTS "PageId" integer NULL,
                ADD COLUMN IF NOT EXISTS "SnapshotData" jsonb NOT NULL DEFAULT '{{}}'::jsonb,
                ADD COLUMN IF NOT EXISTS "VersionNumber" integer NOT NULL DEFAULT 1,
                ADD COLUMN IF NOT EXISTS "ChangeNotes" character varying(2000) NULL,
                ADD COLUMN IF NOT EXISTS "CreatedByUserId" character varying(450) NOT NULL DEFAULT '',
                ADD COLUMN IF NOT EXISTS "CreatedAt" timestamp with time zone NOT NULL DEFAULT NOW();
            """;
        await db.Database.ExecuteSqlRawAsync(ensureContentVersionsColumnsSql);

        const string ensureContentVersionsIndexesAndFkSql = """
            CREATE INDEX IF NOT EXISTS "IX_content_versions_PageId"
                ON "content_versions" ("PageId");
            CREATE UNIQUE INDEX IF NOT EXISTS "IX_content_versions_PageId_VersionNumber"
                ON "content_versions" ("PageId", "VersionNumber");
            CREATE INDEX IF NOT EXISTS "IX_content_versions_CreatedAt"
                ON "content_versions" ("CreatedAt");

            DO $$
            BEGIN
                IF EXISTS (
                    SELECT 1
                    FROM information_schema.columns
                    WHERE table_schema = 'public'
                      AND table_name = 'content_versions'
                      AND column_name = 'PageId'
                )
                AND EXISTS (
                    SELECT 1
                    FROM information_schema.tables
                    WHERE table_schema = 'public'
                      AND table_name = 'pages'
                )
                AND NOT EXISTS (
                    SELECT 1
                    FROM information_schema.table_constraints
                    WHERE table_schema = 'public'
                      AND table_name = 'content_versions'
                      AND constraint_name = 'FK_content_versions_pages_PageId'
                ) THEN
                    ALTER TABLE "content_versions"
                        ADD CONSTRAINT "FK_content_versions_pages_PageId"
                        FOREIGN KEY ("PageId")
                        REFERENCES "pages" ("Id")
                        ON DELETE CASCADE;
                END IF;
            END $$;
            """;
        await db.Database.ExecuteSqlRawAsync(ensureContentVersionsIndexesAndFkSql);

        const string ensureBlockLocalizationsTableSql = """
            CREATE TABLE IF NOT EXISTS "block_localizations" (
                "Id" integer GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY,
                "ContentBlockId" integer NOT NULL,
                "Language" character varying(10) NOT NULL,
                "Heading" character varying(300) NULL,
                "Subheading" character varying(600) NULL,
                "Description" character varying(4000) NULL,
                "ContentData" jsonb NULL,
                "MediaUrl" character varying(2048) NULL,
                "MediaAltText" character varying(500) NULL,
                "ButtonText" character varying(300) NULL,
                "ButtonLink" character varying(2048) NULL,
                "ButtonStyle" character varying(100) NULL
            );
            """;
        await db.Database.ExecuteSqlRawAsync(ensureBlockLocalizationsTableSql);

        const string ensureBlockLocalizationsColumnsSql = """
            ALTER TABLE IF EXISTS "block_localizations"
                ADD COLUMN IF NOT EXISTS "ContentBlockId" integer NULL,
                ADD COLUMN IF NOT EXISTS "Language" character varying(10) NOT NULL DEFAULT '',
                ADD COLUMN IF NOT EXISTS "Heading" character varying(300) NULL,
                ADD COLUMN IF NOT EXISTS "Subheading" character varying(600) NULL,
                ADD COLUMN IF NOT EXISTS "Description" character varying(4000) NULL,
                ADD COLUMN IF NOT EXISTS "ContentData" jsonb NULL,
                ADD COLUMN IF NOT EXISTS "MediaUrl" character varying(2048) NULL,
                ADD COLUMN IF NOT EXISTS "MediaAltText" character varying(500) NULL,
                ADD COLUMN IF NOT EXISTS "ButtonText" character varying(300) NULL,
                ADD COLUMN IF NOT EXISTS "ButtonLink" character varying(2048) NULL,
                ADD COLUMN IF NOT EXISTS "ButtonStyle" character varying(100) NULL;
            """;
        await db.Database.ExecuteSqlRawAsync(ensureBlockLocalizationsColumnsSql);

        const string ensureBlockLocalizationsIndexesAndFkSql = """
            CREATE INDEX IF NOT EXISTS "IX_block_localizations_ContentBlockId"
                ON "block_localizations" ("ContentBlockId");
            CREATE INDEX IF NOT EXISTS "IX_block_localizations_Language"
                ON "block_localizations" ("Language");
            CREATE UNIQUE INDEX IF NOT EXISTS "IX_block_localizations_ContentBlockId_Language"
                ON "block_localizations" ("ContentBlockId", "Language");

            DO $$
            BEGIN
                IF EXISTS (
                    SELECT 1
                    FROM information_schema.columns
                    WHERE table_schema = 'public'
                      AND table_name = 'block_localizations'
                      AND column_name = 'ContentBlockId'
                )
                AND EXISTS (
                    SELECT 1
                    FROM information_schema.tables
                    WHERE table_schema = 'public'
                      AND table_name = 'content_blocks'
                )
                AND NOT EXISTS (
                    SELECT 1
                    FROM information_schema.table_constraints
                    WHERE table_schema = 'public'
                      AND table_name = 'block_localizations'
                      AND constraint_name = 'FK_block_localizations_content_blocks_ContentBlockId'
                ) THEN
                    ALTER TABLE "block_localizations"
                        ADD CONSTRAINT "FK_block_localizations_content_blocks_ContentBlockId"
                        FOREIGN KEY ("ContentBlockId")
                        REFERENCES "content_blocks" ("Id")
                        ON DELETE CASCADE;
                END IF;
            END $$;
            """;
        await db.Database.ExecuteSqlRawAsync(ensureBlockLocalizationsIndexesAndFkSql);

        logger.LogInformation("Ensured dynamic pages compatibility schema.");
    }

    private static async Task EnsureCategoryMedicalCompatibilitySchemaAsync(MedicalDbContext db, ILogger logger)
    {
        const string ensureCategoryTableSql = """
            CREATE TABLE IF NOT EXISTS category_medical (
                "Id" integer GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY,
                "NameAr" character varying(500) NOT NULL,
                "NameEn" character varying(500) NOT NULL,
                "Description" character varying(4000) NULL,
                "DisplayOrder" integer NOT NULL DEFAULT 0,
                "IsActive" boolean NOT NULL DEFAULT TRUE,
                "CreatedAt" timestamp with time zone NOT NULL DEFAULT NOW(),
                "UpdatedAt" timestamp with time zone NULL,
                "CreatedByUserId" character varying(450) NULL
            );
            """;
        await db.Database.ExecuteSqlRawAsync(ensureCategoryTableSql);

        const string ensureCategoryIndexesSql = """
            CREATE INDEX IF NOT EXISTS "IX_category_medical_DisplayOrder"
                ON category_medical ("DisplayOrder");
            CREATE INDEX IF NOT EXISTS "IX_category_medical_IsActive"
                ON category_medical ("IsActive");
            """;
        await db.Database.ExecuteSqlRawAsync(ensureCategoryIndexesSql);

        const string ensureCategoryMedicalIdColumnSql = """
            ALTER TABLE IF EXISTS medical_tests
                ADD COLUMN IF NOT EXISTS "CategoryMedicalId" integer NULL;
            """;
        await db.Database.ExecuteSqlRawAsync(ensureCategoryMedicalIdColumnSql);

        const string migrateCategoryDataSql = """
            DO $$
            BEGIN
                IF EXISTS (
                    SELECT 1
                    FROM information_schema.columns
                    WHERE table_schema = 'public'
                      AND table_name = 'medical_tests'
                      AND column_name = 'Category'
                ) THEN
                    INSERT INTO category_medical ("NameAr", "NameEn", "DisplayOrder", "IsActive", "CreatedAt")
                    SELECT DISTINCT
                        NULLIF(BTRIM("Category"), ''),
                        NULLIF(BTRIM("Category"), ''),
                        0,
                        TRUE,
                        NOW() AT TIME ZONE 'UTC'
                    FROM medical_tests
                    WHERE "Category" IS NOT NULL
                      AND BTRIM("Category") <> ''
                      AND NOT EXISTS (
                          SELECT 1
                          FROM category_medical cm
                          WHERE cm."NameEn" = BTRIM(medical_tests."Category")
                      );

                    INSERT INTO category_medical ("NameAr", "NameEn", "DisplayOrder", "IsActive", "CreatedAt")
                    SELECT 'غير مصنف', 'Uncategorized', 0, TRUE, NOW() AT TIME ZONE 'UTC'
                    WHERE NOT EXISTS (SELECT 1 FROM category_medical);

                    UPDATE medical_tests mt
                    SET "CategoryMedicalId" = cm."Id"
                    FROM category_medical cm
                    WHERE mt."CategoryMedicalId" IS NULL
                      AND BTRIM(mt."Category") <> ''
                      AND cm."NameEn" = BTRIM(mt."Category");

                    UPDATE medical_tests
                    SET "CategoryMedicalId" = (SELECT "Id" FROM category_medical ORDER BY "Id" LIMIT 1)
                    WHERE "CategoryMedicalId" IS NULL;

                    ALTER TABLE medical_tests DROP COLUMN "Category";
                END IF;
            END $$;
            """;
        await db.Database.ExecuteSqlRawAsync(migrateCategoryDataSql);

        const string ensureCategoryMedicalIdNotNullSql = """
            DO $$
            BEGIN
                IF EXISTS (
                    SELECT 1
                    FROM information_schema.columns
                    WHERE table_schema = 'public'
                      AND table_name = 'medical_tests'
                      AND column_name = 'CategoryMedicalId'
                      AND is_nullable = 'YES'
                ) THEN
                    UPDATE medical_tests
                    SET "CategoryMedicalId" = (SELECT "Id" FROM category_medical ORDER BY "Id" LIMIT 1)
                    WHERE "CategoryMedicalId" IS NULL;

                    ALTER TABLE medical_tests
                        ALTER COLUMN "CategoryMedicalId" SET NOT NULL;
                END IF;
            END $$;
            """;
        await db.Database.ExecuteSqlRawAsync(ensureCategoryMedicalIdNotNullSql);

        const string ensureMedicalTestCategoryIndexesAndFkSql = """
            CREATE INDEX IF NOT EXISTS "IX_medical_tests_CategoryMedicalId"
                ON medical_tests ("CategoryMedicalId");

            DO $$
            BEGIN
                IF EXISTS (
                    SELECT 1
                    FROM information_schema.columns
                    WHERE table_schema = 'public'
                      AND table_name = 'medical_tests'
                      AND column_name = 'CategoryMedicalId'
                )
                AND EXISTS (
                    SELECT 1
                    FROM information_schema.tables
                    WHERE table_schema = 'public'
                      AND table_name = 'category_medical'
                )
                AND NOT EXISTS (
                    SELECT 1
                    FROM information_schema.table_constraints
                    WHERE table_schema = 'public'
                      AND table_name = 'medical_tests'
                      AND constraint_name = 'FK_medical_tests_category_medical_CategoryMedicalId'
                ) THEN
                    ALTER TABLE medical_tests
                        ADD CONSTRAINT "FK_medical_tests_category_medical_CategoryMedicalId"
                        FOREIGN KEY ("CategoryMedicalId")
                        REFERENCES category_medical ("Id")
                        ON DELETE RESTRICT;
                END IF;
            END $$;
            """;
        await db.Database.ExecuteSqlRawAsync(ensureMedicalTestCategoryIndexesAndFkSql);

        const string normalizeMedicalTestStatusSql = """
            DO $$
            BEGIN
                IF EXISTS (
                    SELECT 1
                    FROM information_schema.columns
                    WHERE table_schema = 'public'
                      AND table_name = 'medical_tests'
                      AND column_name = 'Status'
                      AND character_maximum_length = 64
                ) THEN
                    UPDATE medical_tests
                    SET "Status" = CASE
                        WHEN LOWER(BTRIM("Status")) IN ('confirm', 'confirmed', 'active') THEN 'Confirm'
                        WHEN LOWER(BTRIM("Status")) IN ('cancel', 'cancelled', 'canceled', 'archived') THEN 'Cancel'
                        ELSE 'Pending'
                    END;

                    ALTER TABLE medical_tests
                        ALTER COLUMN "Status" TYPE character varying(32);
                END IF;
            END $$;
            """;
        await db.Database.ExecuteSqlRawAsync(normalizeMedicalTestStatusSql);

        logger.LogInformation("Ensured category_medical compatibility schema.");
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
            var alterColumnOperations = migration.UpOperations.OfType<AlterColumnOperation>().ToList();
            if (createTableOperations.Count == 0
                && addColumnOperations.Count == 0
                && alterColumnOperations.Count == 0)
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

            foreach (var operation in alterColumnOperations)
            {
                var isNullable = await ColumnIsNullableAsync(db, operation.Table, operation.Name, operation.Schema);
                if (isNullable is not null && isNullable == operation.IsNullable)
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

    private static async Task<bool?> ColumnIsNullableAsync(
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
            SELECT is_nullable
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
        if (scalar is null || scalar is DBNull)
            return null;

        return string.Equals(scalar.ToString(), "YES", StringComparison.OrdinalIgnoreCase);
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
