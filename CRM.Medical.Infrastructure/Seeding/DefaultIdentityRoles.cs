namespace CRM.Medical.Infrastructure.Seeding;

public static class DefaultIdentityRoles
{
    public const string Patient = "Patient";
    public const string Lab = "Lab";
    public const string Admin = "Admin";
    public const string Doctor = "Doctor";
    public const string Special = "Special";

    public static readonly string[] All = [Patient, Doctor, Lab, Admin, Special];
}
