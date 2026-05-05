namespace CRM.Medical.Application.Authorization;

public sealed record AuthorizationDecision(bool IsAllowed, string Reason)
{
    public static AuthorizationDecision Allow(string reason) => new(true, reason);

    public static AuthorizationDecision Deny(string reason) => new(false, reason);
}
