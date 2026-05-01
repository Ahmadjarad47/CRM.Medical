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
using Swashbuckle.AspNetCore.Annotations;

namespace CRM.Medical.API.Controllers.Chat;

[Authorize]
[ApiController]
[Route("api/chat")]
[SwaggerTag("Chat — includes nested `ChatUserSummaryDto` on user references (ids retained)")]
public sealed class ChatController(ISender mediator) : ControllerBase
{
    [HttpGet("conversations")]
    [SwaggerOperation(Summary = "List conversations", Description = "Each item includes `createdByUserId`/`createdByUser` and `lastMessage.senderUserId`/`lastMessage.sender`.")]
    [ProducesResponseType(typeof(IReadOnlyList<ConversationSummaryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetConversations([FromQuery] int skip = 0, [FromQuery] int take = 30, CancellationToken ct = default) =>
        Ok(await mediator.Send(new GetMyConversationsQuery(User.GetRequiredUserId(), skip, take), ct));

    [HttpPost("conversations/direct")]
    [SwaggerOperation(Summary = "Create or open direct conversation", Description = "Includes `createdByUser` and optional `lastMessage.sender`.")]
    [ProducesResponseType(typeof(ConversationSummaryDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateDirect([FromBody] CreateDirectConversationRequest request, CancellationToken ct) =>
        Ok(await mediator.Send(new CreateDirectConversationCommand(User.GetRequiredUserId(), request.OtherUserId), ct));

    [HttpPost("conversations/group")]
    [SwaggerOperation(Summary = "Create group conversation", Description = "Includes `createdByUser`.")]
    [ProducesResponseType(typeof(ConversationSummaryDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateGroup([FromBody] CreateGroupConversationRequest request, CancellationToken ct) =>
        Ok(await mediator.Send(
            new CreateGroupConversationCommand(User.GetRequiredUserId(), request.Title, request.ParticipantUserIds.ToList()),
            ct));

    [HttpGet("conversations/{conversationId:guid}/messages")]
    [SwaggerOperation(Summary = "Message history", Description = "Each message has `sender` + `senderId`; attachments include `uploadedByUser`.")]
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
    [SwaggerOperation(Summary = "Send message", Description = "Returns `MessageDto` with `sender` summary.")]
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
    [SwaggerOperation(Summary = "Mark message read", Description = "No body; read receipts over SignalR include `reader` on `ChatReadReceiptPayload`.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> MarkRead(Guid messageId, CancellationToken ct)
    {
        await mediator.Send(new MarkMessageAsReadCommand(User.GetRequiredUserId(), messageId), ct);
        return NoContent();
    }

    [HttpPost("messages/{messageId:guid}/attachments")]
    [Consumes("multipart/form-data")]
    [SwaggerOperation(Summary = "Upload attachment", Description = "Returns `uploadedByUserId` and nested `uploadedByUser`.")]
    [ProducesResponseType(typeof(MessageAttachmentDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> UploadAttachment(Guid messageId, IFormFile file, CancellationToken ct) =>
        Ok(await mediator.Send(new UploadMessageAttachmentCommand(User.GetRequiredUserId(), messageId, file), ct));

    [HttpGet("online-users")]
    [SwaggerOperation(Summary = "Online peers", Description = "Each row has `userId`, `isOnline`, and nested `user` (`ChatUserSummaryDto`).")]
    [ProducesResponseType(typeof(IReadOnlyList<OnlineUserDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOnlineUsers(CancellationToken ct) =>
        Ok(await mediator.Send(new GetOnlineUsersQuery(User.GetRequiredUserId()), ct));

    [HttpGet("conversations/{conversationId:guid}/participants")]
    [SwaggerOperation(Summary = "Participants", Description = "Each row includes `userId`, `fullName`, and nested `user` summary.")]
    [ProducesResponseType(typeof(IReadOnlyList<ConversationParticipantDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetParticipants(Guid conversationId, CancellationToken ct) =>
        Ok(await mediator.Send(new GetConversationParticipantsQuery(User.GetRequiredUserId(), conversationId), ct));

    [HttpPost("conversations/{conversationId:guid}/leave")]
    [SwaggerOperation(Summary = "Leave conversation", Description = "No response body.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> LeaveConversation(Guid conversationId, CancellationToken ct)
    {
        await mediator.Send(new LeaveConversationCommand(User.GetRequiredUserId(), conversationId), ct);
        return NoContent();
    }
}
