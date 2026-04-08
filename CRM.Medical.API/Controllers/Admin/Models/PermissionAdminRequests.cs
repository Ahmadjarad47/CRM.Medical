namespace CRM.Medical.API.Controllers.Admin.Models;

public sealed record CreatePermissionRequest(string Name, string? Description);

public sealed record UpdatePermissionRequest(string? Description);
