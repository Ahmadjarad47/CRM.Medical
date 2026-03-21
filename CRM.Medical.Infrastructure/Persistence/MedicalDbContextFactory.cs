using System.Text;
using CRM.Medical.Application.Configuration.Database;
using CRM.Medical.Infrastructure.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace CRM.Medical.Infrastructure.Persistence;

/// <summary>
/// EF Core design-time factory (CLI, Visual Studio <c>Add-Migration</c>, etc.).
/// Resolution order:
/// <list type="number">
/// <item><description><c>DOTNET_EF_CONNECTION_STRING</c> if set (full Npgsql connection string).</description></item>
/// <item><description>Build from <c>DB_*</c> environment variables when all are set (same as the running API).</description></item>
/// <item><description><c>appsettings.ef.json</c> in the Infrastructure project folder (or current directory) with <c>ConnectionStrings:DesignTime</c> — use for remote servers when env vars are not set in the IDE.</description></item>
/// </list>
/// </summary>
public sealed class MedicalDbContextFactory : IDesignTimeDbContextFactory<MedicalDbContext>
{
    private const string DesignTimeConnectionName = "DesignTime";

    public MedicalDbContext CreateDbContext(string[] args)
    {
        var connectionString = ResolveDesignTimeConnectionString();

        var options = new DbContextOptionsBuilder<MedicalDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new MedicalDbContext(options);
    }

    private static string ResolveDesignTimeConnectionString()
    {
        var fromTooling = Environment.GetEnvironmentVariable("DOTNET_EF_CONNECTION_STRING");
        if (!string.IsNullOrWhiteSpace(fromTooling))
            return fromTooling;

        try
        {
            var settings = new DatabaseSettings();
            new DatabaseSettingsFromEnvironmentConfigurer().Configure(settings);
            return new NpgsqlDatabaseConnectionStringBuilder().Build(settings);
        }
        catch (InvalidOperationException)
        {
            var fromFile = TryLoadConnectionStringFromDesignTimeJson();
            if (!string.IsNullOrWhiteSpace(fromFile))
                return fromFile;

            throw new InvalidOperationException(BuildMissingConfigurationMessage());
        }
    }

    private static string? TryLoadConnectionStringFromDesignTimeJson()
    {
        foreach (var path in GetCandidateEfJsonPaths())
        {
            if (!File.Exists(path))
                continue;

            var configuration = new ConfigurationBuilder()
                .AddJsonFile(path, optional: false)
                .Build();

            var connectionString = configuration.GetConnectionString(DesignTimeConnectionName);
            if (!string.IsNullOrWhiteSpace(connectionString))
                return connectionString;
        }

        return null;
    }

    private static IEnumerable<string> GetCandidateEfJsonPaths()
    {
        var fileName = "appsettings.ef.json";

        var fromAssembly = GetInfrastructureProjectDirectoryFromAssembly();
        if (fromAssembly is not null)
            yield return Path.Combine(fromAssembly, fileName);

        var cwd = Directory.GetCurrentDirectory();
        yield return Path.Combine(cwd, fileName);
        yield return Path.Combine(cwd, "CRM.Medical.Infrastructure", fileName);
    }

    private static string? GetInfrastructureProjectDirectoryFromAssembly()
    {
        var location = typeof(MedicalDbContextFactory).Assembly.Location;
        if (string.IsNullOrEmpty(location))
            return null;

        // .../CRM.Medical.Infrastructure/bin/Debug/net10.0/*.dll -> project folder
        var dllDir = Path.GetDirectoryName(location);
        if (string.IsNullOrEmpty(dllDir))
            return null;

        var projectRoot = Path.GetFullPath(Path.Combine(dllDir, "..", "..", ".."));
        return Directory.Exists(projectRoot) ? projectRoot : null;
    }

    private static string BuildMissingConfigurationMessage()
    {
        var sb = new StringBuilder();
        sb.AppendLine("EF Core design-time database connection is not configured.");
        sb.AppendLine();
        sb.AppendLine("Your database is remote (not on this machine). Use one of:");
        sb.AppendLine("  1) Set environment variable DOTNET_EF_CONNECTION_STRING to your full Npgsql connection string.");
        sb.AppendLine("  2) Set all DB_* variables (DB_HOST, DB_PORT, ...) like when running the API.");
        sb.AppendLine("  3) Copy CRM.Medical.Infrastructure/appsettings.ef.example.json to appsettings.ef.json");
        sb.AppendLine("     in the same folder and set ConnectionStrings:DesignTime to your remote server.");
        sb.AppendLine();
        sb.AppendLine("appsettings.ef.json is gitignored — safe for local credentials.");
        return sb.ToString();
    }
}
