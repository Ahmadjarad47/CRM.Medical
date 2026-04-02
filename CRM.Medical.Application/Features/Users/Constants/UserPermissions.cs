namespace CRM.Medical.Application.Features.Users.Constants;

/// <summary>
/// User-level permission constants stored as ASP.NET Core Identity claims.
/// ClaimType = "permission", ClaimValue = one of the constants below.
/// Authorization is claim-based (not role-based): each endpoint requires a specific permission claim.
/// </summary>
public static class UserPermissions
{
    public const string ClaimType = "permission";

    public const string UsersView = "users.view";
    public const string UsersCreate = "users.create";
    public const string UsersUpdate = "users.update";
    public const string UsersDelete = "users.delete";
    public const string UsersManagePermissions = "users.manage_permissions";

    public static readonly IReadOnlyList<string> All =
    [
        UsersView,
        UsersCreate,
        UsersUpdate,
        UsersDelete,
        UsersManagePermissions
    ];
}
