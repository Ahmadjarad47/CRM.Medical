namespace CRM.Medical.API.Contracts.User.Auth;

public sealed record LoginRequest(string Email, string Password);
