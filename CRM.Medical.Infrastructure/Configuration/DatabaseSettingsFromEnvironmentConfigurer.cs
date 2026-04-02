using CRM.Medical.Application.Configuration.Database;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace CRM.Medical.Infrastructure.Configuration;

public sealed class DatabaseSettingsFromEnvironmentConfigurer(IConfiguration configuration)
    : IConfigureOptions<DatabaseSettings>
{
    public void Configure(DatabaseSettings options)
    {
        options.Host = configuration["DB_HOST"] ?? string.Empty;
        options.Port = int.TryParse(configuration["DB_PORT"], out var port) ? port : 5432;
        options.Database = configuration["DB_NAME"] ?? string.Empty;
        options.Username = configuration["DB_USER"] ?? string.Empty;
        options.Password = configuration["DB_PASSWORD"] ?? string.Empty;
        options.Pooling = !bool.TryParse(configuration["DB_POOLING"], out var pooling) || pooling;
        options.MaxPoolSize = int.TryParse(configuration["DB_MAX_POOL_SIZE"], out var maxPool) ? maxPool : 100;
        options.Timeout = int.TryParse(configuration["DB_TIMEOUT"], out var timeout) ? timeout : 15;
        options.CommandTimeout = int.TryParse(configuration["DB_COMMAND_TIMEOUT"], out var cmdTimeout) ? cmdTimeout : 30;
        options.SslMode = configuration["DB_SSL_MODE"] ?? "Disable";
        options.Multiplexing = bool.TryParse(configuration["DB_MULTIPLEXING"], out var multiplexing) && multiplexing;
    }
}
