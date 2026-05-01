using CRM.Medical.Domain.Chat;

namespace CRM.Medical.RealTime.Dtos;

/// <summary>SignalR payload for hub <c>SendMessage</c>.</summary>
/// <remarks>
/// SignalR uses JSON over the connection, so a browser multipart <see cref="Microsoft.AspNetCore.Http.IFormFile"/> is not bound directly.
/// Send <see cref="FileContent"/> as a base64-encoded byte array (SignalR JSON) plus <see cref="FileName"/>; the hub builds an <see cref="Microsoft.AspNetCore.Http.IFormFile"/>,
/// uploads via <c>IFileStorageService</c>, and passes the resulting URL to the domain command. Alternatively set <see cref="FileUrl"/> when the file was uploaded over HTTP first.
/// </remarks>
public sealed class SendMessageRequest
{
    public Guid ConversationId { get; set; }

    public string? Text { get; set; }

    public ChatMessageType MessageType { get; set; } = ChatMessageType.Text;

    /// <summary>Optional when the file was already uploaded (e.g. REST). Ignored if <see cref="FileContent"/> is non-empty.</summary>
    public string? FileUrl { get; set; }

    public Guid? ReplyToId { get; set; }

    /// <summary>Raw file bytes; serializes as base64 in JSON. Requires <see cref="FileName"/> when non-empty.</summary>
    public byte[]? FileContent { get; set; }

    /// <summary>Original file name (used for validation and storage).</summary>
    public string? FileName { get; set; }

    /// <summary>MIME type (e.g. <c>image/png</c>). Optional; defaults to <c>application/octet-stream</c>.</summary>
    public string? ContentType { get; set; }
}
