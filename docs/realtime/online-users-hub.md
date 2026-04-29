# Online users hub (`/hubs/online-users`)

Canonical contract: [`asyncapi.yaml`](./asyncapi.yaml) (AsyncAPI — **not** Swagger).

Server implementation: `CRM.Medical.RealTime.Hubs.OnlineUsersHub` · Strongly typed client interface: **`IOnlineUsersClient`**.

Presence (`UserOnline` / `UserOffline`) is broadcast when a user transitions **to online** (first tracked connection) or **offline** (last disconnect), coordinated by **`PresenceLifecycleCoordinator`** and **`RedisConnectionManager`**.

## URL

| Item | Value |
|------|--------|
| Hub path | `/hubs/online-users` |
| Negotiate | `POST /hubs/online-users/negotiate` |

Clients must **subscribe to this hub** if they need presence events; **`/hubs/chat` alone does not deliver `UserOnline` / `UserOffline`** callbacks.

## Authentication

Same rules as [chat-hub](./chat-hub.md): JWT **`[Authorize]`**, optional **`access_token`** query on `/hubs/*` when headers are unavailable.

## Server methods (client → server)

There are **no** application RPC methods on **`OnlineUsersHub`**. Connecting and disconnecting drive **`OnConnectedAsync`** / **`OnDisconnectedAsync`** (presence lifecycle).

## Client callbacks (`IOnlineUsersClient`)

| Callback | Payload | When |
|----------|---------|------|
| `UserOnline` | `UserOnlinePayload` (`userId`, optional `roles`) | First connection for that user became active |
| `UserOffline` | `UserOfflinePayload` (`userId`) | Last connection for that user dropped |

Schemas: [`asyncapi.yaml` components — UserOnlinePayload / UserOfflinePayload](./asyncapi.yaml).

## Related code

- Hub: `CRM.Medical.RealTime.Hubs.OnlineUsersHub`
- Client interface & payloads: `CRM.Medical.RealTime.Hubs.IOnlineUsersClient`, `UserOnlinePayload`, `UserOfflinePayload`
- Notifier: `CRM.Medical.RealTime.Presence.OnlineUsersPresenceNotifier`
