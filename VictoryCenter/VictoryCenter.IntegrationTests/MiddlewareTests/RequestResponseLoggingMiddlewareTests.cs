using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.WebAPI.Middleware;

namespace VictoryCenter.IntegrationTests.MiddlewareTests;

public class RequestResponseLoggingMiddlewareTests
    : IClassFixture<VictoryCenterWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly InMemoryLoggerProvider _loggerProvider;

    public RequestResponseLoggingMiddlewareTests(VictoryCenterWebApplicationFactory<Program> factory)
    {
        _loggerProvider = factory.Services
            .GetServices<ILoggerProvider>()
            .OfType<InMemoryLoggerProvider>()
            .Single();

        _client = factory.CreateClient();
    }

    [Fact]
    public async Task InvokeAsync_Status200_ShouldLogAtInformationLevel()
    {
        var response = await _client.GetAsync("/api/Test/GetAllTestData");
        Assert.Equal(200, (int)response.StatusCode);

        var categoryName = typeof(RequestResponseLoggingMiddleware).FullName;
        var entry = _loggerProvider.Entries.Last(e => e.Category == categoryName);
        Assert.Equal(LogLevel.Information, entry.LogLevel);
        Assert.Contains("200", entry.Message);
    }

    [Fact]
    public async Task InvokeAsync_Status404_ShouldLogAtWarningLevel()
    {
        var response = await _client.GetAsync("/api/Test/GetTestData/-1");
        Assert.Equal(404, (int)response.StatusCode);

        var categoryName = typeof(RequestResponseLoggingMiddleware).FullName;
        var entry = _loggerProvider.Entries.Last(e => e.Category == categoryName);
        Assert.Equal(LogLevel.Warning, entry.LogLevel);
        Assert.Contains("404", entry.Message);
    }

    [Fact]
    public async Task InvokeAsync_Status500_ShouldLogAtErrorLevel()
    {
        var response = await _client.GetAsync("/api/test/internalservererror");
        Assert.Equal(500, (int)response.StatusCode);

        var categoryName = typeof(RequestResponseLoggingMiddleware).FullName;
        var entry = _loggerProvider.Entries.Last(e => e.Category == categoryName);
        Assert.Equal(LogLevel.Error, entry.LogLevel);
        Assert.Contains("500", entry.Message);
    }
}
