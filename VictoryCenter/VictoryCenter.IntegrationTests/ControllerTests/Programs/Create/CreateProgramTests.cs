using System.Net;
using System.Text;
using Newtonsoft.Json;
using VictoryCenter.BLL.DTOs.Programs;
using VictoryCenter.DAL.Enums;
using VictoryCenter.IntegrationTests.ControllerTests.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.Programs.Create;

[Collection("SharedIntegrationTests")]
public class CreateProgramTests : IAsyncLifetime
{
    private readonly IntegrationTestDbFixture _fixture;

    public CreateProgramTests(IntegrationTestDbFixture dbFixture)
    {
        _fixture = dbFixture;
    }

    public async Task InitializeAsync()
    {
        await _fixture.CreateFreshWebApplication();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task CreatePublishedProgram_ShouldCreateProgram()
    {
        var createProgramDto = new CreateProgramDto
        {
            Name = "TestName",
            Description = "TestDescription",
            Status = Status.Published,
            ImageId = 1,
            CategoriesId = [1, 2]
        };

        var serializedDto = JsonConvert.SerializeObject(createProgramDto);

        HttpResponseMessage response = await _fixture.HttpClient.PostAsync("/api/Program/", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();

        ProgramDto? responseContent = JsonConvert.DeserializeObject<ProgramDto>(responseString);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(responseContent);
        Assert.Equal(createProgramDto.Name, responseContent.Name);
    }

    [Fact]
    public async Task CreateDraftProgram_ShouldCreateProgram()
    {
        var createProgramDto = new CreateProgramDto
        {
            Name = "TestName",
            Status = Status.Draft,
            CategoriesId = [1, 4]
        };
        var serializedDto = JsonConvert.SerializeObject(createProgramDto);

        HttpResponseMessage response = await _fixture.HttpClient.PostAsync("/api/Program/", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();

        ProgramDto? responseContent = JsonConvert.DeserializeObject<ProgramDto>(responseString);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(responseContent);
        Assert.Equal(createProgramDto.Name, responseContent.Name);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task CreateProgram_ShouldNotCreateProgram_InvalidName(string? name)
    {
        var createProgramDto = new CreateProgramDto
        {
            Name = name!,
            Description = "TestDescription",
            Status = Status.Draft,
            CategoriesId = [1, 3]
        };
        var serializedDto = JsonConvert.SerializeObject(createProgramDto);

        HttpResponseMessage response = await _fixture.HttpClient.PostAsync("/api/Program/", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task CreatePublishedProgram_ShouldNotCreateProgram_InvalidDescription(string? description)
    {
        var createProgramDto = new CreateProgramDto
        {
            Name = "TestName",
            Description = description,
            Status = Status.Published,
            ImageId = 1,
            CategoriesId = [3, 4]
        };
        var serializedDto = JsonConvert.SerializeObject(createProgramDto);

        HttpResponseMessage response = await _fixture.HttpClient.PostAsync("/api/Program/", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
