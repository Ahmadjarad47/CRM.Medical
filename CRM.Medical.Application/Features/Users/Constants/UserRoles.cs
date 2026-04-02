namespace CRM.Medical.Application.Features.Users.Constants;

public static class UserRoles
{
    public const string Admin = "Admin";
    public const string Doctor = "Doctor";
    public const string Patient = "Patient";
    public const string LabPartner = "LabPartner";
    public const string User = "User";

    public static readonly IReadOnlyList<string> All = [Admin, Doctor, Patient, LabPartner, User];
}
