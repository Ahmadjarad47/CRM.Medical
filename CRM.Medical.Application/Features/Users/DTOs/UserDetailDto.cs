namespace CRM.Medical.Application.Features.Users.DTOs;

public sealed record UserDetailDto(
    string Id,
    string Email,
    string DisplayName,
    bool EmailConfirmed,
    bool LockoutEnabled,
    DateTimeOffset? LockoutEnd,
    bool IsLocked);
