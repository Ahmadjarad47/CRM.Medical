namespace CRM.Medical.API.Contracts.Users.UserManagement;

public sealed record CreateManagedUserRequest(
    string Email,
    string Password,
    string FullName,
    string? City,
    string? PhoneNumber,
    IReadOnlyList<string>? Roles,
    object? ProfileMetadata);
