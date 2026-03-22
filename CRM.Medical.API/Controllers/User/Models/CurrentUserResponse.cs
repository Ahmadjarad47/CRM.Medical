namespace CRM.Medical.API.Controllers.User.Models;

public sealed record CurrentUserResponse(string UserId, string? Email);
