using CRM.Medical.Application.Features.Users.Services;
using Microsoft.Extensions.Logging;

namespace CRM.Medical.Infrastructure.Email;

/// <summary>
/// Stub implementation — logs tokens instead of sending real emails.
/// Replace with a real SMTP / SendGrid / etc. implementation in production.
/// </summary>
public sealed class LoggingEmailVerificationSender(ILogger<LoggingEmailVerificationSender> logger)
    : IEmailVerificationSender
{
    public Task SendAsync(string email, string verificationToken, CancellationToken ct = default)
    {
        logger.LogInformation(
            "[EMAIL STUB] Email verification token for {Email}: {Token}",
            email, verificationToken);
        return Task.CompletedTask;
    }
}
