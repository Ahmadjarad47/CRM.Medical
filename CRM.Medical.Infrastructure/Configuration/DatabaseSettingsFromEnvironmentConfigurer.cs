using System.Globalization;
using CRM.Medical.Application.Configuration.Database;
using Microsoft.Extensions.Options;

namespace CRM.Medical.Infrastructure.Configuration;

/// <summary>
/// Binds <see cref="DatabaseSettings"/> from <c>DB_*</c> environment variables (runtime DI and EF design-time via <see cref="Persistence.MedicalDbContextFactory"/>).
/// </summary>
public sealed class DatabaseSettingsFromEnvironmentConfigurer : IConfigureOptions<DatabaseSettings>
{
    public void Configure(DatabaseSettings options)
    {
        ArgumentNullException.ThrowIfNull(options);

        options.Host = Require("DB_HOST");
        options.Port = ParseInt(Require("DB_PORT"), "DB_PORT");
        options.Name = Require("DB_NAME");
        options.User = Require("DB_USER");
        options.Password = Require("DB_PASSWORD", allowEmpty: true);
        options.Pooling = ParseBool(Require("DB_POOLING"), "DB_POOLING");
        options.MaxPoolSize = ParseInt(Require("DB_MAX_POOL_SIZE"), "DB_MAX_POOL_SIZE");
        options.Timeout = ParseInt(Require("DB_TIMEOUT"), "DB_TIMEOUT");
        options.CommandTimeout = ParseInt(Require("DB_COMMAND_TIMEOUT"), "DB_COMMAND_TIMEOUT");
        options.SslMode = Require("DB_SSL_MODE");
        options.Multiplexing = ParseBool(Require("DB_MULTIPLEXING"), "DB_MULTIPLEXING");
    }

    private static string Require(string name, bool allowEmpty = false)
    {
        var value = Environment.GetEnvironmentVariable(name);
        if (value is null || (!allowEmpty && string.IsNullOrWhiteSpace(value)))
            throw new InvalidOperationException($"Required environment variable '{name}' is not set.");

        return value;
    }

    private static int ParseInt(string value, string name)
    {
        if (!int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result))
            throw new InvalidOperationException($"Environment variable '{name}' must be an integer.");

        return result;
    }

    private static bool ParseBool(string value, string name)
    {
        if (bool.TryParse(value, out var b))
            return b;

        if (string.Equals(value, "1", StringComparison.OrdinalIgnoreCase))
            return true;

        if (string.Equals(value, "0", StringComparison.OrdinalIgnoreCase))
            return false;

        throw new InvalidOperationException($"Environment variable '{name}' must be a boolean (true/false or 1/0).");
    }
}
