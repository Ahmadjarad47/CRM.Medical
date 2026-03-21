namespace CRM.Medical.API.Endpoints.Auth.Models;

public sealed record CurrentUserResponse(string UserId, string? Email);
