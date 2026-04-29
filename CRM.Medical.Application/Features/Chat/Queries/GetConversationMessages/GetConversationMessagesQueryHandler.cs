using CRM.Medical.Application.Features.Chat.DTOs;
using CRM.Medical.Application.Features.Chat.Mapping;
using CRM.Medical.Application.Features.Chat.Persistence;
using CRM.Medical.Application.Features.Chat.Services;
using CRM.Medical.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CRM.Medical.Application.Features.Chat.Queries.GetConversationMessages;

public sealed class GetConversationMessagesQueryHandler(
    IChatPersistence chatPersistence,
    IChatAuthorizationService chatAuthorization,
    UserManager<User> userManager)
    : IRequestHandler<GetConversationMessagesQuery, IReadOnlyList<MessageDto>>
{
    public async Task<IReadOnlyList<MessageDto>> Handle(GetConversationMessagesQuery request, CancellationToken cancellationToken)
    {
        await chatAuthorization.EnsureActiveParticipantAsync(request.UserId, request.ConversationId, cancellationToken);

        var take = Math.Clamp(request.Take, 1, 200);
        var messages = await chatPersistence.ListMessagesAsync(request.ConversationId, request.BeforeUtc, take, cancellationToken);

        var chronological = messages.OrderBy(m => m.CreatedAt).ToList();

        var senderIds = chronological.Select(m => m.SenderId).Distinct().ToList();
        var users = await userManager.Users.Where(u => senderIds.Contains(u.Id)).ToListAsync(cancellationToken);
        var map = users.ToDictionary(u => u.Id);

        var dtos = new List<MessageDto>();
        foreach (var m in chronological)
        {
            map.TryGetValue(m.SenderId, out var sender);
            dtos.Add(m.ToDto(sender));
        }

        return dtos;
    }
}
