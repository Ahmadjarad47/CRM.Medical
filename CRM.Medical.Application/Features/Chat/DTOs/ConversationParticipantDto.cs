using CRM.Medical.Domain.Chat;

namespace CRM.Medical.Application.Features.Chat.DTOs;

public sealed record ConversationParticipantDto(
    Guid Id,
    string UserId,
    string? FullName,
    ConversationParticipantRole Role,
    DateTime JoinedAtUtc,
    DateTime? LeftAtUtc);
