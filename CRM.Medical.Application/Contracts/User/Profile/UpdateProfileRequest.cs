namespace CRM.Medical.API.Contracts.User.Profile;

public sealed record UpdateProfileRequest(
    string FullName,
    string? City,
    string? PhoneNumber,
    object? ProfileMetadata);
