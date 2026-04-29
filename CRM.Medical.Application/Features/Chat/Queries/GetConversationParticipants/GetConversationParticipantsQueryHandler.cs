using CRM.Medical.Application.Features.Chat.DTOs;
using CRM.Medical.Application.Features.Chat.Persistence;
using CRM.Medical.Application.Features.Chat.Services;
using CRM.Medical.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CRM.Medical.Application.Features.Chat.Queries.GetConversationParticipants;

public sealed class GetConversationParticipantsQueryHandler(
    IChatPersistence chatPersistence,
    IChatAuthorizationService chatAuthorization,
    UserManager<User> userManager)
    : IRequestHandler<GetConversationParticipantsQuery, IReadOnlyList<ConversationParticipantDto>>
{
    public async Task<IReadOnlyList<ConversationParticipantDto>> Handle(
        GetConversationParticipantsQuery request,
        CancellationToken cancellationToken)
    {
        await chatAuthorization.EnsureActiveParticipantAsync(request.UserId, request.ConversationId, cancellationToken);

        var rows = await chatPersistence.GetActiveParticipantsAsync(request.ConversationId, cancellationToken);
        var ids = rows.Select(r => r.UserId).Distinct().ToList();
        var users = await userManager.Users.Where(u => ids.Contains(u.Id)).ToListAsync(cancellationToken);
        var names = users.ToDictionary(u => u.Id, u => u.FullName);

        return rows
            .Select(p => new ConversationParticipantDto(
                p.Id,
                p.UserId,
                names.TryGetValue(p.UserId, out var n) ? n : null,
                p.Role,
                p.JoinedAt,
                p.LeftAt))
            .ToList();
    }
}
