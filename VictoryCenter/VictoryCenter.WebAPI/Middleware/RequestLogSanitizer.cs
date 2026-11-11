using Microsoft.Extensions.Options;

namespace VictoryCenter.WebAPI.Middleware;

public interface IRequestLogSanitizer
{
    /// <summary>
    /// Builds a log-safe representation of the request path and query string,
    /// with the values of sensitive query parameters masked.
    /// </summary>
    /// <param name="request">The HTTP request whose path and query string should be logged.</param>
    /// <returns>
    /// The request path followed by the query string (if any), in the form
    /// "{path}{?query}", with sensitive values replaced by "[REDACTED]".
    /// </returns>
    string GetSafePathAndQuery(HttpRequest request);
}

public sealed class RequestLogSanitizer : IRequestLogSanitizer
{
    public const string Mask = "[REDACTED]";

    public static readonly string[] DefaultSensitiveQueryParameters =
    [
        "ticket",
        "token",
        "access_token",
        "id_token",
        "refresh_token",
        "api_key",
        "apikey",
        "key",
        "signature",
        "sig",
        "code",
        "password",
        "secret",
        "auth",
        "authorization"
    ];

    private readonly HashSet<string> _sensitive;

    public RequestLogSanitizer(IOptions<LogRedactionOptions> options)
    {
        _sensitive = new HashSet<string>(
            DefaultSensitiveQueryParameters.Concat(options.Value.AdditionalSensitiveQueryParameters),
            StringComparer.OrdinalIgnoreCase);
    }

    public string GetSafePathAndQuery(HttpRequest request)
    {
        var path = request.Path.Value ?? string.Empty;

        if (!request.QueryString.HasValue)
        {
            return path;
        }

        var parts = request.QueryString.Value!
            .TrimStart('?')
            .Split('&', StringSplitOptions.RemoveEmptyEntries)
            .Select(RedactPart);

        return $"{path}?{string.Join('&', parts)}";
    }

    private string RedactPart(string part)
    {
        var separator = part.IndexOf('=');
        var rawName = separator < 0 ? part : part[..separator];

        // Decode so "%74icket" or "ticket" variants can't dodge the match.
        var name = Uri.UnescapeDataString(rawName.Replace('+', ' '));

        return _sensitive.Contains(name)
            ? $"{rawName}={Mask}"
            : part;
    }
}
