using System.Net;
using System.Text;
using System.Text.Json;
using VictoryCenter.BLL.DTOs.Admin.FeedbackHistories;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
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
            ImageId = null,
            Status = Status.Draft
        };

        var serializedDto = JsonSerializer.Serialize(createDto, JsonOptions);

        var response = await Fixture.HttpClient.PostAsync($"{BaseUrl}/", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(response.IsSuccessStatusCode);

        var responseString = await response.Content.ReadAsStringAsync();
        var responseContent = JsonSerializer.Deserialize<FeedbackHistoryDto>(responseString, JsonOptions);

        Assert.NotNull(responseContent);
        Assert.Equal(createDto.Title, responseContent.Title);
    }

    [Fact]
    public async Task CreateFeedbackHistory_WithValidImage_ShouldReturnOk()
    {
        var testImage = new Image { BlobName = "test-image.jpg", MimeType = "image/jpeg", Url = "test.com" };
        await Fixture.DbContext.Images.AddAsync(testImage);
        await Fixture.DbContext.SaveChangesAsync();

        var createDto = new CreateFeedbackHistoryDto
        {
            Title = "Valid Story with Image Title",
            Story = "Detailed description of the feedback story with an attached image.",
            ImageId = testImage.Id,
            Status = Status.Draft
        };

        var serializedDto = JsonSerializer.Serialize(createDto, JsonOptions);

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
            ImageId = null,
            Status = Status.Draft
        };

        var serializedDto = JsonSerializer.Serialize(createDto, JsonOptions);

        var response = await Fixture.HttpClient.PostAsync($"{BaseUrl}/", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.False(response.IsSuccessStatusCode);
    }

    [Fact]
    public async Task CreateFeedbackHistory_ShouldFail_WhenTitleTooLong()
    {
        var createDto = new CreateFeedbackHistoryDto
        {
            Title = new string('A', 51),
            Story = "Detailed description of the feedback story.",
            ImageId = null,
            Status = Status.Draft
        };

        var serializedDto = JsonSerializer.Serialize(createDto, JsonOptions);

        var response = await Fixture.HttpClient.PostAsync($"{BaseUrl}/", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.False(response.IsSuccessStatusCode);
    }

    [Fact]
    public async Task CreateFeedbackHistory_ShouldFail_WhenStoryTooShort()
    {
        var createDto = new CreateFeedbackHistoryDto
        {
            Title = "Valid Success Story Title",
            Story = "",
            ImageId = null,
            Status = Status.Draft
        };

        var serializedDto = JsonSerializer.Serialize(createDto, JsonOptions);

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
            ImageId = long.MaxValue,
            Status = Status.Draft
        };

        var serializedDto = JsonSerializer.Serialize(createDto, JsonOptions);

        var response = await Fixture.HttpClient.PostAsync($"{BaseUrl}/", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.False(response.IsSuccessStatusCode);
    }
}
