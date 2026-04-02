using System.Text.Json;

namespace CRM.Medical.Application.Features.Users.DTOs;

public sealed record UserDetailDto(
    string Id,
    string Email,
    string FullName,
    string? City,
    string? PhoneNumber,
    bool IsActive,
    bool EmailConfirmed,
    bool LockoutEnabled,
    DateTimeOffset? LockoutEnd,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    IReadOnlyList<string> Roles,
    JsonElement? ProfileMetadata);
