using System.Text.Json;
using CRM.Medical.Domain.Constants;

namespace CRM.Medical.Domain.Entities;

public sealed class MedicalTest
{
    public int Id { get; set; }

    public string NameAr { get; set; } = string.Empty;

    public string NameEn { get; set; } = string.Empty;

    public double Price { get; set; }

    public string Category { get; set; } = string.Empty;

    public string SampleType { get; set; } = string.Empty;

    public JsonDocument? ParameterSchema { get; set; }

    /// <summary>Lifecycle state of this test record (see <see cref="MedicalTestStatuses"/>).</summary>
    public string Status { get; set; } = string.Empty;

    public string CreatedByUserId { get; set; } = string.Empty;
    public User CreatedByUser { get; set; } = null!;

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    /// <summary>Optional 1:1 — at most one appointment may reference this test instance.</summary>
    public Appointment? Appointment { get; set; }

    public TestRequest? TestRequest { get; set; }
}
