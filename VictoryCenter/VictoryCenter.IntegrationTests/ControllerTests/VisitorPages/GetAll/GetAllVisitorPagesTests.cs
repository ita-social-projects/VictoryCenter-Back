using System.Text.Json;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.VisitorPages;
using VictoryCenter.IntegrationTests.ControllerTests.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.VisitorPages.GetAll;

[Collection("SharedIntegrationTests")]
public class GetAllVisitorPagesTests : IAsyncLifetime
{
    private readonly IntegrationTestDbFixture _fixture;
    private readonly JsonSerializerOptions _jsonOptions;

    public GetAllVisitorPagesTests(IntegrationTestDbFixture fixture)
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

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task GetAllVisitorPages_ShouldReturnAllVisitorPages()
    {
        var response = await _fixture.HttpClient.GetAsync(new Uri("/api/faq/pages", UriKind.Relative));
        var responseString = await response.Content.ReadAsStringAsync();
        var responseContent = JsonSerializer.Deserialize<List<VisitorPageDto>>(
            responseString,
            _jsonOptions);

        response.EnsureSuccessStatusCode();
        Assert.NotNull(responseContent);
        Assert.NotEmpty(responseContent);
        foreach (var dto in responseContent)
        {
            Assert.Contains(PageConstants.VisitorPages, x => x.Slug == dto.Slug && x.Title == dto.Title);
        }
    }
}
