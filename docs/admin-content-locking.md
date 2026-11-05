# Admin content locking

## Scope and rollout

This contract coordinates exclusive editing of administrative content through SignalR. It is resource-agnostic: a content domain is connected by registering it in `IContentLockResourcePolicy` and applying `RequireContentLockAttribute` to its edit endpoint.

`EventNews` is the first reference integration. It demonstrates the contract on the Event News aggregate and its localization update endpoint; adding another content type must not require changes to the lock manager, hub protocol, DTOs, or client event names.

`ContentLock:EnforcementEnabled` is disabled by default. In that mode existing clients can save without a lock, while SignalR coordination remains available. Enable it only after a compatible client has been deployed.

Locks are held in process memory. Consequently, this implementation is valid only for a single API instance; a distributed lock provider is required before scaling out.

## Connection and methods

Connect an authenticated SignalR client to `/hubs/admin-content`. Supply the JWT as the `access_token` query value, using the SignalR `accessTokenFactory` option.

The hub methods are:

- `AcquireLock(resource)`;
- `RenewLock(resource, lockId)`;
- `ReleaseLock(resource, lockId)`;
- `GetLockStatus(resource)`.

`resource` has `resourceType` and `resourceId`. The API accepts only resource types registered by the server policy, and ids must be positive. The server derives lock ownership from the authenticated JWT; clients must never send an owner identifier.

The REST status endpoint is `GET /api/content-locks/{resourceType}/{resourceId}`. It follows the same server policy as the hub, so it is general even when only a subset of content domains is currently registered.

The client should call `RenewLock` every 30 seconds. An idle edit lock has a three-minute TTL and is released by the configured expiry scan. Therefore, the `LockExpired` notification and a new acquisition can occur up to one scan interval after the TTL. While the server is processing an update, the lock remains held until that update finishes; it cannot expire mid-save. A disconnected client's lock is released immediately unless an update is in progress, in which case it is released when that update finishes.

## HTTP update contract

When enforcement is enabled, the following headers are required for Event News update requests:

- `X-Content-Lock-Id`: opaque id returned by `AcquireLock`;
- `X-Content-Connection-Id`: id of the active SignalR connection.

An absent, expired, foreign, or invalid lock produces `423 Locked` with a ProblemDetails response. The response header `X-Content-Lock-Enforced` indicates whether this environment requires the headers.

Before an update handler starts, the server atomically marks the lock as updating. The lock cannot expire while the database transaction is running. On a successful commit, the server sends `EntityChanged` and releases the lock. A validation or not-found response keeps the normal edit lock so that its owner may correct the form and retry, unless its SignalR connection disconnected during the update.

## Events

- `LockAcquired`: a resource was locked;
- `LockDenied`: the caller could not acquire it and receives its status;
- `LockReleased`: the specific lock was released;
- `LockExpired`: the specific lock expired;
- `EntityChanged`: content was successfully persisted.

Lifecycle events contain `resourceType`, `resourceId`, `lockId`, `ownerIdentity`, and, where applicable, `expiresAtUtc`. The `lockId` lets clients ignore delayed events for an earlier lock lease. Lock status contains the owner identity and acquisition/expiry times. While a server-side update is in progress, `isUpdateInProgress` is `true` and `expiresAtUtc` is `null` because the lock remains held until the update ends. This endpoint is accessible only to authenticated administrators.

SignalR notifications are sent after persistence and do not change the result of an already committed update. They are real-time coordination signals, not a durable delivery mechanism.

## Client flow

1. Connect to the hub and call `GetLockStatus` or `AcquireLock`.
2. If acquired, retain both the returned `lockId` and SignalR connection id.
3. Renew on the configured interval while the edit form is open.
4. Send both values as HTTP headers when saving.
5. On `EntityChanged`, reload stale form data when the change came from another session.
6. On `LockReleased` or `LockExpired`, a waiting editor may acquire the resource again.
7. After reconnecting, discard the old connection id, request status again, and acquire a new lock when available.
8. When the authentication token expires, the server closes the SignalR connection. Reconnect with a fresh token and acquire a new lock when available.
