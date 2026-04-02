namespace CRM.Medical.Application.Auth;

public sealed record AuthenticatedUser(
    string Id,
    string Email,
    string FullName,
    IReadOnlyList<string> Roles,
    IReadOnlyList<string> Permissions);
