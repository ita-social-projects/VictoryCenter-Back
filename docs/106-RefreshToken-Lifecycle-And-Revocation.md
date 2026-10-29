# Refresh Token Lifecycle and Session Revocation

This document describes the refresh-token lifecycle, storage model, reuse mitigation, cookie configuration, and operational considerations for signing-key rotation.

## 1. Session Storage Model

Refresh-token sessions are tracked in the `RefreshTokenSessions` table (`VictoryCenter.DAL`):

- **`AdminUserId`**: User identifier.
- **`FamilyId`**: `Guid` grouping the token chain of a single login session.
- **`TokenHash`**: SHA-256 hash of the refresh token (`IX_RefreshTokenSessions_TokenHash` unique index). Plaintext tokens are never stored.
- **`ExpiresAt`**: Token expiration timestamp (default 7 days).
- **`IsRevoked`**: Revocation flag (`true` after rotation, logout, or suspected reuse).
- **`RevokedAt`**: Revocation timestamp.
- **`RevocationReason`**: Reason for revocation (`Rotated`, `Logout`, `SupersededByNewLogin`, `SuspectedTokenReuse`, `Expired`).
- **`ReplacedByTokenHash`**: Hash of the next token in the rotation chain.

---

## 2. Token Lifecycle

### 2.1. Login (`/api/auth/login`)
1. Validates administrator credentials.
2. Revokes previous active sessions for the user (`SupersededByNewLogin`).
3. Generates a new `FamilyId` and attaches it as a token claim.
4. Stores the SHA-256 token hash in `RefreshTokenSessions` with `IsRevoked = false`.
5. Updates `AdminUser.RefreshToken` and `AdminUser.RefreshTokenValidTo`.
6. Returns access token in response body and refresh token in an `HttpOnly` cookie.

### 2.2. Token Refresh (`/api/auth/refresh-token`)
1. Reads the refresh token from cookie and validates token claims and signature.
2. Hashes the token (SHA-256) and looks up the session in `RefreshTokenSessions`:
   - **Missing**: Deletes cookie and returns `401 Unauthorized`.
   - **Expired (`ExpiresAt <= UtcNow`)**: Marks session revoked (`Expired`), deletes cookie, returns `401 Unauthorized`.
   - **Already Revoked (`IsRevoked == true`)**: Triggers token reuse detection (see Section 3).
3. If valid:
   - Marks current session as `IsRevoked = true`, `RevocationReason = "Rotated"`, `ReplacedByTokenHash = newTokenHash`.
   - Creates a new session record under the same `FamilyId` with the new token hash.
   - Updates `AdminUser.RefreshToken` and `AdminUser.RefreshTokenValidTo`.
   - Returns a new access token and appends the rotated refresh token cookie.

### 2.3. Logout (`/api/auth/logout`)
1. Validates the authenticated user.
2. If a refresh-token cookie is provided, finds the session and revokes the entire `FamilyId` (`Logout`).
3. Revokes all active sessions for the user (`RevokeAdminSessionsAsync`).
4. Clears `AdminUser.RefreshToken` and `AdminUser.RefreshTokenValidTo`.
5. Emits an expired cookie (`expires=Thu, 01 Jan 1970 00:00:00 GMT`) to delete it from the client.

---

## 3. Token Reuse Detection

When a refresh request presents a token whose session is already marked `IsRevoked == true`:

1. **Family Revocation**: Calls `RevokeFamilyAsync(session.FamilyId, "SuspectedTokenReuse")` to immediately invalidate all tokens in that family.
2. **Clear User Session**: Clears `AdminUser.RefreshToken` if the user's active token belongs to the revoked family.
3. **Audit Logging**: Logs a warning with `AdminUserId`, `FamilyId`, and prior `RevocationReason` without logging token values.
4. **Cookie Removal & Rejection**: Deletes the client cookie and returns `401 Unauthorized`.

---

## 4. Cookie Configuration

Refresh tokens are sent in an HTTP cookie configured with:

- **`HttpOnly = true`**: Prevents client-side script access via `document.cookie`.
- **`Secure = true`**: Restricts transmission to HTTPS connections.
- **`SameSite = SameSiteMode.Strict`**: Prevents cookie transmission in cross-site requests.
- **`Path = /api/auth/refresh-token`**: Scopes cookie transmission exclusively to the refresh endpoint.
- **`Expires`**: Matches backend lifetime (`JwtOptions.RefreshTokenLifetimeInDays`).

---

## 5. Error Consistency & Concurrency

- **Consistent 401 Responses**: All authentication and refresh failures return a uniform `401 Unauthorized` ProblemDetails. Internal token status (expired vs. revoked vs. missing) is not exposed to the client.
- **Concurrency Protection**: Unique index on `TokenHash` and ASP.NET Core Identity `ConcurrencyStamp` on `AdminUser` prevent concurrent requests from establishing parallel valid token chains. Requests losing a race condition fail safely with `401 Unauthorized`.

---

## 6. Operational Considerations for Signing-Key Rotation

To rotate JWT signing keys without terminating active sessions:

1. **Dual-Key Verification**: Configure the authentication middleware with both the new key and the current key in `IssuerSigningKeys` (`TokenValidationParameters`).
2. **Active Key Cutover**: Switch `Jwt:SecretKey` and `Jwt:RefreshTokenSecretKey` to sign newly issued tokens with the new key.
3. **Retire Old Key**: After the maximum refresh token lifetime (7 days) has passed, all legitimate clients will have rotated to tokens signed with the new key, and the old key can be safely removed.
