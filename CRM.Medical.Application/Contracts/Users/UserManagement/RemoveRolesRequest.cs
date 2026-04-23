namespace CRM.Medical.API.Contracts.Users.UserManagement;

public sealed record RemoveRolesRequest(IReadOnlyList<string> Roles);
