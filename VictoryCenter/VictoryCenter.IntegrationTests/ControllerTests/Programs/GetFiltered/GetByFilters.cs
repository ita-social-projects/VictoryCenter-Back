using Newtonsoft.Json;
using VictoryCenter.BLL.DTOs.Programs;
using VictoryCenter.IntegrationTests.ControllerTests.Base;
using VictoryCenter.IntegrationTests.Utils.Seeder;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.IntegrationTests.ControllerTests.Programs.GetFiltered;

[Collection("SharedIntegrationTests")]
public class GetByFilters : IAsyncLifetime
{
    private readonly HttpClient _httpClient;
    private readonly SeederManager _seederManager;

    public GetByFilters(IntegrationTestDbFixture fixture)
    {
        _httpClient = fixture.HttpClient;
        _seederManager = fixture.SeederManager ?? throw new InvalidOperationException(
            "SeederManager is not registered in the service collection.");
    }

    public async Task InitializeAsync()
    {
        await _seederManager.DisposeAllAsync();
        await _seederManager.SeedAllAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Theory]
    [InlineData(0, 3)]
    [InlineData(1, 1)]
    [InlineData(2, 2)]
    public async Task GetPrograms_ShouldReturnPrograms_NoFilters(int offset, int limit)
    {
        ProgramFilterRequestDto requestDto = new()
        {
            Offset = offset,
            Limit = limit,
            Status = null,
            CategoryId = null
        };

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

        var response = await _httpClient.GetAsync($"/api/Program?{queryString}");

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<ProgramsFilterResponseDto>(content);

        Assert.NotNull(result);
        Assert.Equal(result.Programs.Count, limit);
    }

    [Theory]
    [InlineData(Status.Draft)]
    [InlineData(Status.Published)]
    public async Task GetPrograms_ShouldReturnPrograms_FilteredByStatus(Status status)
    {
        ProgramFilterRequestDto requestDto = new()
        {
            Offset = 0,
            Limit = 0,
            Status = status,
            CategoryId = null
        };

        var query = new Dictionary<string, string?>
        {
            ["offset"] = "0",
            ["limit"] = "0",
            ["status"] = status.ToString(),
            ["categoryId"] = null
        };

        var queryString = string.Join("&", query
            .Where(kv => kv.Value is not null)
            .Select(kv => $"{kv.Key}={Uri.EscapeDataString(kv.Value!)}"));

        var response = await _httpClient.GetAsync($"/api/Program?{queryString}");

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<ProgramsFilterResponseDto>(content);

        Assert.NotNull(result);
        Assert.True(response.IsSuccessStatusCode);
    }
}
