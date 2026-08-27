using System.Net;
using System.Text;
using System.Text.Json;
using VictoryCenter.BLL.DTOs.Admin.FeedbackHistories;
using VictoryCenter.DAL.Entities;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.FeedbackHistories.Update;

public class UpdateFeedbackHistoryTests : BaseTestClass
{
    private const string BaseUrl = "/api/FeedbackHistories";

    public UpdateFeedbackHistoryTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task UpdateFeedbackHistory_ValidRequest_ShouldUpdateEntity()
    {
        var existingEntity = await CreateTestFeedbackHistoryAsync();

        var updateDto = new UpdateFeedbackHistoryDto
        {
            Title = "Updated Valid Title Here",
            Story = "Updated story content that satisfies validation.",
            ImageId = null
        };

        var serializedDto = JsonSerializer.Serialize(updateDto);

        HttpResponseMessage response = await Fixture.HttpClient.PutAsync(
            $"{BaseUrl}/{existingEntity.Id}",
            new StringContent(serializedDto, Encoding.UTF8, "application/json"));

        var responseString = await response.Content.ReadAsStringAsync();
        FeedbackHistoryDto? responseContent = JsonSerializer.Deserialize<FeedbackHistoryDto>(responseString, JsonOptions);

        response.EnsureSuccessStatusCode();
        Assert.NotNull(responseContent);
        Assert.Equal(existingEntity.Id, responseContent.Id);
        Assert.Equal(updateDto.Title, responseContent.Title);
        Assert.Equal(updateDto.Story, responseContent.Story);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("TooShort")]
    public async Task UpdateFeedbackHistory_InvalidTitle_ShouldReturnBadRequest(string? invalidTitle)
    {
        var existingEntity = await CreateTestFeedbackHistoryAsync();

        var updateDto = new UpdateFeedbackHistoryDto
        {
            Title = invalidTitle!,
            Story = "Valid story content for testing validation.",
            ImageId = null
        };

        var serializedDto = JsonSerializer.Serialize(updateDto);

        HttpResponseMessage response = await Fixture.HttpClient.PutAsync(
            $"{BaseUrl}/{existingEntity.Id}",
            new StringContent(serializedDto, Encoding.UTF8, "application/json"));

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public async Task UpdateFeedbackHistory_NotFound_ShouldReturnNotFound(long testId)
    {
        var updateDto = new UpdateFeedbackHistoryDto
        {
            Title = "Non Existing Entity Title",
            Story = "Story content for not found scenario.",
            ImageId = null
        };

        var serializedDto = JsonSerializer.Serialize(updateDto);

        HttpResponseMessage response = await Fixture.HttpClient.PutAsync(
            $"{BaseUrl}/{testId}",
            new StringContent(serializedDto, Encoding.UTF8, "application/json"));

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateFeedbackHistory_NonExistentImageId_ShouldReturnNotFound()
    {
        var existingEntity = await CreateTestFeedbackHistoryAsync();

        var updateDto = new UpdateFeedbackHistoryDto
        {
            Title = "Updated Valid Title Here",
            Story = "Updated story content that satisfies validation.",
            ImageId = long.MaxValue
        };

        var serializedDto = JsonSerializer.Serialize(updateDto);

        HttpResponseMessage response = await Fixture.HttpClient.PutAsync(
            $"{BaseUrl}/{existingEntity.Id}",
            new StringContent(serializedDto, Encoding.UTF8, "application/json"));

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private async Task<FeedbackHistory> CreateTestFeedbackHistoryAsync()
    {
        var entity = new FeedbackHistory
        {
            Title = "Original Valid Title",
            Story = "Original story text that meets the length requirements.",
            ImageId = null,
            CreatedAt = DateTimeOffset.UtcNow
        };

        await Fixture.DbContext.FeedbackHistories.AddAsync(entity);
        await Fixture.DbContext.SaveChangesAsync();
        return entity;
    }
}