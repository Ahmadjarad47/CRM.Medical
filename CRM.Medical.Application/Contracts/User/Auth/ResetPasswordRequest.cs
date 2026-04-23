namespace CRM.Medical.API.Contracts.User.Auth;

public sealed record ResetPasswordRequest(string Email, string Token, string NewPassword);
