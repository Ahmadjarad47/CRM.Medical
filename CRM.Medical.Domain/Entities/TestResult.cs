using System.Text.Json;
using CRM.Medical.Domain.Constants;

namespace CRM.Medical.Domain.Entities;

public sealed class TestResult
{
    public int Id { get; set; }

    public int TestRequestId { get; set; }
    public TestRequest TestRequest { get; set; } = null!;

    public DateTime ResultDate { get; set; }

    public JsonDocument? ResultData { get; set; }

    public string? PdfUrl { get; set; }

    /// <summary>Lifecycle state (see <see cref="TestResultStatuses"/>).</summary>
    public string Status { get; set; } = string.Empty;

    public string CreatedByUserId { get; set; } = string.Empty;
    public User CreatedByUser { get; set; } = null!;

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
