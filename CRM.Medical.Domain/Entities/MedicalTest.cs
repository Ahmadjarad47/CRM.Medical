using System.Text.Json;

namespace CRM.Medical.Domain.Entities;

public sealed class MedicalTest : BaseEntity
{
    public int Id { get; set; }

    public string NameAr { get; set; } = string.Empty;

    public string NameEn { get; set; } = string.Empty;

    public double Price { get; set; }

    public string Category { get; set; } = string.Empty;

    public string SampleType { get; set; } = string.Empty;

    public JsonDocument? ParameterSchema { get; set; }

    public string Status { get; set; } = string.Empty;

    public ICollection<TestRequest> TestRequests { get; set; } = new List<TestRequest>();
}
