# Admin Account Lockout

This document describes how the admin login is protected against password guessing and how to unlock an account.

## How it works

The lockout uses the built-in ASP.NET Identity lockout.

- Every wrong password for an existing admin increases the failed attempts counter.
- When the counter reaches `MaxFailedAccessAttempts`, the account is locked for `LockoutMinutes`.
- While the account is locked, login is rejected even with the correct password.
- After the lockout time ends, the admin can log in again.
- A successful login resets the counter to 0.

The response is always the same `401 Unauthorized` with the same message: for a wrong password, an unknown email and a locked account. This way a client cannot find out whether an account exists or is locked.

The lockout works together with the login rate limit by IP. The rate limit slows down one client, the lockout protects one account.

## Configuration

Default values are in `appsettings.json`:

```json
"AccountLockout": {
  "MaxFailedAccessAttempts": 5,
  "LockoutMinutes": 15
}
```

Both values must be 1 or more. If a value is missing or invalid, the application does not start.

To change the values for one environment, override them in `appsettings.{Environment}.json` or with environment variables:

```
AccountLockout__MaxFailedAccessAttempts=5
AccountLockout__LockoutMinutes=15
```

## Logs

Every failed login is logged as a warning with the client IP. A login attempt for a locked account is logged with the admin id and the client IP.

## Unlocking an account

Option 1: wait until `LockoutMinutes` pass. The account unlocks automatically.

Option 2: unlock it manually in the database:

```sql
UPDATE dbo.AspNetUsers
SET LockoutEnd = NULL, AccessFailedCount = 0
WHERE NormalizedEmail = UPPER('admin@example.com');
```

Replace `admin@example.com` with the email of the locked admin.