using System.Net;
using System.Text;
using System.Text.Json;
using VictoryCenter.BLL.DTOs.Admin.HistorySection;
using VictoryCenter.DAL.Enums;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.History.GetAll;

public class GetAllHistorySectionsTests : BaseTestClass
{
    public GetAllHistorySectionsTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task GetAll_AfterValidUpdate_ShouldReturnCreatedSections()
    {
        var updateSections = new List<UpdateHistorySectionDto>
        {
            CreateTextOnlySection(order: 0, title: "History title", description: "History description text"),
            CreateSingleImageSection(order: 1, title: "Image title", description: "Image description text", imageId: 1),
        };

        await PutSections(updateSections);

        var response = await Fixture.HttpClient.GetAsync("/api/History");
        var responseString = await response.Content.ReadAsStringAsync();
        var responseContent = JsonSerializer.Deserialize<List<HistorySectionDto>>(responseString, JsonOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(responseContent);
        Assert.Equal(2, responseContent.Count);
        Assert.Contains(responseContent, s => s.Template == HistorySectionTemplate.TextOnly && s.Order == 0);
        Assert.Contains(responseContent, s => s.Template == HistorySectionTemplate.SingleImageBottom && s.Order == 1);
    }

    [Fact]
    public async Task GetAll_WithImageContent_ShouldIncludeImageInResponse()
    {
        var updateSections = new List<UpdateHistorySectionDto>
        {
            CreateSingleImageSection(order: 0, title: "Image title", description: "Image description text", imageId: 1),
        };

        await PutSections(updateSections);

        var response = await Fixture.HttpClient.GetAsync("/api/History");
        var responseString = await response.Content.ReadAsStringAsync();
        var responseContent = JsonSerializer.Deserialize<List<HistorySectionDto>>(responseString, JsonOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(responseContent);
        var imageContent = responseContent
            .Single()
            .Contents
            .Single(c => c.ContentType == ContentType.Image);
        Assert.NotNull(imageContent.Image);
    }

    private async Task PutSections(List<UpdateHistorySectionDto> sections)
    {
        var serialized = JsonSerializer.Serialize(sections);

        var response = await Fixture.HttpClient.PutAsync(
            "/api/History",
            new StringContent(serialized, Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    private static UpdateHistorySectionDto CreateTextOnlySection(int order, string title, string description)
    {
        return new UpdateHistorySectionDto
        {
            Template = HistorySectionTemplate.TextOnly,
            Order = order,
            Contents =
            [
                new CreateHistorySectionContentDto { ContentType = ContentType.Title, Order = 0, Title = title },
                new CreateHistorySectionContentDto { ContentType = ContentType.Description, Order = 1, Description = description },
            ]
        };
    }

    private static UpdateHistorySectionDto CreateSingleImageSection(int order, string title, string description, long imageId)
    {
        return new UpdateHistorySectionDto
        {
            Template = HistorySectionTemplate.SingleImageBottom,
            Order = order,
            Contents =
            [
                new CreateHistorySectionContentDto { ContentType = ContentType.Title, Order = 0, Title = title },
                new CreateHistorySectionContentDto { ContentType = ContentType.Description, Order = 1, Description = description },
                new CreateHistorySectionContentDto { ContentType = ContentType.Image, Order = 2, ImageId = imageId },
            ]
        };
    }
}