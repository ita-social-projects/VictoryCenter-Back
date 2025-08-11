using Newtonsoft.Json;
using VictoryCenter.BLL.DTOs.ProgramCategories;
using VictoryCenter.IntegrationTests.ControllerTests.Base;

namespace VictoryCenter.IntegrationTests.ControllerTests.ProgramCategories.GetAll;

[Collection("SharedIntegrationTests")]
public class GetAllProgramCategoriesTests : IAsyncLifetime
{
    private readonly IntegrationTestDbFixture _fixture;
    public GetAllProgramCategoriesTests(IntegrationTestDbFixture fixture)
    {
        _fixture = fixture;
    }

    public async Task InitializeAsync()
    {
        await _fixture.CreateFreshDatabase();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task ProgramCategory_ShouldReturnAllProgramCategories()
    {
        var response = await _fixture.HttpClient.GetAsync("/api/ProgramCategory/");
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        var responseContent = JsonConvert.DeserializeObject<IEnumerable<ProgramCategoryDto>>(responseString);
        Assert.NotNull(responseContent);
        Assert.NotEmpty(responseContent);
    }
}
