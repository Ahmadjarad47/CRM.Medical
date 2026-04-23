using System.Text.Json;

namespace CRM.Medical.API.Contracts.MedicalWorkflow.TestRequests;

public sealed record UpdateTestRequestRequest(
    DateTime RequestDate,
    string Status,
    double TotalAmount,
    string? Notes,
    JsonElement? Metadata);
