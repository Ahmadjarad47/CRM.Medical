namespace CRM.Medical.API.Controllers.Users.Models;

/// <summary>Query parameters for listing users (scoped by role: Admin sees all; Doctor/Lab see only allowed users).</summary>
public sealed record ListUsersQueryParameters
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string? Search { get; init; }
    public bool? IsActive { get; init; }
    public string? Role { get; init; }
    public string SortBy { get; init; } = "FullName";
    public bool SortDesc { get; init; }
}

public sealed record CreateManagedUserRequest(
    string Email,
    string Password,
    string FullName,
    string? City,
    string? PhoneNumber,
    IReadOnlyList<string>? Roles,
    object? ProfileMetadata);

public sealed record UpdateManagedUserRequest(
    string FullName,
    string? City,
    string? PhoneNumber,
    object? ProfileMetadata);

public sealed record AssignRolesBody(IReadOnlyList<string> Roles);

public sealed record RemoveRolesBody(IReadOnlyList<string> Roles);

public sealed record AssignPermissionsBody(IReadOnlyList<string> Permissions);

public sealed record ReplacePermissionsBody(IReadOnlyList<string>? Permissions);
