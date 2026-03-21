namespace CRM.Medical.Application.Features.Users.Services;

public interface IEmailVerificationSender
{
    Task SendAsync(string email, string confirmationLink, CancellationToken cancellationToken);
}
