using System.Text.Json;
using CRM.Medical.Domain.Enums;

namespace CRM.Medical.Domain.Entities;

public sealed class MedicalTest : BaseEntity
{
    public int Id { get; set; }

    public string NameAr { get; set; } = string.Empty;

    public string NameEn { get; set; } = string.Empty;

    public double Price { get; set; }

    public int CategoryMedicalId { get; set; }

    public CategoryMedical CategoryMedical { get; set; } = null!;

    public string SampleType { get; set; } = string.Empty;

    public JsonDocument? ParameterSchema { get; set; }

    public MedicalTestStatus Status { get; set; } = MedicalTestStatus.Pending;

    public ICollection<TestRequest> TestRequests { get; set; } = new List<TestRequest>();
}
