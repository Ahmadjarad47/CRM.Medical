using CRM.Medical.Domain.Entities;

namespace CRM.Medical.Application.Features.Users.DTOs;

public sealed record UserSummaryDto(
    string Id,
    string Email,
    string DisplayName,
    UserType UserType,
    bool EmailConfirmed,
    bool LockoutEnabled,
    bool IsLocked);
