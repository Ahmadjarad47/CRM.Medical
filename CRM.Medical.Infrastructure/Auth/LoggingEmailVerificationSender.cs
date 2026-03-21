using CRM.Medical.Application.Features.Users.Services;
using Microsoft.Extensions.Logging;

namespace CRM.Medical.Infrastructure.Auth;

public sealed class LoggingEmailVerificationSender(ILogger<LoggingEmailVerificationSender> logger)
    : IEmailVerificationSender
{
    public Task SendAsync(string email, string confirmationLink, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Email confirmation link for {Email}: {ConfirmationLink}",
            email,
            confirmationLink);

        return Task.CompletedTask;
    }
}
