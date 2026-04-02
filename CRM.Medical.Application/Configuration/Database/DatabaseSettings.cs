namespace CRM.Medical.Application.Configuration.Database;

public sealed class DatabaseSettings
{
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; } = 5432;
    public string Database { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool Pooling { get; set; } = true;
    public int MaxPoolSize { get; set; } = 100;
    public int Timeout { get; set; } = 15;
    public int CommandTimeout { get; set; } = 30;
    public string SslMode { get; set; } = "Disable";
    public bool Multiplexing { get; set; } = false;
}
