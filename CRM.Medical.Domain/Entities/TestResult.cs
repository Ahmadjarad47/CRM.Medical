using System.Text.Json;

namespace CRM.Medical.Domain.Entities;

public sealed class TestResult : BaseEntity
{
    public int Id { get; set; }

    public int TestRequestId { get; set; }

    public TestRequest TestRequest { get; set; } = null!;

    public DateTime ResultDate { get; set; }

    public JsonDocument? ResultData { get; set; }

    public string? PdfUrl { get; set; }

    public string Status { get; set; } = string.Empty;
}
