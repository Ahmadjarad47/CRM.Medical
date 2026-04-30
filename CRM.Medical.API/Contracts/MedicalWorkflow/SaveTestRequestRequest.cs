using System.Text.Json;

namespace CRM.Medical.API.Contracts.MedicalWorkflow;

public sealed class SaveTestRequestRequest
{
    public int MedicalTestId { get; set; }

    public DateTime RequestDate { get; set; }

    public string Status { get; set; } = string.Empty;

    public double TotalAmount { get; set; }

    public string? Notes { get; set; }

    public JsonElement? Metadata { get; set; }

    /// <summary>Admin may set any doctor. Ignored for non-admin doctors (self is used).</summary>
    public string? DoctorId { get; set; }

    /// <summary>Admin or doctor may assign a lab user id. Ignored for lab partners (self is used).</summary>
    public string? LabClientId { get; set; }

    public string? DirectPatientId { get; set; }

    /// <summary>Optional walk-in identity. Mutually exclusive with <see cref="DirectPatientId"/> when assigning a patient subject.</summary>
    public int? ExternalPatientId { get; set; }
}
