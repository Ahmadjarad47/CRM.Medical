using System.Text.Json;
using CRM.Medical.Domain.Constants;

namespace CRM.Medical.Domain.Entities;

public sealed class TestRequest
{
    public int Id { get; set; }

    public int MedicalTestId { get; set; }
    public MedicalTest MedicalTest { get; set; } = null!;

    public DateTime RequestDate { get; set; }

    /// <summary>Lifecycle state (see <see cref="TestRequestStatuses"/>).</summary>
    public string Status { get; set; } = string.Empty;

    public double TotalAmount { get; set; }

    public string? Notes { get; set; }

    public JsonDocument? Metadata { get; set; }

    public string CreatedByUserId { get; set; } = string.Empty;
    public User CreatedByUser { get; set; } = null!;

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public TestResult? Result { get; set; }
}
