# Framework Exception Handling: Technical Design

## Context and Decision

The API previously caught HTTP exceptions in custom middleware with separate
JSON-writing paths. Replace that middleware with ASP.NET Core `UseExceptionHandler`
and one registered `IExceptionHandler`, `ApiExceptionHandler`. One handler is enough
for the existing three classifications; multiple handler classes would add ordering
and registration complexity without separate ownership or recovery strategies.

`AddApiExceptionHandling` registers the handler and `IProblemDetailsService`.
The existing `CustomProblemDetailsFactory` continues to choose titles, details,
configured type links and trace IDs. The same framework problem writer handles all
exception responses. If its Accept negotiation declines a writer, explicitly write
JSON using web serializer options and `application/problem+json`. API callers must
not receive an empty error body just because they request XML or HTML.

## Classification and Propagation

| Failure | Strategy | Response / application exception log |
| --- | --- | --- |
| FluentValidation `ValidationException` | Transform | 400, existing semicolon-joined validation detail; no critical log |
| `BadHttpRequestException` with status 413 | Transform | 413, `Payload Too Large`, fixed safe detail; no critical log |
| Unexpected exception, including cancellation unrelated to `RequestAborted` | Transform | 500, existing generic detail; one Critical log with sanitized path/query |
| Request aborted and `OperationCanceledException` | Framework transport handling | Do not attempt an application error body or log an application server failure |
| Exception after response headers/body started | Propagate | Framework cannot rewrite the response; the connection may terminate with a partial response |
| Failure while writing the error response | Propagate | No retry or second JSON body is attempted |

The framework handles an aborted request before invoking registered handlers.
Its internal client-closed status is a transport detail, not a new documented 499
business API contract. The handler also defensively declines aborted cancellation
and started responses if called directly.

This is HTTP error transformation, not domain recovery. Do not add retries around
failed writes or database operations here: retry safety belongs to their owner.
Do not catch exceptions locally merely to reconstruct the same HTTP error.
Application code may keep context-specific recovery, propagation or FluentResults
where already appropriate.

## Compatibility Boundaries

- Preserve current status/title/detail and factory-provided type/trace fields.
  The current factory does not supply a type link for 413; do not invent one.
- Use `application/problem+json` consistently. The old validation path assigned
  that type before a default JSON writer could replace it with `application/json`.
  This normalizes the media type without changing the validation payload schema.
- Validation exceptions still use joined detail, not a new field-grouped schema.
  MVC model binding already uses its own `ValidationProblemDetails` path and is
  not replaced by this handler.
- Leave non-exception controller results, FluentResults mapping, strict JSON
  filter behavior, authentication, rate limiting and health probes unchanged.
- Other `BadHttpRequestException` statuses retain the old generic 500 mapping.
  Broadening classification is a separate API behavior change.
- Do not expose raw internal exception messages, stack traces or database errors
  in the response. Retain the existing exception object in operator logs and the
  existing `IRequestLogSanitizer` for path/query redaction. This does not claim to
  sanitize every exception message or all other framework logging categories.

PR #4805 changes validation error shape independently. If that PR merges first,
port its agreed mapping into this handler and update the corresponding tests;
do not silently discard it or combine that contract change with this refactor.

## Lifetime and Diagnostics

`IExceptionHandler` is singleton. Its dependencies here are singleton services:
`ProblemDetailsFactory`, `IProblemDetailsService`, logger and request-log sanitizer.
Keep request-specific values in local variables and `HttpContext`; do not retain
request state in fields or inject scoped repositories into the handler.

On .NET 9 the built-in middleware emits an exception log even for handled errors.
Keep the application's existing Critical log for unexpected failures and filter
only the `Microsoft.AspNetCore.Diagnostics.ExceptionHandlerMiddleware` logger
category to avoid duplicate exception logs and Error logs for expected validation.
That filter does not suppress metrics or DiagnosticListener events. A response
access log may still be emitted separately; it is not a second exception log.
The existing Console logger and OpenTelemetry ConsoleExporter can render that
single event twice. This task does not reconfigure those exporter destinations.
Started-response failures still propagate to the server's connection handling.

On .NET 10 handled-exception diagnostics are suppressed by default. A
`NET10_0_OR_GREATER` registration explicitly sets `SuppressDiagnosticsCallback`
to false to retain the .NET 9 diagnostics/metrics policy. The category filter still
prevents duplicate textual exception logs. Reassess that policy deliberately if
future telemetry requirements change; do not rely on runtime-version defaults.

References: [ASP.NET Core error handling](https://learn.microsoft.com/aspnet/core/fundamentals/error-handling)
and [the .NET 10 diagnostics change](https://learn.microsoft.com/dotnet/core/compatibility/aspnet-core/10/exception-handler-diagnostics-suppressed).

## Verification Matrix

| Concern | Regression evidence |
| --- | --- |
| Classification, safe detail, trace ID, JSON fallback | `ApiExceptionHandlerTests` |
| Singleton registration, writer acceptance/failure, started response, cancellation | `ApiExceptionHandlerTests` |
| Actual MVC pipeline, 400/413/500, supported and unsupported Accept | `ApiExceptionPipelineTests`, existing error middleware integration tests adapted to the new handler |
| Concurrent request isolation, partial response and client disconnect | `ApiExceptionPipelineTests` |
| Real request-size limit | `ImageRequestSizeLimitTests.CreateImage_RequestBodyExceedsLimit_ShouldReturnPayloadTooLarge` |
| Normal API, MVC binding, FluentResults, auth, SQL failure/recovery | Real requests against disposable Production-configured Docker API and SQL |
| .NET 10 conditional compilation, metrics and log duplication | Isolated host compiling the same handler/configuration sources under net10.0 |

No dependencies, schema migrations, business endpoints or frontend changes are
required. Test-only exception endpoints must never be added to the production API.
