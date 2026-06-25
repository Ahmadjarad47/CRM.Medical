using System.Text.Json;
using CRM.Medical.Domain.Enums;

namespace CRM.Medical.API.Contracts.MedicalWorkflow;

public sealed class UpdateMedicalTestRequest
{
    public string NameAr { get; set; } = string.Empty;

    public string NameEn { get; set; } = string.Empty;

    public double Price { get; set; }

    public int CategoryMedicalId { get; set; }

    public string SampleType { get; set; } = string.Empty;

    public JsonElement? ParameterSchema { get; set; }

    public MedicalTestStatus Status { get; set; } = MedicalTestStatus.Pending;
}
