using System.Text.Json;

namespace CRM.Medical.API.Contracts.MedicalWorkflow;

public sealed class CreateMedicalTestRequest
{
    public string NameAr { get; set; } = string.Empty;

    public string NameEn { get; set; } = string.Empty;

    public double Price { get; set; }

    public string Category { get; set; } = string.Empty;

    public string SampleType { get; set; } = string.Empty;

    public JsonElement? ParameterSchema { get; set; }

    public string Status { get; set; } = string.Empty;
}
