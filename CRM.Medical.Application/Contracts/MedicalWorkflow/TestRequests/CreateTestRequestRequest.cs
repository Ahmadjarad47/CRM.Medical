using System.Text.Json;

namespace CRM.Medical.API.Contracts.MedicalWorkflow.TestRequests;

public sealed record CreateTestRequestRequest(
    int MedicalTestId,
    DateTime RequestDate,
    string Status,
    double TotalAmount,
    string? Notes,
    JsonElement? Metadata);
