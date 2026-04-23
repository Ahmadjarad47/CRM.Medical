using System.Text.Json;
using CRM.Medical.Application.Exceptions;

namespace CRM.Medical.API.Mapping;

/// <summary>
/// Shared helpers for binding optional JSON in form or body fields to <see cref="JsonElement"/> without duplicating parse logic in controllers.
/// </summary>
public static class JsonRequestParsing
{
    public static JsonElement? ParseOptionalJsonElement(
        string? json,
        string? invalidJsonTitle = "Invalid request",
        string? invalidJsonDescription = "The JSON payload could not be read.")
    {
        if (string.IsNullOrWhiteSpace(json))
            return null;

        try
        {
            using var doc = JsonDocument.Parse(json);
            return doc.RootElement.Clone();
        }
        catch (JsonException)
        {
            throw new BusinessRuleException(
                invalidJsonTitle!,
                invalidJsonDescription!);
        }
    }
}
