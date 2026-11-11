using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using VictoryCenter.WebAPI.Middleware;

namespace VictoryCenter.UnitTests.MiddlewareTests;

public class RequestLogSanitizerTests
{
    public static IEnumerable<object[]> DefaultSensitiveParameters =>
        RequestLogSanitizer.DefaultSensitiveQueryParameters.Select(p => new object[] { p });

    [Theory]
    [InlineData("/api/pdf/preview", "?ticket=abc123", "/api/pdf/preview?ticket=[REDACTED]")]
    [InlineData("/api/pdf/preview", "?id=5&Ticket=abc&page=2", "/api/pdf/preview?id=5&Ticket=[REDACTED]&page=2")]
    [InlineData("/api/pdf/preview", "?TICKET=abc", "/api/pdf/preview?TICKET=[REDACTED]")]
    [InlineData("/api/pdf/preview", "?%74icket=abc", "/api/pdf/preview?%74icket=[REDACTED]")]
    [InlineData("/api/pdf/preview", "?ticket", "/api/pdf/preview?ticket=[REDACTED]")]
    [InlineData("/api/pdf/preview", "?ticket=", "/api/pdf/preview?ticket=[REDACTED]")]
    [InlineData("/api/pdf/preview", "?ticket=a=b", "/api/pdf/preview?ticket=[REDACTED]")]
    [InlineData("/api/pdf/preview", "?ticket=a&ticket=b", "/api/pdf/preview?ticket=[REDACTED]&ticket=[REDACTED]")]
    [InlineData("/api/x", "?token=t&password=p&page=1", "/api/x?token=[REDACTED]&password=[REDACTED]&page=1")]
    public void GetSafePathAndQuery_RedactsSensitiveValues(string path, string query, string expected)
    {
        var sanitizer = CreateSanitizer();

        var result = sanitizer.GetSafePathAndQuery(CreateRequest(path, query));

        Assert.Equal(expected, result);
    }

    [Theory]
    [MemberData(nameof(DefaultSensitiveParameters))]
    public void GetSafePathAndQuery_RedactsEveryDefaultParameter(string name)
    {
        var sanitizer = CreateSanitizer();

        var result = sanitizer.GetSafePathAndQuery(CreateRequest("/api/x", $"?{name}=secret-value&page=1"));

        Assert.Equal($"/api/x?{name}=[REDACTED]&page=1", result);
        Assert.DoesNotContain("secret-value", result);
    }

    [Theory]
    [InlineData("/api/items", "?page=1&size=10")]
    [InlineData("/api/items", "?ticketCount=3&monkey=1")]
    [InlineData("/api/items", "?search=a+b&filter=%D0%A2")]
    [InlineData("/api/items", "")]
    public void GetSafePathAndQuery_LeavesNonSensitiveQueryUnchanged(string path, string query)
    {
        var sanitizer = CreateSanitizer();

        var result = sanitizer.GetSafePathAndQuery(CreateRequest(path, query));

        Assert.Equal(path + query, result);
    }

    [Fact]
    public void GetSafePathAndQuery_PreservesParameterOrder()
    {
        var sanitizer = CreateSanitizer();

        var result = sanitizer.GetSafePathAndQuery(CreateRequest("/api/x", "?b=2&ticket=s&a=1&c=3"));

        Assert.Equal("/api/x?b=2&ticket=[REDACTED]&a=1&c=3", result);
    }

    [Fact]
    public void GetSafePathAndQuery_RedactsConfiguredAdditionalParameters()
    {
        var sanitizer = CreateSanitizer("previewTicket");

        var result = sanitizer.GetSafePathAndQuery(CreateRequest("/api/x", "?PREVIEWTICKET=abc&page=1"));

        Assert.Equal("/api/x?PREVIEWTICKET=[REDACTED]&page=1", result);
    }

    [Fact]
    public void GetSafePathAndQuery_KeepsDefaultsWhenAdditionalParametersConfigured()
    {
        var sanitizer = CreateSanitizer("previewTicket");

        var result = sanitizer.GetSafePathAndQuery(CreateRequest("/api/x", "?ticket=abc"));

        Assert.Equal("/api/x?ticket=[REDACTED]", result);
    }

    [Fact]
    public void GetSafePathAndQuery_ReturnsEmptyPath_WhenPathNotSet()
    {
        var sanitizer = CreateSanitizer();

        var result = sanitizer.GetSafePathAndQuery(new DefaultHttpContext().Request);

        Assert.Equal(string.Empty, result);
    }

    private static RequestLogSanitizer CreateSanitizer(params string[] additional) =>
        new(Options.Create(new LogRedactionOptions { AdditionalSensitiveQueryParameters = additional }));

    private static HttpRequest CreateRequest(string path, string query)
    {
        var context = new DefaultHttpContext();
        context.Request.Path = path;
        context.Request.QueryString = new QueryString(query);
        return context.Request;
    }
}
