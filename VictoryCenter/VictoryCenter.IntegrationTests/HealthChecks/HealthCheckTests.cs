using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.HealthChecks;

public class HealthCheckTests : BaseTestClass
{
    public HealthCheckTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task Live_ReturnsHealthyWithoutAuthentication()
    {
        using var client = Fixture.Factory.CreateClient();
        var response = await client.GetAsync("/health/live");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Healthy", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task Ready_WhenDatabaseIsAvailable_ReturnsHealthyWithoutAuthentication()
    {
        using var client = Fixture.Factory.CreateClient();
        var response = await client.GetAsync("/health/ready");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Healthy", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task Ready_WhenDatabaseIsUnavailable_ReturnsServiceUnavailableWithoutAuthentication()
    {
        using var factory = Fixture.Factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddHealthChecks()
                    .AddCheck(
                        "unavailable-sql",
                        () => HealthCheckResult.Unhealthy(),
                        tags: ["ready"]);
            });
        });
        using var client = factory.CreateClient();
        var response = await client.GetAsync("/health/ready");

        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
        Assert.Equal("Unhealthy", await response.Content.ReadAsStringAsync());
    }
}
