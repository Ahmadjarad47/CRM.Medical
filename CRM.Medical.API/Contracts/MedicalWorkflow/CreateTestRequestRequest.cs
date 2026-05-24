using System.Text.Json;

namespace CRM.Medical.API.Contracts.MedicalWorkflow;

public sealed class CreateTestRequestRequest
{
    public IReadOnlyList<int> MedicalTestIds { get; set; } = [];

    public DateTime RequestDate { get; set; }

    public string Status { get; set; } = string.Empty;

    public double TotalAmount { get; set; }

    public string? Notes { get; set; }

    public JsonElement? Metadata { get; set; }

    public string? DoctorId { get; set; }

    public string? LabClientId { get; set; }

    public string? DirectPatientId { get; set; }

    public int? ExternalPatientId { get; set; }
}
