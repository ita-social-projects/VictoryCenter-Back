using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using VictoryCenter.IntegrationTests.ControllerTests.Base;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.WebAPI.Middleware;

namespace VictoryCenter.IntegrationTests.MiddlewareTests;

[Collection("SharedIntegrationTests")]
public class RequestResponseLoggingMiddlewareTests
{
    private readonly HttpClient _client;
    private readonly InMemoryLoggerProvider _loggerProvider;

    public RequestResponseLoggingMiddlewareTests(IntegrationTestDbFixture fixture)
    {
        var customFactory = fixture.Factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddProvider(new InMemoryLoggerProvider());
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

        _loggerProvider = customFactory.Services
            .GetServices<ILoggerProvider>()
            .OfType<InMemoryLoggerProvider>()
            .Single();
    }

    [Fact]
    public async Task InvokeAsync_Status200_ShouldLogAtInformationLevel()
    {
        var response = await _client.GetAsync("/api/Test");
        Assert.Equal(200, (int)response.StatusCode);

        var categoryName = typeof(RequestResponseLoggingMiddleware).FullName;
        var entry = _loggerProvider.Entries.Last(e => e.Category == categoryName);
        Assert.Equal(LogLevel.Information, entry.LogLevel);
        Assert.Contains("200", entry.Message);
    }

    [Fact]
    public async Task InvokeAsync_Status404_ShouldLogAtWarningLevel()
    {
        var response = await _client.GetAsync("/api/Test/-1");
        Assert.Equal(404, (int)response.StatusCode);

        var categoryName = typeof(RequestResponseLoggingMiddleware).FullName;
        var entry = _loggerProvider.Entries.Last(e => e.Category == categoryName);
        Assert.Equal(LogLevel.Warning, entry.LogLevel);
        Assert.Contains("404", entry.Message);
    }

    [Fact]
    public async Task InvokeAsync_Status500_ShouldLogAtErrorLevel()
    {
        var response = await _client.GetAsync("/api/Test/Get500Response");
        Assert.Equal(500, (int)response.StatusCode);

        var categoryName = typeof(RequestResponseLoggingMiddleware).FullName;
        var entry = _loggerProvider.Entries.Last(e => e.Category == categoryName);
        Assert.Equal(LogLevel.Error, entry.LogLevel);
        Assert.Contains("500", entry.Message);
    }
}
