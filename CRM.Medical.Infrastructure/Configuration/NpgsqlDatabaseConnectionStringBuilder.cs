using CRM.Medical.Application.Configuration.Database;
using Npgsql;

namespace CRM.Medical.Infrastructure.Configuration;

public sealed class NpgsqlDatabaseConnectionStringBuilder : IDatabaseConnectionStringBuilder
{
    public string Build(DatabaseSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);

        if (settings.Multiplexing && !settings.Pooling)
            throw new InvalidOperationException("Multiplexing requires connection pooling (DB_POOLING=true).");

        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = settings.Host,
            Port = settings.Port,
            Database = settings.Name,
            Username = settings.User,
            Password = settings.Password,
            Pooling = settings.Pooling,
            MaxPoolSize = settings.MaxPoolSize,
            Timeout = settings.Timeout,
            CommandTimeout = settings.CommandTimeout,
            Multiplexing = settings.Multiplexing,
        };

        if (!Enum.TryParse<SslMode>(settings.SslMode, ignoreCase: true, out var sslMode))
            throw new InvalidOperationException($"Invalid DB_SSL_MODE value: '{settings.SslMode}'.");

        builder.SslMode = sslMode;

        return builder.ConnectionString;
    }
}
