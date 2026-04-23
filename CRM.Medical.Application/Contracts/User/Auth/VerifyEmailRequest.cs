namespace CRM.Medical.API.Contracts.User.Auth;

public sealed record VerifyEmailRequest(string Email, string Token);
