namespace CRM.Medical.Application.Features.Notifications.Services;

public static class NotificationAudienceType
{
    public const string All = "All";
    public const string Role = "Role";
    public const string User = "User";

    public static readonly IReadOnlySet<string> AllValues = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        All,
        Role,
        User
    };
}
