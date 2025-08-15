using System.Text.Json;
using VictoryCenter.IntegrationTests.ControllerTests.DbFixture;

namespace VictoryCenter.IntegrationTests.Utils;

[Collection("SharedIntegrationTests")]
public abstract class BaseTestClass : IAsyncLifetime
{
    protected BaseTestClass(IntegrationTestDbFixture fixture)
    {
        Fixture = fixture;
        JsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    protected IntegrationTestDbFixture Fixture { get; init; }
    protected JsonSerializerOptions JsonOptions { get; init; }

    public virtual async Task InitializeAsync()
    {
        await Fixture.CreateFreshWebApplication();
    }

    public virtual Task DisposeAsync() => Task.CompletedTask;
}
