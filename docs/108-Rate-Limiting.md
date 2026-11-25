# Rate Limiting

This document describes how the API limits the number of requests from one client: which endpoints are limited, how clients are identified, what a rejected request looks like, how to change the limits, and how to keep the coverage complete when you add new endpoints.

## Why

Anonymous clients can call the public API without an account. Without limits, one client could:

- flood the database with public read requests,
- download large files from storage in a loop,
- send many emails through the contact form or start many payments,
- guess passwords, refresh tokens or PDF preview tickets,
- open an unlimited number of real-time connections.

Rate limiting caps how much one client can do in a time window. It works together with other protections, for example the admin account lockout ([106-Account-Lockout.md](106-Account-Lockout.md)): the rate limit slows down one client, the lockout protects one account.

## How it works

The API uses the built-in ASP.NET Core rate limiter (`Microsoft.AspNetCore.RateLimiting`).

| Part | File |
|---|---|
| Policy registration, client identification | `VictoryCenter.WebAPI/Extensions/RateLimiterConfiguration.cs` |
| Limit values (options) and their validation | `VictoryCenter.WebAPI/Extensions/RateLimitingOptions.cs` |
| The 429 response | `VictoryCenter.WebAPI/Extensions/RateLimitRejectionHandler.cs` |
| Default policy for public controllers | `VictoryCenter.WebAPI/Controllers/Common/PublicApiController.cs` |
| Trusted reverse proxies | `VictoryCenter.WebAPI/Extensions/ForwardedHeadersConfiguration.cs` |
| Pipeline order | `VictoryCenter.WebAPI/Program.cs` |

- Every policy except image upload is a **fixed window** policy: a client may send `PermitLimit` requests per `WindowSeconds`. Requests over the limit are rejected right away, and nothing is queued.
- Each policy keeps its own counters. Using up one limit (for example login) does not affect another one (for example refresh-token).
- A policy is attached to an endpoint with `[EnableRateLimiting(...)]` on a controller or an action, or with `.RequireRateLimiting(...)` on a mapped endpoint.
- **An action-level attribute replaces the controller-level one.** For example, `ContactUsController` inherits the `public-read` policy from `PublicApiController`, but its action has `[EnableRateLimiting(SubmitContactUsForm)]`, so only the contact-us limit applies to it.
- Rate limiting runs before model binding, validation and the handler. A rejected request never reaches the database or an external service, and every request counts, including invalid ones (for example `400` or `401` responses).

## Policies and endpoints

