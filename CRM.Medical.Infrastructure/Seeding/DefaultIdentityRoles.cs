namespace CRM.Medical.Infrastructure.Seeding;

public static class DefaultIdentityRoles
{
    public const string User = "User";
    public const string Admin = "Admin";
    public const string Doctor = "Doctor";

    public static readonly string[] All = [User, Admin, Doctor];
}
