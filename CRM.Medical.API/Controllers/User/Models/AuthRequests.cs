namespace CRM.Medical.API.Controllers.User.Models;

public sealed record LoginRequest(string Email, string Password);

public sealed record RefreshTokenRequest(string Token);

public sealed record RegisterRequest(
    string Email,
    string Password,
    string FullName,
    string? City,
    string? PhoneNumber,
    string Role);

public sealed record ForgotPasswordRequest(string Email);

public sealed record ResetPasswordRequest(string Email, string Token, string NewPassword);

public sealed record VerifyEmailRequest(string Email, string Token);

public sealed record ResendVerificationRequest(string Email);

public sealed record ConfirmAccountDeletionRequest(string Email, string Token);
