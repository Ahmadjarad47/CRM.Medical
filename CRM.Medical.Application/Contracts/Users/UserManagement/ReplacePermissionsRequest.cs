namespace CRM.Medical.API.Contracts.Users.UserManagement;

public sealed record ReplacePermissionsRequest(IReadOnlyList<string>? Permissions);
