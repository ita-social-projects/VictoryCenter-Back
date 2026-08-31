using System.Net;
using System.Text.Json;
using VictoryCenter.BLL.DTOs.Admin.FeedbackHistories;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.FeedbackHistories.GetAll;

public class GetAllFeedbackHistoryTests : BaseTestClass
{
    private const string BaseUrl = "/api/FeedbackHistories";

    public GetAllFeedbackHistoryTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task GetAllFeedbackHistories_ShouldReturnOkAndListOfEntities()
    {
        await CreateTestFeedbackHistoryAsync();

        var response = await Fixture.HttpClient.GetAsync($"{BaseUrl}/");

        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();
        var responseContent = JsonSerializer.Deserialize<IEnumerable<FeedbackHistoryDto>>(responseString, JsonOptions);

        Assert.NotNull(responseContent);
        Assert.NotEmpty(responseContent);
        Assert.Contains(responseContent, item => item.Title == "Title For GetAll Test");
    }

    private async Task<FeedbackHistory> CreateTestFeedbackHistoryAsync()
    {
        var entity = new FeedbackHistory
        {
            Title = "Title For GetAll Test",
            Story = "Story content for get all test that meets length requirements.",
            ImageId = null,
            CreatedAt = DateTimeOffset.UtcNow,
            Priority = 1,
            Status = Status.Draft
        };

        await Fixture.DbContext.FeedbackHistories.AddAsync(entity);
        await Fixture.DbContext.SaveChangesAsync();
        return entity;
    }
}