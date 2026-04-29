using System.Text.Json;

namespace CRM.Medical.Infrastructure.MedicalWorkflow;

internal static class MedicalWorkflowJson
{
    public static JsonElement? ToJsonElement(JsonDocument? doc)
    {
        if (doc is null)
            return null;

        return JsonSerializer.Deserialize<JsonElement>(doc.RootElement.GetRawText());
    }

    public static JsonDocument? ToDocument(JsonElement? element)
    {
        if (element is null || element.Value.ValueKind is JsonValueKind.Undefined or JsonValueKind.Null)
            return null;

        return JsonDocument.Parse(element.Value.GetRawText());
    }
}
