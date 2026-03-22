namespace CRM.Medical.Application.Features.Users.Services;

public interface IPasswordResetSender
{
    Task SendAsync(string email, string resetLink, CancellationToken cancellationToken);
}
