using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Admin.EventsPage;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.EventsPage;

public class EventsPageControllerTests : BaseTestClass
{
    private const string Endpoint = "/api/EventsPage";

    public EventsPageControllerTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task Get_AuthorizedRequest_ReturnsSeededIntroSection()
    {
        // Act
        var response = await Fixture.HttpClient.GetAsync(Endpoint);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var dto = await response.Content.ReadFromJsonAsync<EventsIntroSectionDto>(JsonOptions);
        Assert.NotNull(dto);
        Assert.Equal("<p>Що відбувалось</p>", dto.EventsBlockTitle);
        Assert.Equal(
            "<p>Цей розділ — про ті моменти, коли те, у що ми віримо, стає реальністю. Тут ти знайдеш все: від маленьких зустрічей до великих відкриттів, від перших кроків дітей у стайні до глибоких переживань ветеранів у сідлі.</p>",
            dto.PageDescription);
        Assert.Single(await Fixture.DbContext.EventsIntroSections.AsNoTracking().ToListAsync());
    }

    [Fact]
    public async Task Put_AuthorizedRequests_UpdateOnlyTheirOwnField()
    {
        // Arrange
        var original = await Fixture.DbContext.EventsIntroSections.SingleAsync();
        const string newDescription = "<p>Updated description</p>";
        const string newTitle = "<p>Updated title</p>";

        // Act
        var descriptionResponse = await Fixture.HttpClient.PutAsJsonAsync(
            $"{Endpoint}/description",
            new UpdateEventsPageDescriptionDto { PageDescription = newDescription });

        // Assert
        Assert.Equal(HttpStatusCode.OK, descriptionResponse.StatusCode);
        Fixture.DbContext.ChangeTracker.Clear();
        var afterDescriptionUpdate = await Fixture.DbContext.EventsIntroSections.SingleAsync();
        Assert.Equal(newDescription, afterDescriptionUpdate.PageDescription);
        Assert.Equal(original.EventsBlockTitle, afterDescriptionUpdate.EventsBlockTitle);

        // Act
        var titleResponse = await Fixture.HttpClient.PutAsJsonAsync(
            $"{Endpoint}/events-block-title",
            new UpdateEventsBlockTitleDto { EventsBlockTitle = newTitle });

        // Assert
        Assert.Equal(HttpStatusCode.OK, titleResponse.StatusCode);
        Fixture.DbContext.ChangeTracker.Clear();
        var afterTitleUpdate = await Fixture.DbContext.EventsIntroSections.SingleAsync();
        Assert.Equal(newTitle, afterTitleUpdate.EventsBlockTitle);
        Assert.Equal(newDescription, afterTitleUpdate.PageDescription);
    }

    [Fact]
    public async Task Requests_WithoutAuthorization_ReturnUnauthorized()
    {
        // Arrange
        using var anonymousClient = Fixture.Factory.CreateClient();

        // Act
        var getResponse = await anonymousClient.GetAsync(Endpoint);
        var descriptionResponse = await anonymousClient.PutAsJsonAsync(
            $"{Endpoint}/description",
            new UpdateEventsPageDescriptionDto { PageDescription = "<p>Unauthorized</p>" });
        var titleResponse = await anonymousClient.PutAsJsonAsync(
            $"{Endpoint}/events-block-title",
            new UpdateEventsBlockTitleDto { EventsBlockTitle = "<p>Unauthorized</p>" });

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, getResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, descriptionResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, titleResponse.StatusCode);
    }

    [Fact]
    public async Task Put_WithInvalidContent_ReturnsBadRequest()
    {
        // Act
        var blankTitleResponse = await Fixture.HttpClient.PutAsJsonAsync(
            $"{Endpoint}/events-block-title",
            new UpdateEventsBlockTitleDto { EventsBlockTitle = "<p> </p>" });
        var longTitleResponse = await Fixture.HttpClient.PutAsJsonAsync(
            $"{Endpoint}/events-block-title",
            new UpdateEventsBlockTitleDto { EventsBlockTitle = $"<p>{new string('a', 101)}</p>" });
        var blankDescriptionResponse = await Fixture.HttpClient.PutAsJsonAsync(
            $"{Endpoint}/description",
            new UpdateEventsPageDescriptionDto { PageDescription = "<p> </p>" });
        var longDescriptionResponse = await Fixture.HttpClient.PutAsJsonAsync(
            $"{Endpoint}/description",
            new UpdateEventsPageDescriptionDto { PageDescription = $"<p>{new string('a', 1001)}</p>" });

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, blankTitleResponse.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, longTitleResponse.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, blankDescriptionResponse.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, longDescriptionResponse.StatusCode);
    }

    [Fact]
    public async Task Requests_WhenSingletonIsMissing_ReturnNotFound()
    {
        // Arrange
        Fixture.DbContext.EventsIntroSections.RemoveRange(Fixture.DbContext.EventsIntroSections);
        await Fixture.DbContext.SaveChangesAsync();
        Fixture.DbContext.ChangeTracker.Clear();

        // Act
        var getResponse = await Fixture.HttpClient.GetAsync(Endpoint);
        var descriptionResponse = await Fixture.HttpClient.PutAsJsonAsync(
            $"{Endpoint}/description",
            new UpdateEventsPageDescriptionDto { PageDescription = "<p>Missing</p>" });
        var titleResponse = await Fixture.HttpClient.PutAsJsonAsync(
            $"{Endpoint}/events-block-title",
            new UpdateEventsBlockTitleDto { EventsBlockTitle = "<p>Missing</p>" });

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, descriptionResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, titleResponse.StatusCode);
    }
}
