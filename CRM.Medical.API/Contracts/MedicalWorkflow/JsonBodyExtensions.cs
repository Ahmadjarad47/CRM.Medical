using System.Text.Json;

namespace CRM.Medical.API.Contracts.MedicalWorkflow;

internal static class JsonBodyExtensions
{
    public static JsonDocument? ToJsonDocument(this JsonElement? element)
    {
        if (element is null)
            return null;

        if (element.Value.ValueKind is JsonValueKind.Null or JsonValueKind.Undefined)
            return null;

        return JsonDocument.Parse(element.Value.GetRawText());
    }
}