All limits are per client (see [How clients are identified](#how-clients-are-identified)). The policy name constants are in `RateLimitingPolicyNameConstants`.

| Policy (option name) | Default limit | Endpoints | Why |
|---|---|---|---|
| `PublicRead` | 60 / 1 min | Every action of every controller in `Controllers/Public/**` (main page, news, events, FAQ, team, partners, history, reports list, bank details, support options, video and feedback reviews, hippotherapy pages, localizations, languages, etc.), unless the action has its own policy | Each request runs a database query. The limit is high enough for normal browsing of the website. |
| `FileDownload` | 20 / 1 min | `GET api/PdfReports/{id}/file`, `GET api/PdfReports/preview/{fileName}?ticket=...` | Both stream a whole PDF from storage, the most expensive public read. The preview endpoint is anonymous and checks a ticket, so the limit also slows down ticket guessing. Both endpoints share one counter per client. |
| `RealtimeConnect` | 30 / 1 min | `/hubs/reports` (all hub requests: negotiate, WebSocket / SSE connect, long-polling polls) | Each open connection uses server resources. All hub requests are limited, not only negotiate, because a client can skip negotiation and open a WebSocket directly. |
| `AdminLogin` | 5 / 1 min | `POST api/auth/login` | Password guessing. |
| `AdminLogout` | 20 / 1 min | `POST api/auth/logout` | Anonymous (it revokes the session from the refresh cookie when the access token is expired), so it needs its own limit. Every call reads the database, and failed or invalid calls count too. |
| `AuthRefresh` | 30 / 1 min | `POST api/auth/refresh-token` | Anonymous; each call reads the database and rotates tokens. Failed attempts count too, so token guessing is capped. |
| `InitiateDonation` | 10 / 1 min | `POST api/Payments/donate` | Calls the external payment provider. |
| `SubmitContactUsForm` | 10 / 24 h | `POST api/ContactUs` | Sends an email through the external email provider. |
| `ImageUpload` | 2 at a time, queue 64 | `POST api/Image`, `PUT api/Image/{id}` (admin only) | A **concurrency** limit, not a request rate: it keeps image decoding memory bounded. Extra uploads wait in the queue; they are rejected only when the queue is full. |
| `Swagger` | 60 / 1 min | `/swagger/**` outside Development | Applied by the global limiter, because Swagger is served by middleware, not by endpoints. In Development Swagger is not limited. |

### Not rate limited

| Endpoint | Reason |
|---|---|
| `/health/live`, `/health/ready` | Orchestrator probes. They must never get `429`, otherwise a healthy instance could be restarted or taken out of traffic. |
| Authenticated admin endpoints (`Controllers/Admin/**`, except the anonymous ones listed above) | They require an admin JWT, so anonymous clients cannot reach them. Image upload still has its concurrency limit. |

### Search

There is no public search endpoint. All `Search*` endpoints are in admin controllers and require authentication, so they do not need a public rate limit.

## How clients are identified

Every policy uses the same partition key: `RateLimiterConfiguration.GetClientPartitionKey(HttpContext)`.

- The key comes **only** from `HttpContext.Connection.RemoteIpAddress`.
  - The code never reads `X-Forwarded-For`, `X-Real-IP` or other forwarding headers. Clients can set any value in these headers, so trusting them would let a client get a new limit with every request.
- **IPv4** addresses are used as is, for example `198.51.100.10`.
- **IPv4-mapped IPv6** addresses (`::ffff:198.51.100.10`) are treated as the IPv4 address, so one client does not get two counters.
- **IPv6** addresses are grouped by their `/64` prefix, for example `2001:db8:1:2::/64`. One client usually controls a whole `/64` and could otherwise change its address to get a new limit.
- If the address is unknown, all such requests share one `unknown` counter.

### Behind a reverse proxy

In Staging and Production the API runs behind a reverse proxy. There, `RemoteIpAddress` is the proxy address, not the client address. The forwarded-headers middleware (`app.UseForwardedHeaders()`, which runs before `app.UseRateLimiter()`) replaces it with the client address from `X-Forwarded-For`, **but only when the request comes from a trusted proxy**:

```json
"ReverseProxy": {
  "KnownNetworks": [ "172.16.0.0/12" ]
}
```

- Requests from addresses outside `KnownNetworks` keep their real connection address. Their `X-Forwarded-For` header is ignored.
- Only the last entry of `X-Forwarded-For` is used (the default `ForwardLimit` of 1), which is the address our proxy added.
- Loopback addresses are always trusted.

> **Important:** `ReverseProxy:KnownNetworks` must contain the network of the real reverse proxy in every environment that runs behind one. If it is missing or wrong, all clients share the proxy's counter, and the whole website can get `429` from a few busy users. If it is too wide (for example `0.0.0.0/0`), any client can choose its own address with `X-Forwarded-For` and bypass the limits.

## The 429 response

Every rejected request, from any policy or the Swagger limiter, gets the same response from `RateLimitRejectionHandler`:

```http
HTTP/1.1 429 Too Many Requests
Content-Type: application/problem+json
Retry-After: 42

{
  "title": "Too Many Requests",
  "status": 429,
  "detail": "Too many requests. Please try again in 42 seconds.",
  "traceId": "00-..."
}
```

- `Retry-After` is the number of seconds (at least 1) until the client's window resets.
  - The concurrency limit (image upload) has no window, so its response has **no** `Retry-After` header, and the detail is `Too many requests. Please try again later.`
- The body is created with the application's `ProblemDetailsFactory` (`CustomProblemDetailsFactory`), so it has the same shape as the other API errors. The factory knows the status `429` and gives it the title `Too Many Requests`.
- Endpoints with a limit declare `429` in Swagger. `PublicApiController` declares it for all public actions.
- Clients (the frontend) should show a "try again later" message and wait at least `Retry-After` seconds before retrying.

### Logs

Each rejection writes one warning with the category `VictoryCenter.WebAPI.RateLimiting`:

```
Rate limit exceeded for POST /api/auth/login (policy: admin-login-rate-limiting-policy, retry after: 42s)
```

- The policy is `global` when the Swagger limiter rejected the request.
- The log contains the path without the query string (so the PDF preview ticket is not logged), and does not contain the client IP address.
- Many of these warnings from one endpoint mean either abuse or a limit that is too low for real traffic.

## Configuration

Default values are in code, in `RateLimitingOptions`. The `RateLimiting` section is optional: an environment sets only the values it wants to change. Example `appsettings.{Environment}.json`:

```json
"RateLimiting": {
  "PublicRead": { "PermitLimit": 120, "WindowSeconds": 60 },
  "FileDownload": { "PermitLimit": 30 },
  "ImageUpload": { "PermitLimit": 4, "QueueLimit": 32 }
}
```

The same with environment variables (the `VICTORYCENTER_` prefix is required):

```
VICTORYCENTER_RateLimiting__PublicRead__PermitLimit=120
VICTORYCENTER_RateLimiting__PublicRead__WindowSeconds=60
```

| Option | Values |
|---|---|
| `Swagger`, `PublicRead`, `FileDownload`, `RealtimeConnect`, `AdminLogin`,`AdminLogout`, `AuthRefresh`, `InitiateDonation`, `SubmitContactUsForm` | `PermitLimit` (≥ 1), `WindowSeconds` (≥ 1) |
| `ImageUpload` | `PermitLimit` (≥ 1), `QueueLimit` (≥ 0) |

- The values are validated at startup (`RateLimitingOptionsValidator`). If a value is invalid, the application does not start, and the error names the option.
- The values are read through `IOptions`, so configuration added late (for example by `WebApplicationFactory` in tests) is applied.
- `appsettings.IntegrationTests.json` raises `PublicRead`, `FileDownload`, `RealtimeConnect`, `AdminLogin`, `AdminLogout`, `AuthRefresh`, `InitiateDonation` and `SubmitContactUsForm` to `10000`. Many integration tests share one test server and one client address, so the normal limits would make them fail with `429`. The `Swagger` limit is not raised, because a test checks it. Rate-limit tests set their own small limits.

## Limitations

- **Counters are kept in memory of one instance.** They are lost on restart, and with several instances each instance counts separately, so the real limit is `PermitLimit × number of instances`. A shared store (for example Redis) would be needed for exact global limits.
- **Fixed window.** A client can send up to `PermitLimit` requests at the end of one window and `PermitLimit` more at the start of the next one, so a short burst can be twice the limit.
- **Shared addresses.** Users behind one NAT or one corporate proxy share one counter. `PublicRead` is set high enough for normal browsing, but very large offices could reach it. Increase the limit if logs show this.
- **SignalR long polling.** If a client cannot use WebSockets or SSE and falls back to long polling, every poll counts towards `RealtimeConnect`. The server keeps a poll open for up to about 90 seconds when there are no messages, and the hub sends messages only when an admin changes reports, so normal clients stay far below the limit.

## Adding a new endpoint

- **Public anonymous controller:** put it in `Controllers/Public` and inherit `PublicApiController`. It gets the `PublicRead` limit and the `429` response in Swagger automatically.
- **Expensive anonymous action** (files, emails, external services, auth): add a stricter policy:
  1. Add a constant to `RateLimitingPolicyNameConstants`.
  2. Add an option with defaults to `RateLimitingOptions` and to the dictionary in `GetValidationErrors()`.
  3. Register it in `RateLimiterConfiguration.Configure` with `AddIpFixedWindowPolicy`.
  4. Put `[EnableRateLimiting(...)]` and `[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]` on the action.
  5. If integration tests on the shared test server call it many times, raise the limit in `appsettings.IntegrationTests.json`.
- **Anonymous action in an admin controller** (`[AllowAnonymous]`): it does not inherit any limit. Add `[EnableRateLimiting(...)]` to it.
- **Endpoint that must not be limited:** add its route with a reason to `ExcludedRoutes` in `AnonymousEndpointRateLimitCoverageTests`.

The coverage test fails if you forget: see the next section.

## Tests

| Test | What it checks |
|---|---|
| `UnitTests/MiddlewareTests/RateLimiterConfigurationTests` | Client key: IPv4, IPv4-mapped IPv6, IPv6 `/64` grouping, unknown address, forwarding headers are ignored. Options validation. |
| `IntegrationTests/.../RateLimiting/AnonymousEndpointRateLimitCoverageTests` | **Coverage guard.** Reads every endpoint of the real application and fails if an anonymous endpoint (no `[Authorize]`, or `[AllowAnonymous]`) has no rate-limiting policy. Also checks that the exclusion list is not stale and that the endpoint discovery works. |
| `IntegrationTests/.../RateLimiting/RateLimitRejectionResponseTests` | The `429` body, content type and `Retry-After` header, for fixed window and concurrency limits. |
| `IntegrationTests/.../RateLimiting/PublicReadRateLimitTests` | The public limit, separate counters for different clients behind the trusted proxy, no bypass with fake `X-Forwarded-For` from an untrusted address, and that contact-us and donate use their own policies. |
| `IntegrationTests/.../RateLimiting/FileDownloadRateLimitTests` | The file limit on the public download and the preview, and that it does not block other public endpoints. |
| `IntegrationTests/.../RateLimiting/RefreshTokenRateLimitTests` | The refresh-token limit, failed attempts count, separate from the login limit. |
| `IntegrationTests/.../RateLimiting/RealtimeHubRateLimitTests` | Hub negotiate returns `429` after the limit (uses the real `Program`, so it also checks the hub wiring). |
| `IntegrationTests/.../Auth/LoginRateLimitTests`, `.../Payments/PaymentRateLimitTests`, `.../Images/ImageUploadRateLimitTests`, `.../MiddlewareTests/SwaggerBasicAuthMiddlewareTest` | The older login, donation, image upload and Swagger limits. |

### Checking manually

Run the API and send more requests than the limit, for example for `PublicRead` (60 per minute):

```bash
for i in $(seq 1 61); do
  curl -s -o /dev/null -w "%{http_code}\n" http://localhost:5000/api/MainPage
done
```

The last request returns `429`. Use `curl -i` to see the `Retry-After` header and the body. Behind the reverse proxy, check that two different clients get separate counters. If they don't, `ReverseProxy:KnownNetworks` is not set correctly.
