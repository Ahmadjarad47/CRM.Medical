namespace CRM.Medical.API.Controllers.Admin.Models;

public sealed record GetUsersRequest
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string? Search { get; init; }
    public bool? IsActive { get; init; }
    public string? Role { get; init; }
    public string SortBy { get; init; } = "FullName";
    public bool SortDesc { get; init; } = false;
}


public sealed record CreateUserRequest(
    string Email,
    string Password,
    string FullName,
    string? City,
    string? PhoneNumber,
    IReadOnlyList<string>? Roles,
    object? ProfileMetadata);

public sealed record UpdateUserRequest(
    string FullName,
    string? City,
    string? PhoneNumber,
    object? ProfileMetadata);

public sealed record AssignRolesRequest(IReadOnlyList<string> Roles);

public sealed record RemoveRolesRequest(IReadOnlyList<string> Roles);

public sealed record AssignPermissionsRequest(IReadOnlyList<string> Permissions);

public sealed record ReplacePermissionsRequest(IReadOnlyList<string>? Permissions);
