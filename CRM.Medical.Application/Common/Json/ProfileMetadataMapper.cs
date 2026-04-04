using System.Text.Json;

namespace CRM.Medical.Application.Common.Json;

public static class ProfileMetadataMapper
{
    public static JsonDocument? ToJsonDocument(object? profileMetadata)
    {
        if (profileMetadata is null)
            return null;

        if (profileMetadata is JsonElement el)
        {
            if (el.ValueKind is JsonValueKind.Null or JsonValueKind.Undefined)
                return null;
            return JsonDocument.Parse(el.GetRawText());
        }

        return JsonDocument.Parse(JsonSerializer.Serialize(profileMetadata));
    }

    public static JsonElement? ToJsonElement(JsonDocument? document) =>
        document is null ? null : document.RootElement;
}
