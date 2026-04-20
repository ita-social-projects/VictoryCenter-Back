using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;
using VictoryCenter.WebAPI.Middleware;

namespace VictoryCenter.IntegrationTests.MiddlewareTests;

[Collection("SharedIntegrationTests")]
public class ExceptionHandlingMiddlewareTests
{
    private readonly HttpClient _client;
    private readonly InMemoryLoggerProvider _loggerProvider;

    public ExceptionHandlingMiddlewareTests(IntegrationTestDbFixture fixture)
    {
        _loggerProvider = new InMemoryLoggerProvider();

        var customFactory = fixture.Factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddProvider(_loggerProvider);
            });

            builder.ConfigureServices(services =>
            {
                services
                    .AddControllers()
                    .AddApplicationPart(typeof(FakeErrorController).Assembly)
                    .AddControllersAsServices();
            });
        });

        _client = customFactory.CreateClient();
    }

    [Fact]
    public async Task InvokeAsync_ShouldReturn500AndLogCritical_WhenUnhandledExceptionThrown()
    {
        var response = await _client.GetAsync("/api/Test/ThrowException");
        var content = await response.Content.ReadAsStringAsync();

        Assert.Equal(500, (int)response.StatusCode);

        var pd = JsonSerializer.Deserialize<ProblemDetails>(content);

        Assert.NotNull(pd);
        Assert.Equal(500, pd.Status);
        Assert.Equal("Internal Server Error", pd.Title);
        Assert.Contains("error occurred", pd.Detail);

        var categoryName = typeof(ExceptionHandlingMiddleware).FullName;
        var log = _loggerProvider.Entries.Last(e => e.Category == categoryName);

        Assert.Equal(LogLevel.Critical, log.LogLevel);
        Assert.Contains("Unhandled exception occured while processing request", log.Message);
    }

    [Fact]
    public async Task InvokeAsync_ShouldReturn400WithValidationErrors_WhenValidationExceptionThrown()
    {
        var response = await _client.GetAsync("/api/Test/ThrowValidationException");
        var content = await response.Content.ReadAsStringAsync();

        Assert.Equal(400, (int)response.StatusCode);

        var pd = JsonSerializer.Deserialize<ProblemDetails>(content);

        Assert.NotNull(pd);
        Assert.Equal(400, pd.Status);
        Assert.Equal("Validation error", pd.Title);
        Assert.Contains("validation errors", pd.Detail);

        var categoryName = typeof(ExceptionHandlingMiddleware).FullName;
        Assert.DoesNotContain(_loggerProvider.Entries, e => e.Category == categoryName);
    }
}
