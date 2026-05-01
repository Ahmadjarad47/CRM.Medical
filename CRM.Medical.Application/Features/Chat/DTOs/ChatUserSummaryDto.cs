namespace CRM.Medical.Application.Features.Chat.DTOs;

/// <summary>Batch-friendly user envelope for chat APIs and SignalR (IDs kept elsewhere on parent DTOs).</summary>
public sealed record ChatUserSummaryDto(
    string UserId,
    string FullName,
    string? Email,
    string? PhoneNumber,
    string? Role,
    bool IsOnline);
