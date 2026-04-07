using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Admin.HistorySection;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.History.Update;

public class UpdateHistorySectionsTests : BaseTestClass
{
    public UpdateHistorySectionsTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task Update_ValidRequest_ShouldCreateSections()
    {
        var payload = new List<UpdateHistorySectionDto>
        {
            CreateTextOnlySection(order: 0, title: "  Main title  ", description: "  Main description text  "),
        };

        var response = await PutRaw(payload);
        var responseString = await response.Content.ReadAsStringAsync();
        var responseContent = JsonSerializer.Deserialize<List<HistorySectionDto>>(responseString, JsonOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(responseContent);
        Assert.Single(responseContent);

        var createdSection = await Fixture.DbContext.Set<HistorySection>()
            .Include(s => s.Contents)
            .SingleAsync();
        var title = createdSection.Contents.Single(c => c.ContentType == ContentType.Title) as DAL.Entities.HistoryContents.TitleHistoryContent;
        var description = createdSection.Contents.Single(c => c.ContentType == ContentType.Description) as DAL.Entities.HistoryContents.DescriptionHistoryContent;

        Assert.NotNull(title);
        Assert.NotNull(description);
        Assert.Equal("Main title", title.Title);
        Assert.Equal("Main description text", description.Description);
    }

    [Fact]
    public async Task Update_InvalidRequest_ShouldReturnBadRequest()
    {
        var payload = new List<UpdateHistorySectionDto>
        {
            new()
            {
                Template = HistorySectionTemplate.TextOnly,
                Order = -1,
                Contents =
                [
                    new CreateHistorySectionContentDto { ContentType = ContentType.Title, Order = 0, Title = "Title" },
                    new CreateHistorySectionContentDto { ContentType = ContentType.Description, Order = 1, Description = "Description text" },
                ]
            },
        };

        var response = await PutRaw(payload);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Update_WithUnknownSectionId_ShouldReturnNotFound()
    {
        var payload = new List<UpdateHistorySectionDto>
        {
            new()
            {
                Id = 999,
                Template = HistorySectionTemplate.TextOnly,
                Order = 0,
                Contents =
                [
                    new CreateHistorySectionContentDto { ContentType = ContentType.Title, Order = 0, Title = "Title" },
                    new CreateHistorySectionContentDto { ContentType = ContentType.Description, Order = 1, Description = "Description text" },
                ]
            },
        };

        var response = await PutRaw(payload);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Update_WithUnknownImageId_ShouldReturnNotFound()
    {
        var payload = new List<UpdateHistorySectionDto>
        {
            CreateSingleImageSection(order: 0, title: "Title", description: "Description text", imageId: 999),
        };

        var response = await PutRaw(payload);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private async Task<HttpResponseMessage> PutRaw(List<UpdateHistorySectionDto> payload)
    {
        var serialized = JsonSerializer.Serialize(payload);
        return await Fixture.HttpClient.PutAsync(
            "/api/History",
            new StringContent(serialized, Encoding.UTF8, "application/json"));
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