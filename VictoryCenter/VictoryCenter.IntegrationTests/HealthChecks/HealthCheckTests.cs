using System.Net;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
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
    public async Task Host_WhenStopped_DisposesConfigurationFileWatcher()
    {
        await using var factory = Fixture.Factory.WithWebHostBuilder(_ => { });
        using var client = factory.CreateClient();
        var configuration = factory.Services.GetRequiredService<IConfiguration>();
        var builder = Assert.IsAssignableFrom<IConfigurationBuilder>(configuration);
        var fileProvider = Assert.IsType<PhysicalFileProvider>(builder.GetFileProvider());
        Assert.Equal(
            Path.GetFullPath(Directory.GetCurrentDirectory()).TrimEnd(Path.DirectorySeparatorChar),
            fileProvider.Root.TrimEnd(Path.DirectorySeparatorChar));
        fileProvider.Watch("appsettings.json");

        // PhysicalFileProvider has no public disposal flag; Watch behaves differently in polling mode.
        var disposedField = typeof(PhysicalFileProvider).GetField("_disposed", BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.NotNull(disposedField);
        Assert.Equal(false, disposedField.GetValue(fileProvider));

        await factory.DisposeAsync();

        Assert.Equal(true, disposedField.GetValue(fileProvider));
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
