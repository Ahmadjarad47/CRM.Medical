using CRM.Medical.Application.Features.Users.Services;
using Microsoft.Extensions.Logging;

namespace CRM.Medical.Infrastructure.Email;

/// <summary>
/// Stub implementation — logs tokens instead of sending real emails.
/// Replace with a real SMTP / SendGrid / etc. implementation in production.
/// </summary>
public sealed class LoggingPasswordResetSender(ILogger<LoggingPasswordResetSender> logger)
    : IPasswordResetSender
{
    public Task SendAsync(string email, string resetToken, CancellationToken ct = default)
    {
        logger.LogInformation(
            "[EMAIL STUB] Password reset token for {Email}: {Token}",
            email, resetToken);
        return Task.CompletedTask;
    }
}
