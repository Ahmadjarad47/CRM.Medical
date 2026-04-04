namespace CRM.Medical.Application.Features.Appointments;

public static class AppointmentStatuses
{
    public const string Pending = "Pending";
    public const string Confirmed = "Confirmed";
    public const string Cancelled = "Cancelled";

    public static readonly IReadOnlyList<string> All = [Pending, Confirmed, Cancelled];
}
