using System.Text.Json;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

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
        await Fixture.CreateFreshWebApplicationAsync();
    }

    public virtual Task DisposeAsync() => Task.CompletedTask;
}
