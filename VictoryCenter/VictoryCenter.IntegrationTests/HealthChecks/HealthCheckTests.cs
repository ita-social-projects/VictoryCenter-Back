using System.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VictoryCenter.DAL.Data;
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
        var databaseUnavailable = false;
        var unavailableOptions = new DbContextOptionsBuilder<VictoryCenterDbContext>()
            .UseSqlServer("Server=127.0.0.1,1;Database=UnavailableHealthCheck;User Id=test;Password=test;Connect Timeout=1;ConnectRetryCount=0;Encrypt=False")
            .Options;
        using var factory = Fixture.Factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddScoped(provider => new VictoryCenterDbContext(
                    databaseUnavailable
                        ? unavailableOptions
                        : provider.GetRequiredService<DbContextOptions<VictoryCenterDbContext>>()));
            });
        });
        using var client = factory.CreateClient();
        databaseUnavailable = true;
        var response = await client.GetAsync("/health/ready");

        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
        Assert.Equal("Unhealthy", await response.Content.ReadAsStringAsync());

        var liveResponse = await client.GetAsync("/health/live");
        Assert.Equal(HttpStatusCode.OK, liveResponse.StatusCode);
        Assert.Equal("Healthy", await liveResponse.Content.ReadAsStringAsync());
    }
}
