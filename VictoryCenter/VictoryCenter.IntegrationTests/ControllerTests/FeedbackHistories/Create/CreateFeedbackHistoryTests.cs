using System.Net;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using VictoryCenter.BLL.DTOs.Admin.FeedbackHistories;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.FeedbackHistories.Create;

public class CreateFeedbackHistoryTests : BaseTestClass
{
    private const string BaseUrl = "/api/FeedbackHistories";

    public CreateFeedbackHistoryTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task CreateFeedbackHistory_ValidRequest_ShouldReturnOk()
    {
        var createDto = new CreateFeedbackHistoryDto
        {
            Title = "Valid Success Story Title",
            Story = "Detailed description of the feedback story that satisfies validator lengths.",
            ImageId = null
        };

        var serializedDto = JsonConvert.SerializeObject(createDto);

        var response = await Fixture.HttpClient.PostAsync($"{BaseUrl}/", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(response.IsSuccessStatusCode);
    }

    [Fact]
    public async Task CreateFeedbackHistory_WithValidImage_ShouldReturnOk()
    {
        var existingImage = await Fixture.DbContext.Images.FirstOrDefaultAsync()
                            ?? throw new InvalidOperationException("Couldn't setup existing image entity.");

        var createDto = new CreateFeedbackHistoryDto
        {
            Title = "Valid Story with Image Title",
            Story = "Detailed description of the feedback story with an attached image.",
            ImageId = existingImage.Id
        };

        var serializedDto = JsonConvert.SerializeObject(createDto);

        var response = await Fixture.HttpClient.PostAsync($"{BaseUrl}/", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(response.IsSuccessStatusCode);
    }

    [Fact]
    public async Task CreateFeedbackHistory_ShouldFail_WhenTitleTooShort()
    {
        var createDto = new CreateFeedbackHistoryDto
        {
            Title = "Short",
            Story = "Detailed description of the feedback story.",
            ImageId = null
        };

        var serializedDto = JsonConvert.SerializeObject(createDto);

        var response = await Fixture.HttpClient.PostAsync($"{BaseUrl}/", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.False(response.IsSuccessStatusCode);
    }

    [Fact]
    public async Task CreateFeedbackHistory_ShouldFail_WhenImageNotFound()
    {
        var createDto = new CreateFeedbackHistoryDto
        {
            Title = "Valid Success Story Title",
            Story = "Detailed description of the feedback story.",
            ImageId = long.MaxValue
        };

        var serializedDto = JsonConvert.SerializeObject(createDto);

        var response = await Fixture.HttpClient.PostAsync($"{BaseUrl}/", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.False(response.IsSuccessStatusCode);
    }
}