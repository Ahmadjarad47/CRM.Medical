namespace CRM.Medical.Application.Features.Users.DTOs;

public sealed record UserSummaryDto(
    string Id,
    string Email,
    string FullName,
    string? City,
    string? PhoneNumber,
    bool IsActive,
    bool EmailConfirmed,
    DateTime CreatedAt);
