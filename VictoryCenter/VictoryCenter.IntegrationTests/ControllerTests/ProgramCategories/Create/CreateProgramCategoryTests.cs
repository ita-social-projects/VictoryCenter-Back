using System.Net;
using System.Text;
using Newtonsoft.Json;
using VictoryCenter.BLL.DTOs.ProgramCategories;
using VictoryCenter.IntegrationTests.ControllerTests.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.ProgramCategories.Create;

[Collection("SharedIntegrationTests")]
public class CreateProgramCategoryTests : IAsyncLifetime
{
    private IntegrationTestDbFixture _fixture;

    public CreateProgramCategoryTests(IntegrationTestDbFixture fixture)
    {
        _fixture = fixture;
    }

    public async Task InitializeAsync()
    {
        await _fixture.CreateFreshWebApplication();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task ProgramCategory_ShouldCreateProgramCategory()
    {
        var createProgramCategoryDto = new CreateProgramCategoryDto
        {
            Name = "NewName1"
        };
        var serializedDto = JsonConvert.SerializeObject(createProgramCategoryDto);

        var response = await _fixture.HttpClient.PostAsync("/api/ProgramCategory/", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();

        var responseContent = JsonConvert.DeserializeObject<ProgramCategoryDto>(responseString);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(responseContent);
        Assert.Equal(createProgramCategoryDto.Name, responseContent.Name);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task ProgramCategory_ShouldNotCreateProgramCategory_InvalidName(string? name)
    {
        var createProgramCategoryDto = new CreateProgramCategoryDto
        {
            Name = name
        };
        var serializedDto = JsonConvert.SerializeObject(createProgramCategoryDto);

        var response = await _fixture.HttpClient.PostAsync("/api/ProgramCategory/", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
