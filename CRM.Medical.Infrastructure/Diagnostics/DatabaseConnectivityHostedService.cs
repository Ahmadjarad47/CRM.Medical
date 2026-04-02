using CRM.Medical.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CRM.Medical.Infrastructure.Diagnostics;

public sealed class DatabaseConnectivityHostedService(
    IServiceProvider services,
    DatabaseConnectionReport report,
    ILogger<DatabaseConnectivityHostedService> logger)
    : BackgroundService
{
    private static readonly TimeSpan CheckInterval = TimeSpan.FromSeconds(30);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = services.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<MedicalDbContext>();
                await dbContext.Database.ExecuteSqlRawAsync("SELECT 1", stoppingToken);
                report.ReportSuccess();
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                report.ReportFailure(ex.Message);
                logger.LogWarning(ex, "Database connectivity check failed");
            }

            await Task.Delay(CheckInterval, stoppingToken).ConfigureAwait(ConfigureAwaitOptions.SuppressThrowing);
        }
    }
}
