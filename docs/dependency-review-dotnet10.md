# Dependency Review: .NET 10 Upgrade

Issue #3400 / PR #4847. Review baseline: `f904679d52a19fd68143b57a7db608a68e863d32`.
This note records the existing upgrade decisions; it does not introduce another
package upgrade or claim that all dependencies are free of vulnerabilities.

## Versions and Decisions

| Component | Selected version | Decision and trade-off |
|---|---|---|
| Target framework | net10.0, all six projects | Upgrade application and test projects together rather than mixing EF/test framework generations |
| SDK | 10.0.403 | One global.json used by developers, CI and Docker; latestPatch permits servicing within this feature band, not an arbitrary feature-band upgrade |
| ASP.NET runtime / container | 10.0.14-noble | Explicit runtime patch and OS family; predictable baseline but requires deliberate servicing updates, and tags are not immutable digests |
| EF Core, Microsoft.Extensions and MVC testing | 10.0.14 | Keep related runtime, design-time and test dependencies on the same servicing baseline |
| dotnet-ef | 10.0.14 | Local tool manifest avoids accidentally running an installed EF 9 CLI against EF 10 |
| MockQueryable.Moq | 10.0.13 | EF 10-compatible query mocks; its package metadata requires Moq >= 4.21.0 |
| Moq / Moq.EntityFrameworkCore | 4.21.0 / 10.0.0.2 | Meet the mock adapter constraint and EF 10 compatibility instead of retaining Moq 4.20.72 |
| System.Security.Cryptography.Pkcs | 10.0.14 | Explicit transitive override in DAL; review its resolved graph and advisories before future servicing rather than assuming the framework upgrade fixes every advisory |

Unrelated library majors are deliberately unchanged to limit regression scope.
The unused Microsoft.AspNetCore.OpenApi reference is removed; Swashbuckle remains
the documentation generator, so this is not a Swagger contract migration.

## Breaking Changes and Risks

- Trusted proxy configuration replaces deprecated KnownNetworks usage with
  KnownIPNetworks. Retain existing CIDR settings and trust boundaries; incorrect
  forwarding configuration affects HTTPS, secure cookies and client-IP limits.
- EF query translation can differ even when the model is unchanged. Dedicated
  SqlServerCompatibilityTests check model/snapshot parity, collection filtering,
  ordering, grouping and aggregation using the SQL Server provider. InMemory
  tests alone cannot establish production SQL compatibility or performance.
- Test adapters and compiler/analyzer behavior also change with the SDK. Existing
  StyleCop record-declaration analyzer failures and other warnings are not fixed
  by suppressing them in this upgrade.
- AutoMapper 12.0.1 and ImageSharp 3.1.12 advisory warnings remain a separate
  dependency-remediation task. This review does not assert patched advisory ranges
  for them or present the SDK upgrade as their fix.
- The pending NuGet lock-file PR #4893 must be synchronized and its graphs
  regenerated for net10.0 after combining changes; net9.0 lock graphs are not a
  valid upgrade baseline.

## Verification and Release Decision

The PR contains SQL compatibility and forwarded-header regression tests. Earlier
local upgrade verification recorded builds, unit tests, HTTP/SQL compatibility,
container health and restart checks; those experiments were not rerun for this
documentation-only change. Their local logs are not a substitute for passing CI
on the final combined release SHA. Full integration tests were not run locally.

Before merge: synchronize release, review current package audit output, run the
existing CI suite and confirm real environment secrets, proxy and mounted storage.
No application schema migration is introduced by the framework upgrade. Developers
and DbUpdate hosts need the selected SDK/runtime; container-only hosts do not.

Deploy the verified image, retain its predecessor and inspect readiness/smoke
checks. Application rollback uses the prior image, not a blanket package downgrade
in place. Do not assume rolling back the runtime reverses independent database
migrations: inspect any migrations merged alongside the upgrade before rollback.
