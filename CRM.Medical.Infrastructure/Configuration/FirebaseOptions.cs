namespace CRM.Medical.Infrastructure.Configuration;

public sealed class FirebaseOptions
{
    public const string SectionName = "Firebase";

    public string? ProjectId { get; init; }

    public string? CredentialsPath { get; init; }

    public string? CredentialsJson { get; init; }

    public string? WebPushVapidKey { get; init; }
}
