namespace CRM.Medical.Application.Features.Users.Services;

public interface IEmailVerificationSender
{
    Task SendAsync(string email, string verificationToken, CancellationToken ct = default);
}
