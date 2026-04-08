namespace CRM.Medical.Domain.Constants;

/// <summary>Lifecycle values for <see cref="Entities.MedicalTest"/>.</summary>
public static class MedicalTestStatuses
{
    public const string Draft = "Draft";
    public const string Active = "Active";
    public const string Archived = "Archived";
    public const string Cancelled = "Cancelled";

    public static readonly IReadOnlyList<string> All = [Draft, Active, Archived, Cancelled];
}

/// <summary>Lifecycle values for <see cref="Entities.TestRequest"/>.</summary>
public static class TestRequestStatuses
{
    public const string Pending = "Pending";
    public const string InProgress = "InProgress";
    public const string Completed = "Completed";
    public const string Cancelled = "Cancelled";

    public static readonly IReadOnlyList<string> All = [Pending, InProgress, Completed, Cancelled];
}

/// <summary>Lifecycle values for <see cref="Entities.TestResult"/>.</summary>
public static class TestResultStatuses
{
    public const string Draft = "Draft";
    public const string Finalized = "Finalized";
    public const string Delivered = "Delivered";
    public const string Cancelled = "Cancelled";

    public static readonly IReadOnlyList<string> All = [Draft, Finalized, Delivered, Cancelled];
}

/// <summary>
/// When an appointment is linked to a medical test, tracks progress of that test portion (null when no test is linked).
/// </summary>
public static class AppointmentMedicalTestCompletionStatuses
{
    public const string NotStarted = "NotStarted";
    public const string InProgress = "InProgress";
    public const string Finished = "Finished";
    public const string Cancelled = "Cancelled";

    public static readonly IReadOnlyList<string> All = [NotStarted, InProgress, Finished, Cancelled];
}
