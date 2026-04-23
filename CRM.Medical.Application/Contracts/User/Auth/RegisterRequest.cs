namespace CRM.Medical.API.Contracts.User.Auth;

public sealed record RegisterRequest(
    string Email,
    string Password,
    string FullName,
    string? City,
    string? PhoneNumber,
    string Role);
