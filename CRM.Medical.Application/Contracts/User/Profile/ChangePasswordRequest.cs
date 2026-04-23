namespace CRM.Medical.API.Contracts.User.Profile;

public sealed record ChangePasswordRequest(string CurrentPassword, string NewPassword);
