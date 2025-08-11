using Newtonsoft.Json;
using VictoryCenter.BLL.DTOs.ProgramCategories;
using VictoryCenter.IntegrationTests.ControllerTests.Base;
using VictoryCenter.IntegrationTests.Utils.Seeder;

namespace VictoryCenter.IntegrationTests.ControllerTests.ProgramCategories.GetAll;

[Collection("SharedIntegrationTests")]
public class GetAllProgramCategoriesTests : IAsyncLifetime
{
    private readonly HttpClient _httpClient;
    private readonly SeederManager _seederManager;

    public GetAllProgramCategoriesTests(IntegrationTestDbFixture fixture)
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

    [Fact]
    public async Task ProgramCategory_ShouldReturnAllProgramCategories()
    {
        var response = await _httpClient.GetAsync("/api/ProgramCategory/");
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        var responseContent = JsonConvert.DeserializeObject<IEnumerable<ProgramCategoryDto>>(responseString);
        Assert.NotNull(responseContent);
        Assert.NotEmpty(responseContent);
    }
}
