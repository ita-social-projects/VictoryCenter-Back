# Image and PDF Storage Lifecycle

## Scope and Request Flow

Issue #4645 changes storage reliability, not the public API. There are no new
HTTP endpoints, UI, dashboard, CLI application or external scheduler.

1. Reserve an immutable filename in `media.StorageOperations` before writing.
   The independent, transaction-suppressed upload-cleanup intent survives a
   rollback of the request's metadata transaction.
2. Write a temporary file, close it, then rename it to the final filename.
   Temporary ownership includes the complete filename and random suffix.
   Replacement never modifies the previous file.
3. SaveChanges completes the intent together with metadata. Operation and image
   concurrency tokens reject reclaimed intents and stale image replacements.
4. Metadata deletion/replacement queues the old file's deletion in the same
   SQL transaction. Physical deletion happens later, not inside the request.

The existing delete response acknowledges committed metadata. Direct storage
delete methods enqueue work too. FK/concurrency failures roll back metadata and
its outbox together. Rejected upload payloads are not retained or reconstructed:
their persistent intent retries cleanup. Missing content needs backup restoration
or a fresh upload. Do not reuse upload IDs or bypass tracking through ExecuteDelete,
raw SQL or database-side cascades when deleting image/PDF metadata.

## Worker and Reconciliation

BackgroundService claims due operations with optimistic concurrency and a lease.
Reconciliation runs in a separate hosted service with its own scope and DbContext,
so the cleanup loop does not wait for a full inventory scan to finish. Scans are
sequential within each application instance; the configured reconciliation interval
starts after a successful scan. Failed scans retry after PollIntervalSeconds.
Each reconciliation pass uses batches of up to 200 entries. File references,
protecting operations and existing findings are loaded for a batch rather than
queried per file. Findings are saved together with one SaveChanges call per batch;
EF may split that save into multiple SQL commands according to provider limits.
Only a conflicting batch falls back to reloading/saving individual findings, so
an operator's new cleanup link is not overwritten and healthy entries can progress.
Queries do not load the entire storage inventory into memory. The workers still
share database and filesystem resources; batching is not a latency guarantee.
Expired claims can be reclaimed after restart. Deleting an absent file is success,
including after a failed acknowledgement. I/O failures use capped exponential
backoff and a bounded retry budget. Failed operations remain available for recovery.

Before deletion, live references are checked. A referenced file is retained and
the operation fails with `FileStillReferenced`. Expired uploads clean only their
own final/temporary files. Legacy temporary files use explicit orphan cleanup.

Reconciliation runs at startup and periodically; it records missing/unreferenced
files without automatically deleting discovered orphans. Active operations and
their temporary files are protected. SQL grouping and keyset paging deduplicate
shared references. A reappearing resolved finding restarts its age and clears
its previous cleanup link.

For reviewed orphan cleanup, the worker also checks finding/file identity,
unresolved Unreferenced status, minimum finding age, modification age and other
active operations. These requests use the distinct OrphanCleanup operation kind;
a missing finding link fails closed, even after an operator retry. Ordinary
metadata deletion remains independently retryable. Unsafe cleanup fails with
`CleanupNotSafe`; absent files with a valid finding link are
acknowledged idempotently. Logs contain codes, IDs, kinds, attempts and statuses,
not payloads, Base64 or raw exception messages.

Invalid filenames (including names longer than 150 characters) are skipped before
database access rather than aborting reconciliation. File-scan warnings identify
the entry by FileKind and FileNameHash: the uppercase SHA-256 of the exact filename
encoded as UTF-8 (case-sensitive, excluding its directory). Operators can hash
local filenames with the same algorithm to locate the entry without exposing
legacy filenames or full paths in application logs.

## Configuration

Optional defaults:

```json
"StorageLifecycle": {
  "PollIntervalSeconds": 15,
  "BatchSize": 25,
  "MaxAttempts": 8,
  "LeaseSeconds": 300,
  "UploadReservationHours": 24,
  "OrphanMinimumAgeHours": 24,
  "ReconciliationIntervalMinutes": 60,
  "RetryDelaySeconds": 5,
  "MaxRetryDelaySeconds": 3600
}
```

