namespace CRM.Medical.API.Controllers.User.Models;

public sealed record UpdateProfileRequest(
    string FullName,
    string? City,
    string? PhoneNumber,
    object? ProfileMetadata);

public sealed record ChangePasswordRequest(string CurrentPassword, string NewPassword);
