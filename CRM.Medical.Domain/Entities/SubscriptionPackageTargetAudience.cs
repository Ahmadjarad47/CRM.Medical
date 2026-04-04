namespace CRM.Medical.Domain.Entities;

/// <summary>
/// Which user category may purchase or use this subscription package.
/// <see cref="All"/> means any authenticated user type.
/// </summary>
public enum SubscriptionPackageTargetAudience
{
    All = 0,
    Patient = 1,
    Doctor = 2,
    LabPartner = 3
}
