using System.Text.Json;

namespace CRM.Medical.API.Contracts.MedicalWorkflow;

public sealed class SaveTestResultRequest
{
    public DateTime ResultDate { get; set; }

    public JsonElement? ResultData { get; set; }

    public string? PdfUrl { get; set; }

    public string Status { get; set; } = string.Empty;
}
