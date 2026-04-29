using CRM.Medical.API.Contracts.Chat;
using CRM.Medical.API.Extensions;
using CRM.Medical.Application.Features.Chat.Commands.CreateDirectConversation;
using CRM.Medical.Application.Features.Chat.Commands.CreateGroupConversation;
using CRM.Medical.Application.Features.Chat.Commands.LeaveConversation;
using CRM.Medical.Application.Features.Chat.Commands.MarkMessageAsRead;
using CRM.Medical.Application.Features.Chat.Commands.SendMessage;
using CRM.Medical.Application.Features.Chat.Commands.UploadMessageAttachment;
using CRM.Medical.Application.Features.Chat.DTOs;
using CRM.Medical.Application.Features.Chat.Queries.GetConversationMessages;
using CRM.Medical.Application.Features.Chat.Queries.GetConversationParticipants;
using CRM.Medical.Application.Features.Chat.Queries.GetMyConversations;
using CRM.Medical.Application.Features.Chat.Queries.GetOnlineUsers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Medical.API.Controllers.Chat;

[Authorize]
[ApiController]
[Route("api/chat")]
public sealed class ChatController(ISender mediator) : ControllerBase
{
    [HttpGet("conversations")]
    [ProducesResponseType(typeof(IReadOnlyList<ConversationSummaryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetConversations([FromQuery] int skip = 0, [FromQuery] int take = 30, CancellationToken ct = default) =>
        Ok(await mediator.Send(new GetMyConversationsQuery(User.GetRequiredUserId(), skip, take), ct));

    [HttpPost("conversations/direct")]
    [ProducesResponseType(typeof(ConversationSummaryDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateDirect([FromBody] CreateDirectConversationRequest request, CancellationToken ct) =>
        Ok(await mediator.Send(new CreateDirectConversationCommand(User.GetRequiredUserId(), request.OtherUserId), ct));

    [HttpPost("conversations/group")]
    [ProducesResponseType(typeof(ConversationSummaryDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateGroup([FromBody] CreateGroupConversationRequest request, CancellationToken ct) =>
        Ok(await mediator.Send(
            new CreateGroupConversationCommand(User.GetRequiredUserId(), request.Title, request.ParticipantUserIds.ToList()),
            ct));

    [HttpGet("conversations/{conversationId:guid}/messages")]
    [ProducesResponseType(typeof(IReadOnlyList<MessageDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMessages(
        Guid conversationId,
        [FromQuery] DateTime? beforeUtc,
        [FromQuery] int take = 50,
        CancellationToken ct = default) =>
        Ok(await mediator.Send(
            new GetConversationMessagesQuery(User.GetRequiredUserId(), conversationId, beforeUtc, take),
            ct));

    [HttpPost("messages")]
    [ProducesResponseType(typeof(MessageDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> PostMessage([FromBody] PostChatMessageRequest request, CancellationToken ct) =>
        Ok(await mediator.Send(
            new SendMessageCommand(
                User.GetRequiredUserId(),
                request.ConversationId,
                request.Text,
                request.MessageType,
                request.FileUrl,
                request.ReplyToId),
            ct));

    [HttpPost("messages/{messageId:guid}/read")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> MarkRead(Guid messageId, CancellationToken ct)
    {
        await mediator.Send(new MarkMessageAsReadCommand(User.GetRequiredUserId(), messageId), ct);
        return NoContent();
    }

    [HttpPost("messages/{messageId:guid}/attachments")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(MessageAttachmentDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> UploadAttachment(Guid messageId, IFormFile file, CancellationToken ct) =>
        Ok(await mediator.Send(new UploadMessageAttachmentCommand(User.GetRequiredUserId(), messageId, file), ct));

    [HttpGet("online-users")]
    [ProducesResponseType(typeof(IReadOnlyList<OnlineUserDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOnlineUsers(CancellationToken ct) =>
        Ok(await mediator.Send(new GetOnlineUsersQuery(User.GetRequiredUserId()), ct));

    [HttpGet("conversations/{conversationId:guid}/participants")]
    [ProducesResponseType(typeof(IReadOnlyList<ConversationParticipantDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetParticipants(Guid conversationId, CancellationToken ct) =>
        Ok(await mediator.Send(new GetConversationParticipantsQuery(User.GetRequiredUserId(), conversationId), ct));

    [HttpPost("conversations/{conversationId:guid}/leave")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> LeaveConversation(Guid conversationId, CancellationToken ct)
    {
        await mediator.Send(new LeaveConversationCommand(User.GetRequiredUserId(), conversationId), ct);
        return NoContent();
    }
}
