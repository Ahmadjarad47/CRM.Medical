namespace CRM.Medical.Application.Features.Users.DTOs;

public sealed record UserSummaryDto(
    string Id,
    string Email,
    string DisplayName,
    bool EmailConfirmed,
    bool LockoutEnabled,
    bool IsLocked);
