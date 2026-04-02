namespace CRM.Medical.Application.Features.Users.DTOs;

public sealed record UserPermissionsDto(
    string UserId,
    IReadOnlyList<string> Permissions);
