# Main Page Localization Query Consolidation (#4985)

## Change

This change is stacked on .NET 10 upgrade PR #4847, branch
`feature/3400_migrate-to-dotnet-10`, base `e66cc0bc2995e6f182120c68ccc0100df19ac7e1`.
The original handler, unchanged from release `c68d2fcfc81a082a2142b407059d47152bbef466`,
loads a page, its translation and up to three block translations in sequential
repository calls. `MainPageRepository.GetLocalizationAsync` now projects the
requested translations through .NET 10 `Queryable.LeftJoin` operators in one SQL
command. EF Core 10 translates the joins; the intermediate anonymous objects are
query expressions, not materialized repository calls. It does not load
source-page text, unrelated languages or tracked entities. The response schema,
translation statuses, null fields and two distinct not-found outcomes are preserved.

## Verification (2026-11-22)

The opt-in `MainPageLocalizationSqlTests` in the existing test project compares
the current handler against the previous read algorithm and its real AutoMapper
profiles. It creates/drops only GUID-named databases on a disposable loopback
SQL Server. No production credentials, schema changes or additional packages are needed.

Checks cover the full response, another language, unknown language, missing page,
missing root translation, partially translated blocks, absent blocks, null text,
outdated status, language metadata, zero/negative IDs, cancellation and an empty change tracker.
The matrix test additionally exercises all 54 combinations: each of three blocks
absent/untranslated/translated, with and without the root translation. Every
combination compares serialized DTOs or error types/messages against the old path
and asserts one command and no tracking.
An EF command interceptor verifies exactly one command for the projected reads.
Four handler unit cases and four opt-in SQL tests passed in each of two runs,
with no skipped tests when the connection variable was configured. A broader
MainPage/SQL-compatibility unit run passed 301 tests (the four SQL tests explicitly
skip without the connection variable); the complete solution built with analyzers.

Regular CI additionally runs 59 `MainPageRepositoryTests` on the existing EF
InMemory provider, including the 54 combinations, missing-page boundaries, unknown
language and cancellation. These execute the production projection without an
external database, rather than relying on the opt-in SQL tests for coverage.
The focused Release coverage run passed 63 tests and measured 100% line/branch
coverage for both the handler and repository. InMemory checks do not replace the
SQL Server translation, command-count or performance checks described below.

An additional real HTTP check launched the compiled WebAPI on Kestrel with a
fresh SQL Server container and an isolated configuration directory (no project
`.env`). Standard startup migrations/seeding completed and a test admin logged in
through `/api/auth/login`. All 67 asserted HTTP requests passed: 54 block/root
translation combinations compared with the legacy DTOs or expected 404 statuses,
missing/invalid JWT (401), unknown/zero/negative IDs and languages, invalid and
overflowing route values, ProblemDetails content/status and readiness. Nullable
text, outdated status and existing translations in the other language were included.
This verifies endpoint behavior, not HTTP performance. The temporary API process,
container/database and storage were removed. The verifier/logs remain under `.tmp`.

## Measured Comparison

Environment: Windows client, SDK 10.0.403, .NET 10.0.14, EF Core 10.0.14; local SQL Server 2022
Developer CU28 (16.0.4325.4) in Docker on Ubuntu 22.04.5, container memory 3 GiB,
SQL memory 2048 MB. The same SQL container served both runs; each test created a
fresh database. Measurements used the uncommitted implementation accompanying
this report, not a separate published benchmark revision. Both legacy and optimized
paths run on the same .NET 10 stack: this is not a .NET 9 versus .NET 10 comparison.

Fixture: one page (respecting the singleton constraint), three optional blocks,
two languages, short representative text, one outdated block translation and
nullable text. The measured request has translations for all three blocks.
Two warmup pairs precede nine measured pairs, alternating old/new execution order.
Each operation uses a fresh context; connection opening and seeding are not timed.
Both paths include repository/handler work and DTO mapping. No artificial network
latency or cold-cache flushing is introduced.

| Run | Sequential median ms | Projection median ms | Actual SQL commands |
| --- | ---: | ---: | ---: |
| 1 | 5.569 | 2.107 | 5 -> 1 |
| 2 | 5.493 | 1.901 | 5 -> 1 |

Raw elapsed samples in milliseconds (before / after):

```text
1 before: 6.3532,6.1050,5.5685,6.0173,5.5028,5.5574,5.3190,6.0558,5.4320
1 after:  2.3573,2.0388,2.0597,2.5602,2.2664,2.1075,2.3590,1.8793,1.8747
2 before: 5.3922,5.5636,5.2993,5.2732,5.4950,5.2401,5.6199,6.3571,5.4931
2 after:  2.3712,2.4417,2.2202,1.8438,2.3545,1.8366,1.8725,1.7463,1.9011
```

This demonstrates fewer round trips and lower local end-to-end medians for the
complete response. Missing-data paths previously used fewer than five commands.
SQL CPU, logical reads, production throughput and endpoint/network latency were
not measured. Small synthetic data, host load and warm caches limit extrapolation;
there is deliberately no fragile latency assertion in CI.

## Repeat

Use the SDK pinned by `global.json`. Set
`VICTORYCENTER_MAIN_LOCALIZATION_SQL_TEST_CONNECTION` to a **disposable** SQL
Server at `127.0.0.1,<port>`, `Initial Catalog=master`, with CREATE/DROP DATABASE
permission. Never commit its credentials. Other targets are rejected. The tests
explicitly skip when the variable is absent; normal CI needs no extra database.

```powershell
dotnet test VictoryCenter/VictoryCenter.UnitTests/VictoryCenter.UnitTests.csproj --filter 'FullyQualifiedName~MainPageLocalizationSqlTests|FullyQualifiedName~GetMainPageLocalizationByLanguageIdHandlerTests' --logger 'trx;LogFileName=main-localization.trx' --results-directory .tmp/main-localization-verification
```

The comparison test writes raw samples, medians, SQL counts and runtime/server
versions into the TRX output. Original local .NET 10 runs are in `.tmp/main-localization-sql-net10`;
generated TRX files and disposable-container scripts are intentionally not committed.
