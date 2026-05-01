using System.Security.Claims;
using CRM.Medical.Application.Features.Chat.Commands.LeaveConversation;
using CRM.Medical.Application.Features.Chat.Commands.MarkMessageAsRead;
using CRM.Medical.Application.Features.Chat.Commands.SendMessage;
using CRM.Medical.Application.Features.Chat.Models;
using CRM.Medical.Application.Features.Chat.Services;
using CRM.Medical.RealTime.Dtos;
using CRM.Medical.RealTime.Groups;
using CRM.Medical.RealTime.Presence;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace CRM.Medical.RealTime.Hubs;

/// <summary>
/// Real-time chat hub — JWT authenticated via Bearer token on HTTP negotiate / WebSockets.
/// </summary>
[Authorize]
public sealed class ChatHub(
    IMediator mediator,
    IChatAuthorizationService chatAuthorization,
    PresenceLifecycleCoordinator presenceLifecycle,
    ILogger<ChatHub> logger)
    : Hub<IChatClient>
{
    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!string.IsNullOrEmpty(userId))
        {
            var roles = Context.User!.FindAll(ClaimTypes.Role)
                .Select(c => c.Value)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            logger.LogInformation(
                "SignalR CONNECTED | UserId: {UserId} | ConnectionId: {ConnectionId} | Roles: {Roles}",
                userId,
                Context.ConnectionId,
                string.Join(",", roles));

            await presenceLifecycle.OnHubConnectedAsync(
                userId,
                Context.ConnectionId,
                roles,
                Context.ConnectionAborted);

            await Groups.AddToGroupAsync(Context.ConnectionId, ChatGroups.User(userId));
        }
        else
        {
            logger.LogWarning(
                "SignalR CONNECTED without NameIdentifier | ConnectionId: {ConnectionId}",
                Context.ConnectionId);
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await presenceLifecycle.OnHubDisconnectedAsync(Context.ConnectionId, Context.ConnectionAborted);
        await base.OnDisconnectedAsync(exception);
    }

    public async Task JoinConversation(Guid conversationId)
    {
        var userId = RequireUserId();
        await chatAuthorization.EnsureActiveParticipantAsync(userId, conversationId, Context.ConnectionAborted);
        await Groups.AddToGroupAsync(Context.ConnectionId, ChatGroups.Conversation(conversationId));
    }

    public async Task LeaveConversation(Guid conversationId)
    {
        var userId = RequireUserId();
        await mediator.Send(new LeaveConversationCommand(userId, conversationId), Context.ConnectionAborted);
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, ChatGroups.Conversation(conversationId));
    }

    public async Task SendMessage(SendMessageRequest request)
    {
        var userId = RequireUserId();
        await mediator.Send(
            new SendMessageCommand(
                userId,
                request.ConversationId,
                request.Text,
                request.MessageType,
                request.FileUrl,
                request.ReplyToId),
            Context.ConnectionAborted);
    }

    public Task MarkAsRead(Guid messageId) =>
        mediator.Send(new MarkMessageAsReadCommand(RequireUserId(), messageId), Context.ConnectionAborted);

    public async Task Typing(Guid conversationId)
    {
        var userId = RequireUserId();
        await chatAuthorization.EnsureActiveParticipantAsync(userId, conversationId, Context.ConnectionAborted);
        await Clients.OthersInGroup(ChatGroups.Conversation(conversationId))
            .Typing(new ChatTypingPayload(userId, IsTyping: true));
    }

    public async Task StopTyping(Guid conversationId)
    {
        var userId = RequireUserId();
        await chatAuthorization.EnsureActiveParticipantAsync(userId, conversationId, Context.ConnectionAborted);
        await Clients.OthersInGroup(ChatGroups.Conversation(conversationId))
            .StopTyping(new ChatTypingPayload(userId, IsTyping: false));
    }

    private string RequireUserId()
    {
        var id = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(id))
            throw new HubException("AUTH_REQUIRED");

        return id;
    }
}
