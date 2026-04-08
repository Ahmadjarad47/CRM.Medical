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

    public const string PermissionsManage = "permissions.manage";
    public const string RolesManage = "roles.manage";

    public const string ComplaintsView = "complaints.view";
    public const string ComplaintsUpdateStatus = "complaints.update_status";

    public const string AppointmentsView = "appointments.view";
    public const string AppointmentsManage = "appointments.manage";

    public const string SubscriptionsView = "subscriptions.view";
    public const string SubscriptionsManage = "subscriptions.manage";

    public const string MedicalWorkflowView = "medical_workflow.view";

    /// <summary>
    /// Create/update/delete medical tests, test requests, results, and appointment test links (assign to doctor, lab partner, and admin as needed).
    /// </summary>
    public const string MedicalWorkflowManage = "medical_workflow.manage";

    public static readonly IReadOnlyList<string> All =
    [
        UsersView,
        UsersCreate,
        UsersUpdate,
        UsersDelete,
        UsersManagePermissions,
        PermissionsManage,
        RolesManage,
        ComplaintsView,
        ComplaintsUpdateStatus,
        AppointmentsView,
        AppointmentsManage,
        SubscriptionsView,
        SubscriptionsManage,
        MedicalWorkflowView,
        MedicalWorkflowManage
    ];
}
