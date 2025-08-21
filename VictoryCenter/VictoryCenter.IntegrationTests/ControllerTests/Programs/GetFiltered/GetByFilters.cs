using Newtonsoft.Json;
using VictoryCenter.BLL.DTOs.Programs;
using VictoryCenter.IntegrationTests.ControllerTests.DbFixture;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.IntegrationTests.ControllerTests.Programs.GetFiltered;

[Collection("SharedIntegrationTests")]
public class GetByFilters : IAsyncLifetime
{
    private readonly IntegrationTestDbFixture _fixture;

    public GetByFilters(IntegrationTestDbFixture fixture)
    {
        _fixture = fixture;
    }

    public async Task InitializeAsync()
    {
        await _fixture.CreateFreshWebApplication();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Theory]
    [InlineData(0, 3)]
    [InlineData(1, 1)]
    [InlineData(2, 2)]
    public async Task GetPrograms_ShouldReturnPrograms_NoFilters(int offset, int limit)
    {
        var query = new Dictionary<string, string?>
        {
            ["offset"] = offset.ToString(),
            ["limit"] = limit.ToString(),
            ["status"] = null,
            ["categoryId"] = null
        };

        var queryString = string.Join("&", query
            .Where(kv => kv.Value is not null)
            .Select(kv => $"{kv.Key}={Uri.EscapeDataString(kv.Value!)}"));

        HttpResponseMessage response = await _fixture.HttpClient.GetAsync($"/api/Program?{queryString}");

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        ProgramsFilterResponseDto? result = JsonConvert.DeserializeObject<ProgramsFilterResponseDto>(content);

        Assert.NotNull(result);
        Assert.True(response.IsSuccessStatusCode);
    }

    [Theory]
    [InlineData(Status.Draft)]
    [InlineData(Status.Published)]
    public async Task GetPrograms_ShouldReturnPrograms_FilteredByStatus(Status status)
    {
        var query = new Dictionary<string, string?>
        {
            ["offset"] = "0",
            ["limit"] = "10",
            ["status"] = status.ToString(),
            ["categoryId"] = null
        };

        var queryString = string.Join("&", query
            .Where(kv => kv.Value is not null)
            .Select(kv => $"{kv.Key}={Uri.EscapeDataString(kv.Value!)}"));

        HttpResponseMessage response = await _fixture.HttpClient.GetAsync($"/api/Program?{queryString}");

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        ProgramsFilterResponseDto? result = JsonConvert.DeserializeObject<ProgramsFilterResponseDto>(content);

        Assert.NotNull(result);
        Assert.True(response.IsSuccessStatusCode);
    }
}
