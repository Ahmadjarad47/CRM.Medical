namespace CRM.Medical.API.Contracts.MedicalWorkflow;

public sealed class ListAppointmentsRequest
{
    public DateTime? FromUtc { get; set; }

    public DateTime? ToUtc { get; set; }

    public string? UserId { get; set; }

    public string? Status { get; set; }
}
