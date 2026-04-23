namespace CRM.Medical.API.Contracts.Users.UserManagement;

public sealed record AssignRolesRequest(IReadOnlyList<string> Roles);
