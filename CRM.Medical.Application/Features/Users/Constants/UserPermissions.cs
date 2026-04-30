namespace CRM.Medical.Application.Features.Users.Constants;

/// <summary>
/// Permission names used for authorization policies and JWT claims.
/// Assignments are stored in <c>role_permissions</c> (per ASP.NET Identity role); at login, names are aggregated
/// from the user’s roles into the access token as claims with <see cref="ClaimType"/> for dynamic permission policies on the API.
/// </summary>
public static class UserPermissions
{
    public const string ClaimType = "permission";

    public const string UsersView = "users.view";
    public const string UsersCreate = "users.create";
    public const string UsersUpdate = "users.update";
    public const string UsersDelete = "users.delete";
    public const string UsersManagePermissions = "users.manage_permissions";

    public const string RolesManage = "roles.manage";

    public const string ComplaintsView = "complaints.view";
    public const string ComplaintsUpdateStatus = "complaints.update_status";

    public const string SubscriptionsView = "subscriptions.view";
    public const string SubscriptionsManage = "subscriptions.manage";

    public const string MedicalTestRead = "MedicalTest.Read";
    public const string MedicalTestCreate = "MedicalTest.Create";
    public const string MedicalTestUpdate = "MedicalTest.Update";
    public const string MedicalTestDelete = "MedicalTest.Delete";

    public const string TestRequestRead = "TestRequest.Read";
    public const string TestRequestCreate = "TestRequest.Create";
    public const string TestRequestUpdate = "TestRequest.Update";
    public const string TestRequestDelete = "TestRequest.Delete";

    public const string TestResultRead = "TestResult.Read";
    public const string TestResultCreate = "TestResult.Create";
    public const string TestResultUpdate = "TestResult.Update";
    public const string TestResultDelete = "TestResult.Delete";

    public static readonly IReadOnlyList<string> All =
    [
        UsersView,
        UsersCreate,
        UsersUpdate,
        UsersDelete,
        UsersManagePermissions,
        RolesManage,
        ComplaintsView,
        ComplaintsUpdateStatus,
        SubscriptionsView,
        SubscriptionsManage,
        MedicalTestRead,
        MedicalTestCreate,
        MedicalTestUpdate,
        MedicalTestDelete,
        TestRequestRead,
        TestRequestCreate,
        TestRequestUpdate,
        TestRequestDelete,
        TestResultRead,
        TestResultCreate,
        TestResultUpdate,
        TestResultDelete
    ];
}
