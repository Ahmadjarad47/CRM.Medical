namespace CRM.Medical.API.Contracts.MedicalWorkflow;

public sealed class LinkExternalPatientRequest
{
    /// <summary> Registered portal patient user id to associate with this external record.</summary>
    public string DirectPatientUserId { get; set; } = string.Empty;
}
