using System.Net;
using System.Text;
using Newtonsoft.Json;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyPrograms;
using VictoryCenter.DAL.Enums;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.HippotherapyPrograms.Create;

public class CreateHippotherapyProgramTests : BaseTestClass
{
    public CreateHippotherapyProgramTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task CreatePublishedProgram_ShouldCreateProgram()
    {
        var createProgramDto = new CreateHippotherapyProgramDto
        {
            Name = "TestName",
            Description = "Test description for program",
            Status = Status.Published,
            BackgroundImageId = 1,
            PreviewImageId = 2,
            CategoryIds = [1, 2]
        };

        var serializedDto = JsonConvert.SerializeObject(createProgramDto);

        HttpResponseMessage response = await Fixture.HttpClient.PostAsync(
            "/api/HippotherapyPrograms/",
            new StringContent(serializedDto, Encoding.UTF8, "application/json"));

        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();

        HippotherapyProgramDto? responseContent =
            JsonConvert.DeserializeObject<HippotherapyProgramDto>(responseString);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(responseContent);
        Assert.Equal(createProgramDto.Name, responseContent.Name);
    }

    [Fact]
    public async Task CreateDraftProgram_ShouldCreateProgram()
    {
        var createProgramDto = new CreateHippotherapyProgramDto
        {
            Name = "TestName",
            Status = Status.Draft,
            CategoryIds = [1, 4]
        };
        var serializedDto = JsonConvert.SerializeObject(createProgramDto);

        HttpResponseMessage response = await Fixture.HttpClient.PostAsync(
            "/api/HippotherapyPrograms/",
            new StringContent(serializedDto, Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();

        HippotherapyProgramDto? responseContent = JsonConvert.DeserializeObject<HippotherapyProgramDto>(responseString);

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
        var createProgramDto = new CreateHippotherapyProgramDto
        {
            Name = name!,
            Description = "TestDescription",
            Status = Status.Draft,
            CategoryIds = [1, 3]
        };
        var serializedDto = JsonConvert.SerializeObject(createProgramDto);

        HttpResponseMessage response = await Fixture.HttpClient.PostAsync("/api/HippotherapyPrograms/", new StringContent(
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
        var createProgramDto = new CreateHippotherapyProgramDto
        {
            Name = "TestName",
            Description = description,
            Status = Status.Published,
            BackgroundImageId = 1,
            PreviewImageId = 2,
            CategoryIds = [3, 4]
        };
        var serializedDto = JsonConvert.SerializeObject(createProgramDto);

        HttpResponseMessage response = await Fixture.HttpClient.PostAsync(
            "/api/HippotherapyPrograms/", new StringContent(serializedDto, Encoding.UTF8, "application/json"));

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
