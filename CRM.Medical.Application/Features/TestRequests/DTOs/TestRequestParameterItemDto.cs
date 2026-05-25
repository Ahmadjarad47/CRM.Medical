using System.Text.Json;

namespace CRM.Medical.Application.Features.TestRequests.DTOs;

public sealed record TestRequestParameterItemDto(
    string ParameterName,
    string? ParameterNameAr,
    string? ParameterKey,
    JsonElement? Value);
