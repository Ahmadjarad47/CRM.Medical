namespace CRM.Medical.API.Contracts.MedicalWorkflow;

public sealed class SaveAvailabilityRequest
{
    /// <summary>Optional for admin assignment; omitted for self-management.</summary>
    public string? UserId { get; set; }

    public int DayOfWeek { get; set; }

    public TimeSpan StartTime { get; set; }

    public TimeSpan EndTime { get; set; }

    public int SlotDuration { get; set; }

    public bool IsActive { get; set; } = true;
}
