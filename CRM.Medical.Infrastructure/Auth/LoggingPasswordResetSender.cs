using CRM.Medical.Application.Features.Users.Services;
using Microsoft.Extensions.Logging;

namespace CRM.Medical.Infrastructure.Auth;

public sealed class LoggingPasswordResetSender(ILogger<LoggingPasswordResetSender> logger) : IPasswordResetSender
{
    public Task SendAsync(string email, string resetLink, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Password reset link for {Email}: {ResetLink}",
            email,
            resetLink);

        return Task.CompletedTask;
    }
}
