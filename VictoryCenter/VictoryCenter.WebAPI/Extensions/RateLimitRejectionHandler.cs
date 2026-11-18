using System.Diagnostics;
using System.Globalization;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Net.Http.Headers;

namespace VictoryCenter.WebAPI.Extensions;

/// <summary>
/// Writes the single 429 response used by every rate-limiting policy and the global limiter.
/// </summary>
public static class RateLimitRejectionHandler
{
    public static readonly string Title = "Too Many Requests";
    public static readonly string GlobalLimiterName = "global";
    private static readonly string ProblemJsonContentType = "application/problem+json";
    private static readonly string LoggerCategory = "VictoryCenter.WebAPI.RateLimiting";

    public static async ValueTask OnRejectedAsync(OnRejectedContext context, CancellationToken cancellationToken)
    {
        var httpContext = context.HttpContext;
        var response = httpContext.Response;

        int? retryAfterSeconds = null;
        if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out TimeSpan retryAfter))
        {
            retryAfterSeconds = Math.Max(1, (int)Math.Ceiling(retryAfter.TotalSeconds));
            response.Headers[HeaderNames.RetryAfter] = retryAfterSeconds.Value.ToString(CultureInfo.InvariantCulture);
        }

        var detail = retryAfterSeconds is null
            ? "Too many requests. Please try again later."
            : $"Too many requests. Please try again in {retryAfterSeconds} seconds.";

        var policyName = httpContext.GetEndpoint()?.Metadata.GetMetadata<EnableRateLimitingAttribute>()?.PolicyName
            ?? GlobalLimiterName;

        httpContext.RequestServices
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger(LoggerCategory)
            .LogWarning(
                "Rate limit exceeded for {Method} {Path} (policy: {Policy}, retry after: {RetryAfterSeconds}s)",
                httpContext.Request.Method,
                httpContext.Request.Path,
                policyName,
                retryAfterSeconds);

        response.StatusCode = StatusCodes.Status429TooManyRequests;
        await response.WriteAsJsonAsync(
            CreateProblemDetails(httpContext, detail),
            options: null,
            contentType: ProblemJsonContentType,
            cancellationToken);
    }

    private static ProblemDetails CreateProblemDetails(HttpContext httpContext, string detail)
    {
        var factory = httpContext.RequestServices.GetService<ProblemDetailsFactory>();
        if (factory is not null)
        {
            return factory.CreateProblemDetails(
                httpContext,
                statusCode: StatusCodes.Status429TooManyRequests,
                title: Title,
                detail: detail);
        }

        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status429TooManyRequests,
            Title = Title,
            Detail = detail,
        };
        problemDetails.Extensions["traceId"] = Activity.Current?.Id ?? httpContext.TraceIdentifier;

        return problemDetails;
    }
}
