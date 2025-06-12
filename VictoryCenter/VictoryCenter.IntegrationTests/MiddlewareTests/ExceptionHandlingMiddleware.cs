using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using VictoryCenter.IntegrationTests.Utils;

namespace VictoryCenter.IntegrationTests.MiddlewareTests;

public class ExceptionHandlingMiddleware
    : IClassFixture<VictoryCenterWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly InMemoryLoggerProvider _loggerProvider;

    public ExceptionHandlingMiddleware(VictoryCenterWebApplicationFactory<Program> factory)
    {
        _loggerProvider = factory.Services
            .GetServices<ILoggerProvider>()
            .OfType<InMemoryLoggerProvider>()
            .Single();

        _client = factory.CreateClient();
    }

    [Fact]
    public async Task InvokeAsync_UnhandledException_ShouldWriteProblemDetailsAndLogAtCriticalLevel()
    {
        var response = await _client.GetAsync("/api/test/exception");
        var content = await response.Content.ReadAsStringAsync();

        Assert.Equal(500, (int)response.StatusCode);

        var pd = JsonSerializer.Deserialize<ProblemDetails>(content);

        Assert.Equal(500, pd!.Status);
        Assert.Equal("Internal Server Error", pd.Title);
        Assert.Contains("error occurred", pd.Detail);

        var categoryName = typeof(ExceptionHandlingMiddleware).FullName;
        var log = _loggerProvider.Entries.Where(e => e.Category == categoryName).Last();

        Assert.Contains("Unhandled exception occured while processing request", log.Message);
    }
}
