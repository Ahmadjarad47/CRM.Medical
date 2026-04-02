using CRM.Medical.Application.Features.Users.Services;
using Microsoft.Extensions.Logging;

namespace CRM.Medical.Infrastructure.Email;

/// <summary>
/// Stub — logs the deletion token to console.
/// Replace with a real email provider (SMTP / SendGrid / etc.) in production.
/// </summary>
public sealed class LoggingAccountDeletionSender(ILogger<LoggingAccountDeletionSender> logger)
    : IAccountDeletionSender
{
    public Task SendAsync(string email, string deletionToken, CancellationToken ct = default)
    {
        logger.LogWarning(
            "[EMAIL STUB] Account deletion token for {Email}: {Token}",
            email, deletionToken);
        return Task.CompletedTask;
    }
}
