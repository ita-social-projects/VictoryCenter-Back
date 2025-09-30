using System.Net;
using System.Text;
using Newtonsoft.Json;
using VictoryCenter.BLL.DTOs.Admin.HypotherapyPrograms;
using VictoryCenter.DAL.Enums;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.HypotherapyPrograms.Create;

public class CreateHypotherapyProgramTests : BaseTestClass
{
    public CreateHypotherapyProgramTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task CreatePublishedProgram_ShouldCreateProgram()
    {
        var createProgramDto = new CreateHypotherapyProgramDto
        {
            Name = "TestName",
            Description = "TestDescription",
            Status = Status.Published,
            ImageId = 1,
            CategoryIds = [1, 2]
        };

        var serializedDto = JsonConvert.SerializeObject(createProgramDto);

        HttpResponseMessage response = await Fixture.HttpClient.PostAsync("/api/HypotherapyPrograms/", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();

        HypotherapyProgramDto? responseContent = JsonConvert.DeserializeObject<HypotherapyProgramDto>(responseString);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(responseContent);
        Assert.Equal(createProgramDto.Name, responseContent.Name);
    }

    [Fact]
    public async Task CreateDraftProgram_ShouldCreateProgram()
    {
        var createProgramDto = new CreateHypotherapyProgramDto
        {
            Name = "TestName",
            Status = Status.Draft,
            CategoryIds = [1, 4]
        };
        var serializedDto = JsonConvert.SerializeObject(createProgramDto);

        HttpResponseMessage response = await Fixture.HttpClient.PostAsync("/api/HypotherapyPrograms/", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();

        HypotherapyProgramDto? responseContent = JsonConvert.DeserializeObject<HypotherapyProgramDto>(responseString);

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
        var createProgramDto = new CreateHypotherapyProgramDto
        {
            Name = name!,
            Description = "TestDescription",
            Status = Status.Draft,
            CategoryIds = [1, 3]
        };
        var serializedDto = JsonConvert.SerializeObject(createProgramDto);

        HttpResponseMessage response = await Fixture.HttpClient.PostAsync("/api/HypotherapyPrograms/", new StringContent(
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
        var createProgramDto = new CreateHypotherapyProgramDto
        {
            Name = "TestName",
            Description = description,
            Status = Status.Published,
            ImageId = 1,
            CategoryIds = [3, 4]
        };
        var serializedDto = JsonConvert.SerializeObject(createProgramDto);

        HttpResponseMessage response = await Fixture.HttpClient.PostAsync("/api/HypotherapyPrograms/", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
