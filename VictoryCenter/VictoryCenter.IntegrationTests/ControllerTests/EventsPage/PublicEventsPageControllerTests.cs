using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Public.EventNews;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.EventsPage;

public class PublicEventsPageControllerTests : BaseTestClass
{
    private const string Endpoint = "/api/EventsPage/public";

    public PublicEventsPageControllerTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task Get_ReturnsMappedPublicDto()
    {
        // Act
        var response = await Fixture.HttpClient.GetAsync(Endpoint);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var dto = await response.Content.ReadFromJsonAsync<EventsIntroSectionPublicDto>(JsonOptions);
        Assert.NotNull(dto);
        Assert.Equal("<p>Що відбувалось</p>", dto.EventsBlockTitle);
        Assert.Equal(
            "<p>Цей розділ — про ті моменти, коли те, у що ми віримо, стає реальністю. Тут ти знайдеш все: від маленьких зустрічей до великих відкриттів, від перших кроків дітей у стайні до глибоких переживань ветеранів у сідлі.</p>",
            dto.PageDescription);
    }

    [Fact]
    public async Task Get_WhenHidden_ReturnsEmptyStringsForHiddenFields()
    {
        // Arrange
        var section = await Fixture.DbContext.EventsIntroSections.SingleAsync();
        section.IsEventsBlockTitleHidden = true;
        section.IsPageDescriptionHidden = true;
        await Fixture.DbContext.SaveChangesAsync();
        Fixture.DbContext.ChangeTracker.Clear();

        // Act
        var response = await Fixture.HttpClient.GetAsync(Endpoint);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var dto = await response.Content.ReadFromJsonAsync<EventsIntroSectionPublicDto>(JsonOptions);
        Assert.NotNull(dto);
        Assert.Equal(string.Empty, dto.EventsBlockTitle);
        Assert.Equal(string.Empty, dto.PageDescription);
    }

    [Fact]
    public async Task Get_WhenSectionDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        Fixture.DbContext.EventsIntroSections.RemoveRange(Fixture.DbContext.EventsIntroSections);
        await Fixture.DbContext.SaveChangesAsync();
        Fixture.DbContext.ChangeTracker.Clear();

        // Act
        var response = await Fixture.HttpClient.GetAsync(Endpoint);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
