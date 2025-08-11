using System.Net;
using System.Text;
using Newtonsoft.Json;
using VictoryCenter.DAL.Enums;
using VictoryCenter.BLL.DTOs.Programs;
using VictoryCenter.IntegrationTests.ControllerTests.Base;
using VictoryCenter.IntegrationTests.Utils.Seeder;

namespace VictoryCenter.IntegrationTests.ControllerTests.Programs.Update;

[Collection("SharedIntegrationTests")]
public class UpdateProgramTests : IAsyncLifetime
{
    private readonly HttpClient _httpClient;
    private readonly SeederManager _seederManager;

    public UpdateProgramTests(IntegrationTestDbFixture fixture)
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
    public async Task UpdatePublishedProgram_ShouldUpdateProgram()
    {
        var updateProgramDto = new UpdateProgramDto
        {
            Id = 1,
            Name = "UpdatedName",
            Description = "UpdatedDescription",
            ImageId = 1,
            CategoriesId = [1, 4]
        };

        var serializedDto = JsonConvert.SerializeObject(updateProgramDto);

        var response = await _httpClient.PutAsync("/api/Program/", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));

        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();

        var responseContent = JsonConvert.DeserializeObject<ProgramDto>(responseString);

        Assert.NotNull(responseContent);
        Assert.Equal(updateProgramDto.Name, responseContent.Name);
        Assert.Equal(updateProgramDto.Description, responseContent.Description);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task UpdateProgramWithInvalidName_ShouldReturnBadRequest_InvalidName(string? invalidName)
    {
        var updateProgramDto = new UpdateProgramDto
        {
            Id = 1,
            Name = invalidName,
            Description = "UpdatedDescription",
            Status = Status.Published,
            ImageId = 2,
            CategoriesId = [1, 4]
        };
        var serializedDto = JsonConvert.SerializeObject(updateProgramDto);
        var response = await _httpClient.PutAsync("/api/Program/", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));
        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task UpdateProgram_ShouldReturnBadRequest_InvalidDescription(
        string? invalidDescription)
    {
        var updateProgramDto = new UpdateProgramDto
        {
            Id = 1,
            Name = "TestName",
            Description = invalidDescription,
            Status = Status.Published,
            ImageId = 2,
            CategoriesId = [1, 4]
        };
        var serializedDto = JsonConvert.SerializeObject(updateProgramDto);
        var response = await _httpClient.PutAsync("/api/Program/", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));
        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData(null)]
    public async Task UpdateProgram_ShouldUpdateToDraft(string? description)
    {
        var updateProgramDto = new UpdateProgramDto
        {
            Id = 1,
            Name = "TestName",
            Description = description,
            Status = Status.Draft,
            ImageId = 2,
            CategoriesId = [1, 4]
        };
        var serializedDto = JsonConvert.SerializeObject(updateProgramDto);
        var response = await _httpClient.PutAsync("/api/Program/", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));
        var responseString = await response.Content.ReadAsStringAsync();
        var responseContent = JsonConvert.DeserializeObject<ProgramDto>(responseString);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(responseContent);
        Assert.Equal(updateProgramDto.Description, responseContent.Description);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public async Task UpdateProgram_ShouldNotUpdateProgram_NotFound(int id)
    {
        var updateProgramDto = new UpdateProgramDto
        {
            Id = id,
            Name = "TestName",
            Description = "TestDescription",
            Status = Status.Draft,
            ImageId = 2,
            CategoriesId = [1, 4]
        };
        var serializedDto = JsonConvert.SerializeObject(updateProgramDto);
        var response = await _httpClient.PutAsync("/api/Program/", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));
        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
