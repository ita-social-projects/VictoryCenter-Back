using System.Net;
using System.Text;
using Newtonsoft.Json;
using VictoryCenter.BLL.DTOs.ProgramCategories;
using VictoryCenter.IntegrationTests.ControllerTests.Base;
using VictoryCenter.IntegrationTests.Utils.Seeder;

namespace VictoryCenter.IntegrationTests.ControllerTests.ProgramCategories.Update;

[Collection("SharedIntegrationTests")]
public class UpdateProgramCategoryTests : IAsyncLifetime
{
    private readonly HttpClient _httpClient;
    private readonly SeederManager _seederManager;

    public UpdateProgramCategoryTests(IntegrationTestDbFixture fixture)
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
    public async Task UpdateProgramCategory_ShouldUpdateProgramCategory()
    {
        var updateProgramDto = new UpdateProgramCategoryDto
        {
            Id = 1,
            Name = "UpdatedName"
        };
        var serializedDto = JsonConvert.SerializeObject(updateProgramDto);

        var response = await _httpClient.PutAsync("api/ProgramCategory/", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));

        var responseString = await response.Content.ReadAsStringAsync();

        var responseContent = JsonConvert.DeserializeObject<ProgramCategoryDto>(responseString);

        response.EnsureSuccessStatusCode();
        Assert.NotNull(responseContent);
        Assert.Equal(updateProgramDto.Name, responseContent.Name);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task UpdateProgramCategory_ShouldNotUpdateProgramCategory_InvalidName(string? name)
    {
        var updateProgramCategoryDto = new UpdateProgramCategoryDto
        {
            Id = 1,
            Name = name
        };

        var serializedDto = JsonConvert.SerializeObject(updateProgramCategoryDto);

        var response = await _httpClient.PutAsync("/api/ProgramCategory/", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public async Task ProgramCategory_ShouldNotUpdateProgramCategory_NotFound(int id)
    {
        var updateProgramCategoryDto = new UpdateProgramCategoryDto
        {
            Id = id,
            Name = "UpdatedName"
        };
        var serializedDto = JsonConvert.SerializeObject(updateProgramCategoryDto);

        var response = await _httpClient.PutAsync("/api/ProgramCategories/", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
