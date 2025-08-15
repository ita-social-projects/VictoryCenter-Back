using System.Text.Json;
using VictoryCenter.IntegrationTests.ControllerTests.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.Faq.GetAllVisitorPages;

[Collection("SharedIntegrationTests")]
public class GetAllVisitorPagesTest : IAsyncLifetime
{
    private readonly IntegrationTestDbFixture _fixture;
    private readonly JsonSerializerOptions _jsonOptions;

    public GetAllVisitorPagesTest(IntegrationTestDbFixture fixture)
    {
        _fixture = fixture;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task InitializeAsync()
    {
        await _fixture.CreateFreshWebApplication();
    }

    public async Task DisposeAsync() => await _fixture.DisposeAsync();
}
