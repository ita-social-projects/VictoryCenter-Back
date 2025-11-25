using System.Net;
using System.Text;
using Newtonsoft.Json;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyPrograms;
using VictoryCenter.DAL.Enums;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.HippotherapyPrograms.Update;

public class UpdateHippotherapyProgramTests : BaseTestClass
{
    public UpdateHippotherapyProgramTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task UpdatePublishedProgram_ShouldUpdateProgram()
    {
        var updateProgramDto = new UpdateHippotherapyProgramDto
        {
            Name = "UpdatedName",
            Description = "Updated description for program",
            Status = Status.Published,
            BackgroundImageId = 1,
            PreviewImageId = 2,
            CategoryIds = [1, 4]
        };

        var serializedDto = JsonConvert.SerializeObject(updateProgramDto);

        HttpResponseMessage response = await Fixture.HttpClient.PutAsync(
            "/api/HippotherapyPrograms/1",
            new StringContent(serializedDto, Encoding.UTF8, "application/json"));

        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();

        HippotherapyProgramDto? responseContent =
            JsonConvert.DeserializeObject<HippotherapyProgramDto>(responseString);

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
        var updateProgramDto = new UpdateHippotherapyProgramDto
        {
            Name = invalidName!,
            Description = "Updated description for program",
            Status = Status.Published,
            BackgroundImageId = 2,
            PreviewImageId = 3,
            CategoryIds = [1, 4]
        };

        var serializedDto = JsonConvert.SerializeObject(updateProgramDto);

        HttpResponseMessage response = await Fixture.HttpClient.PutAsync(
            "/api/HippotherapyPrograms/1",
            new StringContent(serializedDto, Encoding.UTF8, "application/json"));

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task UpdatePublishedProgram_ShouldReturnBadRequest_InvalidDescription(string? invalidDescription)
    {
        var updateProgramDto = new UpdateHippotherapyProgramDto
        {
            Name = "TestName",
            Description = invalidDescription,
            Status = Status.Published,
            BackgroundImageId = 2,
            PreviewImageId = 3,
            CategoryIds = [1, 4]
        };

        var serializedDto = JsonConvert.SerializeObject(updateProgramDto);

        HttpResponseMessage response = await Fixture.HttpClient.PutAsync(
            "/api/HippotherapyPrograms/1",
            new StringContent(serializedDto, Encoding.UTF8, "application/json"));

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task UpdateProgram_ShouldUpdateToDraft_WithOptionalDescription(string? description)
    {
        var updateProgramDto = new UpdateHippotherapyProgramDto
        {
            Name = "TestName",
            Description = description,
            Status = Status.Draft,
            CategoryIds = [1, 4]
        };

        var serializedDto = JsonConvert.SerializeObject(updateProgramDto);

        HttpResponseMessage response = await Fixture.HttpClient.PutAsync(
            "/api/HippotherapyPrograms/1",
            new StringContent(serializedDto, Encoding.UTF8, "application/json"));

        var responseString = await response.Content.ReadAsStringAsync();

        HippotherapyProgramDto? responseContent =
            JsonConvert.DeserializeObject<HippotherapyProgramDto>(responseString);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(responseContent);
        Assert.Equal(updateProgramDto.Description, responseContent.Description);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public async Task UpdateProgram_ShouldNotUpdateProgram_NotFound(int id)
    {
        var updateProgramDto = new UpdateHippotherapyProgramDto
        {
            Name = "TestName",
            Description = "TestDescription",
            Status = Status.Draft,
            CategoryIds = [1, 4]
        };

        var serializedDto = JsonConvert.SerializeObject(updateProgramDto);

        HttpResponseMessage response = await Fixture.HttpClient.PutAsync(
            $"/api/HippotherapyPrograms/{id}",
            new StringContent(serializedDto, Encoding.UTF8, "application/json"));

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
