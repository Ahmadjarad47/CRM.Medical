namespace CRM.Medical.Application.Features.Notifications.Services;

public interface IFirebasePushSender
{
    Task<IReadOnlyList<FirebasePushSendResult>> SendAsync(
        IReadOnlyList<string> tokens,
        string title,
        string body,
        IReadOnlyDictionary<string, string>? data,
        CancellationToken cancellationToken);
}
