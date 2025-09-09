using Newtonsoft.Json;
using VictoryCenter.BLL.DTOs.Admin.Programs;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.DAL.Enums;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.Programs.GetFiltered;

public class GetProgramsByFilters : BaseTestClass
{
    public GetProgramsByFilters(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

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

        HttpResponseMessage response = await Fixture.HttpClient.GetAsync($"/api/Programs?{queryString}");

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        PaginationResult<ProgramDto>? result = JsonConvert.DeserializeObject<PaginationResult<ProgramDto>>(content);

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

        HttpResponseMessage response = await Fixture.HttpClient.GetAsync($"/api/Programs?{queryString}");

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        PaginationResult<ProgramDto>? result = JsonConvert.DeserializeObject<PaginationResult<ProgramDto>>(content);

        Assert.NotNull(result);
        Assert.True(response.IsSuccessStatusCode);
    }
}
