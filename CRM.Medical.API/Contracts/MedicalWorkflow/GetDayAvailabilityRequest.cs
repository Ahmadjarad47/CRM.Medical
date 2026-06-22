namespace CRM.Medical.API.Contracts.MedicalWorkflow;

public sealed class GetDayAvailabilityRequest
{
    public DateOnly Date { get; set; }

    /// <summary>Optional for admin to inspect another provider.</summary>
    public string? UserId { get; set; }
}
