namespace CRM.Medical.Application.Features.Complaints;

public static class ComplaintStatuses
{
    public const string Pending = "Pending";
    public const string InReview = "InReview";
    public const string Resolved = "Resolved";
    public const string Rejected = "Rejected";

    public static readonly IReadOnlyList<string> All =
    [
        Pending,
        InReview,
        Resolved,
        Rejected
    ];
}
