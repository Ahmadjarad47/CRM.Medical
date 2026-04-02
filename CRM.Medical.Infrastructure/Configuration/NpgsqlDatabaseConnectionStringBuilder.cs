using System.Text;
using CRM.Medical.Application.Configuration.Database;

namespace CRM.Medical.Infrastructure.Configuration;

public sealed class NpgsqlDatabaseConnectionStringBuilder : IDatabaseConnectionStringBuilder
{
    public string Build(DatabaseSettings settings)
    {
        var sb = new StringBuilder();
        sb.Append($"Host={settings.Host};");
        sb.Append($"Port={settings.Port};");
        sb.Append($"Database={settings.Database};");
        sb.Append($"Username={settings.Username};");
        sb.Append($"Password={settings.Password};");
        sb.Append($"Pooling={settings.Pooling};");
        sb.Append($"Maximum Pool Size={settings.MaxPoolSize};");
        sb.Append($"Timeout={settings.Timeout};");
        sb.Append($"Command Timeout={settings.CommandTimeout};");
        sb.Append($"SSL Mode={settings.SslMode};");
        sb.Append($"Multiplexing={settings.Multiplexing}");
        return sb.ToString();
    }
}
