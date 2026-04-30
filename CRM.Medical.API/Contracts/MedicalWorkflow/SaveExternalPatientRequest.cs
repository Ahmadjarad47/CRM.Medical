namespace CRM.Medical.API.Contracts.MedicalWorkflow;

public sealed class SaveExternalPatientRequest
{
    public string FullName { get; set; } = string.Empty;

    public int? Age { get; set; }

    public string Gender { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public string? ExternalId { get; set; }
}
