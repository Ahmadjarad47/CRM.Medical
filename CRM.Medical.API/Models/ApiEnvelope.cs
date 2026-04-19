using System.Text.Json.Serialization;

namespace CRM.Medical.API.Models;

/// <summary>
/// Standard JSON envelope for API responses: <c>ok</c> / <c>bad</c> messages plus optional payload.</summary>
public sealed class ApiEnvelope
{
    [JsonPropertyName("success")]
    public bool Success { get; init; }

    [JsonPropertyName("message")]
    public string Message { get; init; } = "";

    [JsonPropertyName("data")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public object? Data { get; init; }

    [JsonPropertyName("detail")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Detail { get; init; }

    [JsonPropertyName("errors")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IDictionary<string, string[]>? Errors { get; init; }

    public static ApiEnvelope Ok(object? data = null) => new()
    {
        Success = true,
        Message = "ok",
        Data = data
    };

    public static ApiEnvelope Bad(string? detail = null, IDictionary<string, string[]>? errors = null) => new()
    {
        Success = false,
        Message = "bad",
        Detail = detail,
        Errors = errors
    };
}
