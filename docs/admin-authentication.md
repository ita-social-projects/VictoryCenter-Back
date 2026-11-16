# Admin Authentication

`AuthenticationConfiguration.AddAdminAuthentication` owns Identity, account lockout,
JWT options, Bearer authentication and the default authorization policy.
`AuthHelper` creates fresh validation parameters from the same typed `JwtOptions`
used by `TokenService`; it does not reread raw configuration.

## Configuration

The existing `JwtOptions` section and environment-variable names are unchanged.
Issuer and audience must be nonblank. Access lifetime remains 1-60 minutes and
refresh lifetime 1-30 days. Both signing secrets must contain at least 32 UTF-8
bytes, the HS256 minimum. This is a length check, not an entropy guarantee: use
independently generated random secrets, not passwords. Never commit them.

Check the byte lengths of `JWTOPTIONS_SECRETKEY` and
`JWTOPTIONS_REFRESH_TOKEN_SECRETKEY` in the target environment before deployment.
Options validation rejects invalid settings during startup and reports property
names without including key values. Existing missing-environment-variable checks
remain in place. No signing keys are automatically rotated by this change.

## Access Boundaries

- Existing `[Authorize]` endpoints use the explicit default policy: Bearer scheme
  and an authenticated principal. Identity cookies are not accepted for these APIs.
- This policy does not introduce roles. The existing access-token event still
  checks the admin email and current security-stamp hash against the database.
- Access validation requires the configured issuer, audience, signature, expiry
  and HS256 algorithm, retaining the 30-second clock skew.
- Refresh validation uses the separate refresh key and the same issuer, audience
  and algorithm restrictions. Its lifetime check remains in the refresh-session
  handler against the persisted session expiry; extracting refresh claims does
  not authorize a refresh by itself.
- Refresh rotation, reuse detection, logout revocation and cookie settings are
  unchanged. Existing tokens signed with the configured HS256 keys remain valid,
  subject to the existing stamp and session checks.
- No authenticated fallback policy is installed: public APIs, login, refresh and
  health probes retain their existing anonymous access.

## Regression Checks

`AuthenticationConfigurationTests` checks startup validation, UTF-8 key boundaries,
default schemes/policy and account lockout. `JwtValidationTests` checks valid tokens,
wrong issuer/audience/key, expiration, future not-before, missing expiration,
disallowed/unsigned algorithms, key separation and clock skew. Existing token,
login, refresh, logout and security-stamp event tests remain applicable.

`AuthenticationBoundaryTests` checks a correctly protected Identity cookie cannot
authorize an admin endpoint and that public API/health routes remain anonymous.
The existing `AuthControllerTests` covers refresh rotation/replay and logout.

For HTTP verification, use disposable SQL and API containers with synthetic keys
and an admin, never the shared deployment database. Exercise login, admin access,
negative JWT variants, refresh rotation, logout and post-logout token rejection.
The change does not require schema migrations, frontend updates or dependency changes.
