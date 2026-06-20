namespace CRM.Medical.Domain.Entities;

public sealed class Availability : BaseEntity
{
    public int Id { get; set; }

    /// <summary>Doctor/LabPartner identity user id that owns this schedule.</summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>Day of week in range [0..6], where 0 = Sunday.</summary>
    public DayOfWeek DayOfWeek { get; set; }

    public TimeSpan StartTime { get; set; }

    public TimeSpan EndTime { get; set; }

    /// <summary>Appointment slot duration in minutes.</summary>
    public int SlotDuration { get; set; }

    public bool IsActive { get; set; } = true;
}
