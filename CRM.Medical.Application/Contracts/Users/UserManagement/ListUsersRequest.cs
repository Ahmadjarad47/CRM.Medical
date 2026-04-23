namespace CRM.Medical.API.Contracts.Users.UserManagement;

/// <summary>Query parameters for listing users (scoped by role: Admin sees all; Doctor/Lab see only allowed users).</summary>
public sealed record ListUsersRequest
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string? Search { get; init; }
    public bool? IsActive { get; init; }
    public string? Role { get; init; }
    public string SortBy { get; init; } = "FullName";
    public bool SortDesc { get; init; }
}
