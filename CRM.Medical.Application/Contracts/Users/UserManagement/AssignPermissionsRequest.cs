namespace CRM.Medical.API.Contracts.Users.UserManagement;

public sealed record AssignPermissionsRequest(IReadOnlyList<string> Permissions);
