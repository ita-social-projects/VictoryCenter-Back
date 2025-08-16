using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using VictoryCenter.IntegrationTests.ControllerTests.DbFixture;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.WebAPI.Middleware;

namespace VictoryCenter.IntegrationTests.MiddlewareTests;

[Collection("SharedIntegrationTests")]
public class ExceptionHandlingMiddlewareTests
{
    private readonly HttpClient _client;
    private readonly InMemoryLoggerProvider _loggerProvider;

    public ExceptionHandlingMiddlewareTests(IntegrationTestDbFixture fixture)
    {
        var customFactory = fixture._factory.WithWebHostBuilder(builder =>
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
    public async Task InvokeAsync_UnhandledException_ShouldWriteProblemDetailsAndLogAtCriticalLevel()
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

        Assert.Contains("Unhandled exception occured while processing request", log.Message);
    }
}
