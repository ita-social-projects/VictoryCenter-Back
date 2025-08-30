using System.Net;
using System.Text;
using Newtonsoft.Json;
using VictoryCenter.BLL.DTOs.ProgramCategories;
using VictoryCenter.IntegrationTests.ControllerTests.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.ProgramCategories.Update;

[Collection("SharedIntegrationTests")]
public class UpdateProgramCategoryTests : IAsyncLifetime
{
    private readonly IntegrationTestDbFixture _fixture;

    public UpdateProgramCategoryTests(IntegrationTestDbFixture fixture)
    {
        _fixture = fixture;
    }

    public async Task InitializeAsync()
    {
        await _fixture.CreateFreshWebApplication();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task UpdateProgramCategory_ShouldUpdateProgramCategory()
    {
        var updateProgramDto = new UpdateProgramCategoryDto
        {
            Name = "UpdatedName"
        };
        var serializedDto = JsonConvert.SerializeObject(updateProgramDto);

        HttpResponseMessage response = await _fixture.HttpClient.PutAsync("/api/ProgramCategory/1", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();

        ProgramCategoryDto? responseContent = JsonConvert.DeserializeObject<ProgramCategoryDto>(responseString);

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
            Name = name!
        };

        var serializedDto = JsonConvert.SerializeObject(updateProgramCategoryDto);

        HttpResponseMessage response = await _fixture.HttpClient.PutAsync("/api/ProgramCategory/1", new StringContent(
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
            Name = "UpdatedName"
        };
        var serializedDto = JsonConvert.SerializeObject(updateProgramCategoryDto);

        HttpResponseMessage response = await _fixture.HttpClient.PutAsync($"/api/ProgramCategories/{id}", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
