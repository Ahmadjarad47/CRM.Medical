namespace CRM.Medical.Application.Features.Users.Services;

public interface IAccountDeletionSender
{
    Task SendAsync(string email, string deletionToken, CancellationToken ct = default);
}
