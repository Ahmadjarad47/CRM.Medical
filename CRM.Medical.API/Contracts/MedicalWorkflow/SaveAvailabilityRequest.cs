namespace CRM.Medical.API.Contracts.MedicalWorkflow;

public sealed class SaveAvailabilityRequest
{
    public DayOfWeek DayOfWeek { get; set; }

    public TimeSpan StartTime { get; set; }

    public TimeSpan EndTime { get; set; }

    public int SlotDuration { get; set; }

    public bool IsActive { get; set; } = true;
}
