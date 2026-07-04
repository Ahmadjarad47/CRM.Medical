using System.Text.Json;

namespace CRM.Medical.Domain.Entities;

public sealed class TestRequest : BaseEntity
{
    public int Id { get; set; }


    public int MedicalTestId { get; set; }

    public MedicalTest MedicalTest { get; set; } = null!;

    /// <summary>Treating or ordering doctor (nullable when created by lab/admin only).</summary>
    public string? DoctorId { get; set; }

    /// <summary>Lab partner account responsible for fulfilling the request.</summary>
    public string? LabClientId { get; set; }

    /// <summary>Patient user this test is performed for (portal visibility).</summary>
    public string? DirectPatientId { get; set; }

    /// <summary>Optional walk-in or partner-system patient identity (exclusive with <see cref="DirectPatientId"/> when assigning a subject).</summary>
    public int? ExternalPatientId { get; set; }

    public ExternalPatient? ExternalPatient { get; set; }

    public DateTime RequestDate { get; set; }

    public string Status { get; set; } = string.Empty;

    public double TotalAmount { get; set; }

    public string? Notes { get; set; }

    public JsonDocument? Metadata { get; set; }

    public TestResult? TestResult { get; set; }
}
