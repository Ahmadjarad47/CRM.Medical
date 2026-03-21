namespace CRM.Medical.Application.Features.Users.DTOs;

public sealed record UserRolesDto(string UserId, IReadOnlyList<string> Roles);
