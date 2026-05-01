namespace CRM.Medical.Application.Features.Chat.DTOs;

public sealed record OnlineUserDto(string UserId, bool IsOnline, ChatUserSummaryDto User);
