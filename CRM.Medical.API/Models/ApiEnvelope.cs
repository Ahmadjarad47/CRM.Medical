using System.Text.Json.Serialization;
using CRM.Medical.Application.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;

namespace CRM.Medical.API.Models;

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

    /// <summary>Validation or field-level error messages; used with message "Validation failed".</summary>
    [JsonPropertyName("errors")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IReadOnlyList<string>? Errors { get; init; }

    public static ApiEnvelope Ok(object? data = null) => new()
    {
        Success = true,
        Message = "ok",
        Data = data
    };

    public static ApiEnvelope ValidationFailed(IReadOnlyList<string> errors) => new()
    {
        Success = false,
        Message = "Validation failed",
        Errors = errors
    };

    public static ApiEnvelope BusinessRuleInvalid(string message, string detail) => new()
    {
        Success = false,
        Message = message,
        Detail = detail
    };

    public static ApiEnvelope FromApplicationException(ApplicationExceptionBase ex) => ex switch
    {
        BusinessRuleException b => new ApiEnvelope
        {
            Success = false,
            Message = b.PublicMessage,
            Detail = b.Detail
        },
        ApplicationBadRequestException => new ApiEnvelope
        {
            Success = false,
            Message = "Invalid request",
            Detail = ex.Message
        },
        ApplicationNotFoundException => new ApiEnvelope
        {
            Success = false,
            Message = "Not found",
            Detail = ex.Message
        },
        ApplicationConflictException => new ApiEnvelope
        {
            Success = false,
            Message = "Conflict",
            Detail = ex.Message
        },
        ApplicationForbiddenException => new ApiEnvelope
        {
            Success = false,
            Message = "Forbidden",
            Detail = ex.Message
        },
        ApplicationUnauthorizedException => new ApiEnvelope
        {
            Success = false,
            Message = "Unauthorized",
            Detail = ex.Message
        },
        _ => new ApiEnvelope
        {
            Success = false,
            Message = "Error",
            Detail = ex.Message
        }
    };

    public static ApiEnvelope InternalServerError() => new()
    {
        Success = false,
        Message = "Internal server error",
        Detail = "An unexpected error occurred."
    };

    public static ApiEnvelope FromHttpStatusCode(int statusCode, string? detail) => new()
    {
        Success = false,
        Message = statusCode switch
        {
            StatusCodes.Status404NotFound => "Not found",
            StatusCodes.Status401Unauthorized => "Unauthorized",
            StatusCodes.Status403Forbidden => "Forbidden",
            _ => "Error"
        },
        Detail = string.IsNullOrEmpty(detail) ? ReasonPhrases.GetReasonPhrase(statusCode) : detail
    };
}
