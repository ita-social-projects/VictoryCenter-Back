using System.Net;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.DTOs.Public.EventNews;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.EventNews.GetPublished;

public class GetPublishedEventNewsTests : BaseTestClass
{
    private const string BaseUrl = "/api/EventNews/published";

    public GetPublishedEventNewsTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task GetPublishedEventNews_ShouldReturnOkWithPaginatedPublishedItems()
    {
        var publishedCount = await Fixture.DbContext.EventNews
            .CountAsync(item => item.Status == Status.Published);

        var page = await GetPageAsync($"?limit={EventNewsConstants.PublishedTakeMaxValue}");

        Assert.NotEmpty(page.Items);
        Assert.Equal(publishedCount, page.TotalItemsCount);
        Assert.All(page.Items, item => Assert.NotNull(item.Localizations));
        Assert.All(page.Items, item => Assert.NotNull(item.Categories));
    }

    [Fact]
    public async Task GetPublishedEventNews_ShouldNotReturnUnpublishedItems()
    {
        var unpublishedIds = await Fixture.DbContext.EventNews
            .Where(item => item.Status != Status.Published)
            .Select(item => item.Id)
            .ToListAsync();

        var page = await GetPageAsync($"?limit={EventNewsConstants.PublishedTakeMaxValue}");

        Assert.DoesNotContain(page.Items, item => unpublishedIds.Contains(item.Id));
    }

    [Fact]
    public async Task GetPublishedEventNews_WithoutParameters_ShouldReturnAtMostDefaultLimitItems()
    {
        var page = await GetPageAsync();

        Assert.True(page.Items.Count() <= EventNewsConstants.DefaultLimit);
    }

    [Fact]
    public async Task GetPublishedEventNews_ShouldReturnCategoryBaseNameAndLocalizations()
    {
        var eventNews = await Fixture.DbContext.EventNews
            .Include(item => item.Categories)
            .FirstAsync(item => item.Status == Status.Published && item.Categories.Any());
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

        var page = await GetPageAsync($"?categoryId={category.Id}&limit={EventNewsConstants.PublishedTakeMaxValue}");

        var returnedEvent = Assert.Single(page.Items, item => item.Id == eventNews.Id);
        var returnedCategory = Assert.Single(returnedEvent.Categories, item => item.Id == category.Id);
        Assert.Equal(category.Name, returnedCategory.Name);
        var localization = Assert.Single(
            returnedCategory.Localizations,
            item => item.Language.Code == language.Code);
        Assert.Equal(localizedName, localization.Name);
    }

    [Fact]
    public async Task GetPublishedEventNews_WhenLimit4_ShouldReturnAtMost4Items()
    {
        var page = await GetPageAsync("?limit=4");

        Assert.True(page.Items.Count() <= 4);
    }

    [Fact]
    public async Task GetPublishedEventNews_ConsecutivePages_ShouldNotOverlap()
    {
        var firstPage = await GetPageAsync("?offset=0&limit=2");
        var secondPage = await GetPageAsync("?offset=2&limit=2");

        Assert.Equal(firstPage.TotalItemsCount, secondPage.TotalItemsCount);
        var firstIds = firstPage.Items.Select(item => item.Id).ToHashSet();
        Assert.DoesNotContain(secondPage.Items, item => firstIds.Contains(item.Id));
    }

    [Fact]
    public async Task GetPublishedEventNews_WhenOffsetIsBeyondTotal_ShouldReturnEmptyItemsWithTotalCount()
    {
        var total = (await GetPageAsync()).TotalItemsCount;

        var page = await GetPageAsync($"?offset={total + 100}&limit=2");

        Assert.Empty(page.Items);
        Assert.Equal(total, page.TotalItemsCount);
    }

    [Fact]
    public async Task GetPublishedEventNews_WhenFilteredByCategory_ShouldReturnOnlyItemsOfThatCategory()
    {
        var publishedWithCategory = await Fixture.DbContext.EventNews
            .Include(n => n.Categories)
            .FirstAsync(n => n.Status == Status.Published && n.Categories.Any());
        var category = publishedWithCategory.Categories.First();
        var expectedCount = await Fixture.DbContext.EventNews
            .CountAsync(n => n.Status == Status.Published && n.Categories.Any(c => c.Id == category.Id));

        var page = await GetPageAsync($"?categoryId={category.Id}&limit={EventNewsConstants.PublishedTakeMaxValue}");

        Assert.Equal(expectedCount, page.TotalItemsCount);
        Assert.NotEmpty(page.Items);
        Assert.All(page.Items, item => Assert.Contains(item.Categories, c => c.Id == category.Id));
    }

    [Fact]
    public async Task GetPublishedEventNews_WhenCategoryHasNoNews_ShouldReturnEmptyPage()
    {
        var page = await GetPageAsync("?categoryId=999999");

        Assert.Empty(page.Items);
        Assert.Equal(0, page.TotalItemsCount);
    }

    [Fact]
    public async Task GetPublishedEventNews_ShouldOrderItemsByPublishedAtDescending()
    {
        // Arrange
        var publishedEvents = Fixture.DbContext.EventNews
            .Where(eventNews => eventNews.Status == Status.Published)
            .Take(3)
            .ToList();

        Assert.Equal(3, publishedEvents.Count);

        publishedEvents[0].PublishedAt = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        publishedEvents[1].PublishedAt = new DateTimeOffset(2026, 3, 1, 12, 0, 0, TimeSpan.Zero);
        publishedEvents[2].PublishedAt = new DateTimeOffset(2026, 2, 1, 12, 0, 0, TimeSpan.Zero);

        await Fixture.DbContext.SaveChangesAsync();

        // Act
        var page = await GetPageAsync($"?limit={EventNewsConstants.PublishedTakeMaxValue}");

        // Assert
        var testedIds = publishedEvents.Select(eventNews => eventNews.Id).ToHashSet();
        var testedItems = page.Items
            .Where(item => testedIds.Contains(item.Id))
            .Select(item => item.Id)
            .ToList();

        Assert.Equal(
            [
                publishedEvents[1].Id,
                publishedEvents[2].Id,
                publishedEvents[0].Id
            ],
            testedItems);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(EventNewsConstants.PublishedTakeMaxValue + 1)]
    public async Task GetPublishedEventNews_WhenLimitIsOutOfRange_ShouldReturnBadRequest(int limit)
    {
        HttpResponseMessage response = await Fixture.HttpClient.GetAsync($"{BaseUrl}?limit={limit}");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetPublishedEventNews_WhenOffsetIsNegative_ShouldReturnBadRequest()
    {
        HttpResponseMessage response = await Fixture.HttpClient.GetAsync($"{BaseUrl}?offset=-1");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    private async Task<PaginationResult<PublishedEventNewsDto>> GetPageAsync(string query = "")
    {
        HttpResponseMessage response = await Fixture.HttpClient.GetAsync($"{BaseUrl}{query}");
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        var page = JsonConvert.DeserializeObject<PaginationResult<PublishedEventNewsDto>>(responseString);
        Assert.NotNull(page);
        return page;
    }
}
