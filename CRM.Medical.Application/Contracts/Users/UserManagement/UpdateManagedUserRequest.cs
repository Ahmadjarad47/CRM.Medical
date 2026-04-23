namespace CRM.Medical.API.Contracts.Users.UserManagement;

public sealed record UpdateManagedUserRequest(
    string FullName,
    string? City,
    string? PhoneNumber,
    object? ProfileMetadata);
