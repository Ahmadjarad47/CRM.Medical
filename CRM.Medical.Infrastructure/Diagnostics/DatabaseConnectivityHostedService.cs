using CRM.Medical.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CRM.Medical.Infrastructure.Diagnostics;

public sealed class DatabaseConnectivityHostedService : IHostedService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<DatabaseConnectivityHostedService> _logger;
    private readonly DatabaseConnectionReport _report;

    public DatabaseConnectivityHostedService(
        IServiceScopeFactory scopeFactory,
        ILogger<DatabaseConnectivityHostedService> logger,
        DatabaseConnectionReport report)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _report = report;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<MedicalDbContext>();

        _report.Verified = true;

        try
        {
            await db.Database.ExecuteSqlRawAsync("SELECT 1", cancellationToken);
            _report.Success = true;
            _report.ErrorMessage = null;
            _logger.LogInformation("Database connection OK. PostgreSQL responded successfully.");
        }
        catch (Exception ex)
        {
            _report.Success = false;
            _report.ErrorMessage = ex.Message;
            _logger.LogError(ex, "Database connection failed. Verify DB_* environment variables and that PostgreSQL is running.");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
