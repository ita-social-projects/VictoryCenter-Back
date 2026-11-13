# Audit Trail

This document describes how administrative changes are recorded, what is excluded from the records, and how to read them.

## What is recorded

Every create, update and delete of an audited entity adds one row to the `audit.AuditLogs` table.

| Column | Meaning |
|---|---|
| `ActorId` | Id of the admin who made the change (`AspNetUsers.Id`). `NULL` when the change was not made by an authenticated admin request. |
| `Operation` | `Create`, `Update` or `Delete`. |
| `EntityType` | Entity class name, for example `FaqQuestion` or `FaqQuestionLocalization`. |
| `EntityId` | Primary key. Composite keys are written as `Name=value;Name=value`, for example `EntityId=5;LanguageId=2`. |
| `Timestamp` | UTC time of the change. |
| `CorrelationId` | ASP.NET Core request id (`HttpContext.TraceIdentifier`). All rows written by one request share it. It is the same value as `RequestId` in the application logs. |
| `Changes` | JSON with old and new values of the changed audited fields, for example `{"Status":{"Old":"Draft","New":"Published"}}`. `NULL` when none of the changed fields may be recorded. |

Publishing, unpublishing and reordering are recorded as `Update`. They are visible in `Changes` as a change of `Status` or `Priority`.

## How it works

Records are written by `AuditInterceptor`, an EF Core `SaveChangesInterceptor`. It runs on every `SaveChanges` call, so no command handler has to write audit records itself.

- `Update` and `Delete` records are added to the same `SaveChanges` as the change itself. They are saved in one database transaction: either both are saved or neither is.
- `Create` records need the generated id, so they are saved by a second `SaveChanges` right after the first one. Both calls run in one transaction. If the handler already opened a transaction, it is used. Otherwise the interceptor opens its own.
- If an operation fails and its transaction is rolled back, its audit records are rolled back too. A failed operation never leaves a record that says the change succeeded.
- For `Update`, old values are read from the database, because most handlers load entities without tracking. An update that does not change any value is not recorded.

Archiving a video review uses `ExecuteUpdateAsync`, which bypasses `SaveChanges`. `DeleteVideoReviewHandler` writes its `Delete` record explicitly through `IAuditLogFactory`, in the same transaction as the archiving.

## Which entities are audited

Auditing is opt-in, with two attributes from `VictoryCenter.DAL.Entities.Attributes`:

- `[Auditable]` on an entity class: changes of this entity are recorded.
- `[Audited]` on a property: old and new values of this property may be written to `Changes`.

Both attributes are inherited. `[Auditable]` on `LocalizationBase<T>` covers all translation entities, and `[Auditable]` on `ProgramSectionContent`, `HistorySectionContent` and `WhoWeAreContent` covers all their content blocks.

When you add a new entity that admins can change:

1. Add `[Auditable]` to the class.
2. Add `[Audited]` to every property that is safe to record (see the masking policy below).
3. Do not add `[Audited]` to navigation properties. They are ignored.

These entities are not audited:

| Entity | Reason |
|---|---|
| `AdminUser` | Contains credentials and tokens. |
| `RefreshTokenSession` | Contains refresh token hashes. |
| `AuditLog` | The audit records themselves. |
| `StorageOperation`, `StorageReconciliationFinding` | Internal storage bookkeeping, not changed by admins. |
| `VisitorPage` | Seeded data, admins cannot change it. |
| `Backup*`, `Published*` report entities | Technical copies made by the publish and cancel commands. The publish itself is visible as an `Update` of `ReportFundsExpendituresSettings.HasUnpublishedChanges`. |

## Masking policy

Only properties marked with `[Audited]` are written to `Changes`. Everything else is excluded. A new property is excluded until someone marks it on purpose.

Never mark:

- credentials and tokens;
- payment details: IBAN, SWIFT, EDRPOU, account numbers and `SupportOptions.Value`;
- personal data of team members, such as `TeamMember.Email`;
- technical fields such as row versions, storage versions and service timestamps.

File contents are never recorded, because entities store only file metadata (name, URL, type, size). The files themselves are in the blob storage.

When only excluded fields change, the record is still written, with `Changes` set to `NULL`. It shows who changed the entity and when, without the values.

## Protection of audit records

- There are no API endpoints for audit records.
- `IRepositoryWrapper.AddAuditLog` can only add records. There is no update or delete method.
- `AuditInterceptor` throws an exception when an audit record is modified or deleted through EF Core.

Only maintainers with direct database access can read or delete audit records.

## Retention

Audit records are kept indefinitely. There is no automatic cleanup. The table grows slowly, because only admin changes are recorded.

Records can be deleted only manually, with SQL, by maintainers with database access.

## Reading audit records

History of one entity:

```sql
SELECT Timestamp, ActorId, Operation, Changes
FROM audit.AuditLogs
WHERE EntityType = 'FaqQuestion' AND EntityId = '42'
ORDER BY Timestamp;
```

All changes in a period, with the admin email:

```sql
SELECT a.Timestamp, u.Email, a.Operation, a.EntityType, a.EntityId, a.Changes
FROM audit.AuditLogs a
LEFT JOIN dbo.AspNetUsers u ON u.Id = TRY_CAST(a.ActorId AS int)
WHERE a.Timestamp >= '2026-11-01' AND a.Timestamp < '2026-12-01'
ORDER BY a.Timestamp;
```

Everything changed by one request:

```sql
SELECT Operation, EntityType, EntityId, Changes
FROM audit.AuditLogs
WHERE CorrelationId = '0HNQ11Q0HBCVO:0000002B'
ORDER BY Id;
```

## Known limitations

- Bulk operations (`ExecuteUpdateAsync`, `ExecuteDeleteAsync`) bypass `SaveChanges` and are not recorded. They are used to delete dependent translations when a section translation is deleted, and to clear draft records when report changes are cancelled. The main action is still recorded.
- The program-category links (`ProgramsProgramCategories` table) have no entity class and are not recorded.
- Rows removed by a database cascade delete are not recorded unless they were loaded into EF Core. The deleted parent is recorded.
- Reordering writes two records per item, because priorities are changed in two passes through temporary negative values.
- Every audited `Update` reads the old values with one extra `SELECT` per entity.
- `ActorId` is `NULL` for changes made outside an HTTP request, and for up to 15 minutes after deployment, until access tokens issued before the `NameIdentifier` claim was added expire.