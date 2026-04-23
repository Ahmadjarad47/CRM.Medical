using System.Text.Json;

namespace CRM.Medical.API.Contracts.MedicalWorkflow.MedicalTests;

public sealed record CreateMedicalTestRequest(
    string NameAr,
    string NameEn,
    double Price,
    string Category,
    string SampleType,
    JsonElement? ParameterSchema,
    string Status);
