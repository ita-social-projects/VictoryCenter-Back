using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Public.EventNews;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.EventNews.GetPublished;

public class GetPublishedEventNewsTests : BaseTestClass
{
    public GetPublishedEventNewsTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task GetPublishedEventNews_ShouldReturnOnlyPublishedItems()
    {
        HttpResponseMessage response = await Fixture.HttpClient.GetAsync("/api/EventNews/published/");
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        IEnumerable<PublishedEventNewsDto>? responseContent =
            JsonConvert.DeserializeObject<IEnumerable<PublishedEventNewsDto>>(responseString);
        Assert.NotNull(responseContent);
        Assert.NotEmpty(responseContent);
        Assert.All(responseContent, item => Assert.NotNull(item.Localizations));
        Assert.All(responseContent, item => Assert.NotNull(item.Categories));
    }

    [Fact]
    public async Task GetPublishedEventNews_ShouldReturnCategoryBaseNameAndLocalizations()
    {
        var eventNews = await Fixture.DbContext.EventNews
            .Include(item => item.Categories)
            .FirstAsync(item => item.Status == VictoryCenter.DAL.Enums.Status.Published);
        var category = eventNews.Categories.First();
        var language = await Fixture.DbContext.LocalizationLanguages
            .FirstAsync(item => item.Code == "en");
        const string localizedName = "Localized category name";

        Fixture.DbContext.EventNewsCategoryLocalizations.Add(new EventNewsCategoryLocalization
        {
            EntityId = category.Id,
            LanguageId = language.Id,
            Name = localizedName,
            CreatedAt = DateTimeOffset.UtcNow
        });
        await Fixture.DbContext.SaveChangesAsync();
        Fixture.DbContext.ChangeTracker.Clear();

        var response = await Fixture.HttpClient.GetAsync("/api/EventNews/published/");

        response.EnsureSuccessStatusCode();
        var items = await response.Content.ReadFromJsonAsync<List<PublishedEventNewsDto>>();
        var returnedEvent = Assert.Single(items!, item => item.Id == eventNews.Id);
        var returnedCategory = Assert.Single(returnedEvent.Categories, item => item.Id == category.Id);
        Assert.Equal(category.Name, returnedCategory.Name);
        var localization = Assert.Single(
            returnedCategory.Localizations,
            item => item.Language.Code == language.Code);
        Assert.Equal(language.Code, localization.Language.Code);
        Assert.Equal(localizedName, localization.Name);
    }

    [Fact]
    public async Task GetPublishedEventNews_WhenTake4_ShouldReturnAtMost4Items()
    {
        HttpResponseMessage response = await Fixture.HttpClient.GetAsync("/api/EventNews/published?take=4");
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        IEnumerable<PublishedEventNewsDto>? responseContent =
            JsonConvert.DeserializeObject<IEnumerable<PublishedEventNewsDto>>(responseString);
        Assert.NotNull(responseContent);
        Assert.True(responseContent.Count() <= 4);
    }

    [Fact]
    public async Task GetPublishedEventNews_ShouldReturnItemsSortedByPublishedAtDescending()
    {
        HttpResponseMessage response = await Fixture.HttpClient.GetAsync("/api/EventNews/published/");
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        List<PublishedEventNewsDto>? responseContent =
            JsonConvert.DeserializeObject<List<PublishedEventNewsDto>>(responseString);
        Assert.NotNull(responseContent);
        Assert.NotEmpty(responseContent);

        var publishedDates = responseContent
            .Select(e => e.PublishedAt ?? DateTimeOffset.MinValue)
            .ToList();
        Assert.Equal(publishedDates.OrderByDescending(d => d).ToList(), publishedDates);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(EventNewsConstants.PublishedTakeMaxValue + 1)]
    public async Task GetPublishedEventNews_WhenTakeIsOutOfRange_ShouldReturnBadRequest(int take)
    {
        HttpResponseMessage response = await Fixture.HttpClient.GetAsync($"/api/EventNews/published?take={take}");

        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }
}
