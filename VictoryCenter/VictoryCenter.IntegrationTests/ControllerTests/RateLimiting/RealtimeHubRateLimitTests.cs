using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.RateLimiting;

[Collection("SharedIntegrationTests")]
public class RealtimeHubRateLimitTests(IntegrationTestDbFixture fixture)
{
    private const int RealtimeConnectPermitLimit = 2;
    private const string NegotiatePath = "/hubs/reports/negotiate?negotiateVersion=1";

    [Fact]
    public async Task Negotiate_RequestLimitExceeded_ShouldReturnTooManyRequests()
    {
        await using var factory = CreateFactory(RealtimeConnectPermitLimit);
        using var client = CreateClient(factory);

        for (var requestNumber = 0; requestNumber < RealtimeConnectPermitLimit; requestNumber++)
        {
            using var permittedResponse = await client.PostAsync(NegotiatePath, content: null);
            Assert.Equal(HttpStatusCode.OK, permittedResponse.StatusCode);
        }

        using var rejectedResponse = await client.PostAsync(NegotiatePath, content: null);

        Assert.Equal(HttpStatusCode.TooManyRequests, rejectedResponse.StatusCode);
        Assert.NotNull(rejectedResponse.Headers.RetryAfter);
    }

    private WebApplicationFactory<Program> CreateFactory(int permitLimit)
    {
        return fixture.Factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((_, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["RateLimiting:RealtimeConnect:PermitLimit"] = permitLimit.ToString(),
                });
            });
        });
    }

    private static HttpClient CreateClient(WebApplicationFactory<Program> factory)
    {
        var client = factory.CreateClient();
        client.BaseAddress = new Uri("https://localhost");
        return client;
    }
}
