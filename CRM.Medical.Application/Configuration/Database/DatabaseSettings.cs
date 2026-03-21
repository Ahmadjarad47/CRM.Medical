namespace CRM.Medical.Application.Configuration.Database;

public sealed class DatabaseSettings
{
    public string Host { get; set; } = string.Empty;

    public int Port { get; set; }

    public string Name { get; set; } = string.Empty;

    public string User { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public bool Pooling { get; set; }

    public int MaxPoolSize { get; set; }

    public int Timeout { get; set; }

    public int CommandTimeout { get; set; }

    public string SslMode { get; set; } = string.Empty;

    public bool Multiplexing { get; set; }
}
