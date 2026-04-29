# Chat hub (`/hubs/chat`)

Canonical contract: [`asyncapi.yaml`](./asyncapi.yaml) (AsyncAPI — **not** Swagger).

Server implementation: `CRM.Medical.RealTime.Hubs.ChatHub` · Strongly typed client interface: **`IChatClient`**.

## URL

| Item | Value |
|------|--------|
| Hub path | `/hubs/chat` |
| Negotiate | `POST /hubs/chat/negotiate` (SignalR versioning query `negotiateVersion` as usual) |

## Authentication

- **`[Authorize]`** — JWT access token required.
- HTTP negotiate can use **`Authorization: Bearer <JWT>`**.
- WebSocket builds that cannot send headers must pass **`access_token=<JWT>`** as a query parameter on negotiate and hub URLs. The API maps `access_token` into the bearer token when the path starts with **`/hubs`** (`JwtAuthenticationExtensions`).

## Groups

After authorization, connections are added to:

| Group pattern | Purpose |
|----------------|---------|
| `user:{userId}` | User-scoped pushes (e.g. `ConversationUpdated`) |
| `conversation:{conversationId}` | Conversation participants receive messages, typing, read receipts |

Joining a conversation group requires **`JoinConversation(conversationId)`** after connect.

## Server methods (client → server)

Invoked by name on **`ChatHub`** (same names as C# methods):

| Method | Arguments | Behavior |
|--------|-----------|----------|
| `JoinConversation` | `conversationId` (uuid) | Ensures participant; adds connection to `conversation:{id}` |
| `LeaveConversation` | `conversationId` | Domain leave + removes group |
| `SendMessage` | [`SendMessageRequest`](./asyncapi.yaml#/components/schemas/SendMessageRequest) | Persist message; server broadcasts `ReceiveMessage` etc. |
| `MarkAsRead` | `messageId` (uuid) | Marks read; may broadcast `ReadReceipt` |
| `Typing` | `conversationId` | Others in conversation receive **`Typing`** (`IChatClient`) |
| `StopTyping` | `conversationId` | Others receive **`StopTyping`** |

Missing authenticated user id raises **`HubException("AUTH_REQUIRED")`**.

## Client callbacks (`IChatClient`)

Implemented by the SignalR client (method names match C# **`IChatClient`**):

| Callback | Payload | Source |
|----------|---------|--------|
| `ReceiveMessage` | `ChatMessageRealtimePayload` | New message in conversation |
| `Typing` | `ChatTypingPayload` | Someone is typing |
| `StopTyping` | `ChatTypingPayload` | Someone stopped typing |
| `ReadReceipt` | `ChatReadReceiptPayload` | Message read by a participant |
| `ConversationUpdated` | `ConversationUpdatedPayload` | Badge/list hint for peer users (`user:{userId}` group) |

Payload shapes are defined under **`components/schemas`** in [`asyncapi.yaml`](./asyncapi.yaml).

### DTO summary

- **`SendMessageRequest`** — hub-only request for `SendMessage` (`conversationId`, optional `text`, `messageType`, `fileUrl`, `replyToId`).
- **`ChatMessageRealtimePayload`**, **`ChatTypingPayload`**, **`ChatReadReceiptPayload`**, **`ConversationUpdatedPayload`** — application-layer records (`CRM.Medical.Application.Features.Chat.Models`), serialized with default SignalR JSON (**camelCase** names on the wire unless configured otherwise).

## Related code

- Hub: `CRM.Medical.RealTime.Hubs.ChatHub`
- Client interface: `CRM.Medical.RealTime.Hubs.IChatClient`
- Hub request DTO: `CRM.Medical.RealTime.Dtos.SendMessageRequest`