Reservation time must exceed the longest request/upload. All instances must
share storage and DB. Both existing Local/Azure ServiceType names currently
select filesystem storage, not a remote Azure Blob API. Apply the generated
migration before starting this version.

Storage directories are initialized once during application startup. Scoped
services never recreate a missing root. A missing root aborts reconciliation
instead of reporting its references as missing files; restoring the root lets
the next worker iteration retry. Operators must verify the configured paths
and mounted volumes: an existing empty directory alone cannot prove that the
intended volume is mounted.

## Operator Recovery

Use restricted database administration access in the correct environment:

The recovery scripts are run manually by an operator. They are not executed
by application startup, EF migrations or deployment. They queue/reset work;
the background worker performs the actual file operations.

For a read-only count of operations by file kind, operation kind and status,
run [summarize-operations.sql](../scripts/storage/summarize-operations.sql).
It includes historical Completed/Failed operations as well as active work.

```sql
SELECT TOP (100) * FROM media.StorageOperations
WHERE Status = 3 ORDER BY CreatedAt, Id;

SELECT TOP (100) * FROM media.StorageReconciliationFindings
WHERE ResolvedAt IS NULL ORDER BY FirstSeenAt, Id;
```

Inspect LastErrorCode and fix permissions/mounts first. Investigate live metadata
for `FileStillReferenced`, never delete that file manually. Restore missing files
from backup; the next scan resolves the finding.

### Request Orphan Cleanup

Review an unresolved Unreferenced finding and confirm its file is obsolete.
Before queuing, check that both the finding's age (`FirstSeenAt`) and the file's
age (filesystem last-write time in UTC) are at least `OrphanMinimumAgeHours`
(24 hours by default). The worker rechecks both ages before deleting the file.

Declare the reviewed ID and your operator identity in a new standalone SQL batch,
then append [queue-orphan-cleanup.sql](../scripts/storage/queue-orphan-cleanup.sql):

```sql
DECLARE @FindingId uniqueidentifier = 'REPLACE-WITH-REVIEWED-FINDING-ID';
DECLARE @Actor nvarchar(200) = N'REPLACE-WITH-YOUR-OPERATOR-IDENTITY';
-- Append the script in this same batch, without GO.
```

The script atomically links one queued operation to the finding. Repeated or
concurrent requests reuse its ID and retain the original actor. It never deletes
files; the worker validates the actual filesystem state before executing cleanup.

If cleanup is queued before both ages meet the minimum, the worker marks the
operation Failed. Wait until both ages meet the minimum, then use
[retry-failed-operation.sql](../scripts/storage/retry-failed-operation.sql) as
described below. Rerunning the queue script returns the existing operation ID;
it does not retry a failed operation.

### Retry a Failed Operation

After fixing the cause, declare the operation ID and actor, then append
[retry-failed-operation.sql](../scripts/storage/retry-failed-operation.sql):

```sql
DECLARE @OperationId uniqueidentifier = 'REPLACE-WITH-FAILED-OPERATION-ID';
DECLARE @Actor nvarchar(200) = N'REPLACE-WITH-YOUR-OPERATOR-IDENTITY';
-- Append the script in this same batch, without GO.
```

Only Failed operations can be reset. The script records the actor and changes
the version; the worker still rechecks references and cleanup safety. Never reset
active leases or bypass recovery with filesystem deletion. Retain operation
history through the recovery window and back up DB/storage together. Before
rolling back the migration, stop writers/workers and export/drain the queue:
Down removes the recovery ledger.

## Verification

HTTP tests exercise existing image/PDF endpoints and explicitly drain the queue;
the factory disables both workers, whose lifecycles are unit-tested separately.
InMemory does not prove SQL atomicity.

SQL tests cover transactions, constraints, races, recovery scripts, generated
Down/Up and the complete EF migration chain. They require the explicit variable
`VICTORYCENTER_STORAGE_SQL_TEST_CONNECTION` for a dedicated local/test SQL Server
with CREATE/DROP DATABASE permission and otherwise skip. They never use
DB_CONNECTION_STRING and always create generated disposable databases. Do not
target production. Real process kills, mount outages and hardware faults still
require environment-specific verification, not just unit fault simulation.
