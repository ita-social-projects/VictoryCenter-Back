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
        Assert.All(page.Items, item => Assert.NotNull(item.Category));
    }

    [Fact]
    public async Task GetPublishedEventNews_ShouldNotReturnUnpublishedItems()
    {
        var unpublishedIds = await Fixture.DbContext.EventNews
            .Where(item => item.Status != Status.Published)
            .Select(item => item.Id)
            .ToListAsync();

        var items = await GetAllItemsAsync();

        Assert.DoesNotContain(items, item => unpublishedIds.Contains(item.Id));
    }

    [Fact]
    public async Task GetPublishedEventNews_WithoutParameters_ShouldReturnAtMostDefaultLimitItems()
    {
        var page = await GetPageAsync();

        Assert.True(page.Items.Length <= EventNewsConstants.DefaultLimit);
    }

    [Fact]
    public async Task GetPublishedEventNews_ShouldReturnCategoryBaseNameAndLocalizations()
    {
        var eventNews = await Fixture.DbContext.EventNews
            .Include(item => item.Category)
            .FirstAsync(item => item.Status == Status.Published);
        var category = eventNews.Category;
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

        var items = await GetAllItemsAsync($"categoryId={category.Id}");

        var returnedEvent = Assert.Single(items, item => item.Id == eventNews.Id);
        var returnedCategory = returnedEvent.Category;
        Assert.Equal(category.Id, returnedCategory.Id);
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

        Assert.True(page.Items.Length <= 4);
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
            .Include(n => n.Category)
            .FirstAsync(n => n.Status == Status.Published);
        var category = publishedWithCategory.Category;
        var expectedCount = await Fixture.DbContext.EventNews
            .CountAsync(n => n.Status == Status.Published && n.CategoryId == category.Id);

        var page = await GetPageAsync($"?categoryId={category.Id}&limit={EventNewsConstants.PublishedTakeMaxValue}");

        Assert.Equal(expectedCount, page.TotalItemsCount);
        Assert.NotEmpty(page.Items);
        Assert.All(page.Items, item => Assert.Equal(category.Id, item.Category.Id));
    }

    [Fact]
    public async Task GetPublishedEventNews_WhenCategoryHasNoNews_ShouldReturnEmptyPage()
    {
        var page = await GetPageAsync("?categoryId=999999");

        Assert.Empty(page.Items);
        Assert.Equal(0, page.TotalItemsCount);
    }

    [Fact]
    public async Task GetPublishedEventNews_ShouldReturnItemsInAdminDefinedOrder()
    {
        var published = await GetPublishedEntitiesAsync(3);
        var desiredOrder = new[] { published[2].Id, published[0].Id, published[1].Id };

        await SetPrioritiesAsync(desiredOrder);

        var items = await GetAllItemsAsync();

        var actual = items
            .Select(item => item.Id)
            .Where(desiredOrder.Contains)
            .ToList();

        Assert.Equal(desiredOrder, actual);
    }

    [Fact]
    public async Task GetPublishedEventNews_WhenAdminChangesOrder_ShouldReflectNewOrder()
    {
        var published = await GetPublishedEntitiesAsync(2);
        var firstId = published[0].Id;
        var secondId = published[1].Id;

        await SetPrioritiesAsync([firstId, secondId]);
        var before = (await GetAllItemsAsync()).Select(item => item.Id).ToList();

        await SetPrioritiesAsync([secondId, firstId]);
        var after = (await GetAllItemsAsync()).Select(item => item.Id).ToList();

        Assert.True(before.IndexOf(firstId) < before.IndexOf(secondId));
        Assert.True(after.IndexOf(secondId) < after.IndexOf(firstId));
    }

    [Fact]
    public async Task GetPublishedEventNews_WhenPrioritiesAreEqual_ShouldOrderById()
    {
        var published = await GetPublishedEntitiesAsync(2);
        var ids = published.Select(item => item.Id).OrderBy(id => id).ToList();

        var tracked = await Fixture.DbContext.EventNews
            .Where(item => ids.Contains(item.Id))
            .ToListAsync();
        var samePriority = tracked.Max(item => item.Priority) + 1;
        foreach (var item in tracked)
        {
            item.Priority = samePriority;
        }

        await Fixture.DbContext.SaveChangesAsync();
        Fixture.DbContext.ChangeTracker.Clear();

        var items = await GetAllItemsAsync();

        var actual = items.Select(item => item.Id).Where(ids.Contains).ToList();
        Assert.Equal(ids, actual);
    }

    [Fact]
    public async Task GetPublishedEventNews_WhenUnpublishedItemHasTopPriority_ShouldNotReturnIt()
    {
        var draft = await Fixture.DbContext.EventNews
            .FirstOrDefaultAsync(item => item.Status != Status.Published);
        if (draft is null)
        {
            return;
        }

        var draftId = draft.Id;
        var minPriority = await Fixture.DbContext.EventNews.MinAsync(item => item.Priority);
        draft.Priority = minPriority - 1;
        await Fixture.DbContext.SaveChangesAsync();
        Fixture.DbContext.ChangeTracker.Clear();

        var page = await GetPageAsync("?offset=0&limit=2");

        Assert.DoesNotContain(page.Items, item => item.Id == draftId);
    }

    [Fact]
    public async Task GetPublishedEventNews_PagesConcatenated_ShouldMatchSortedOrderWithoutDuplicates()
    {
        var total = (await GetPageAsync("?limit=2")).TotalItemsCount;
        Assert.True(total >= 3, "At least 3 published items are required in seed data");

        var expectedIds = await Fixture.DbContext.EventNews
            .AsNoTracking()
            .Where(item => item.Status == Status.Published)
            .OrderBy(item => item.Priority)
            .ThenBy(item => item.Id)
            .Select(item => item.Id)
            .ToListAsync();

        const int pageSize = 2;
        var collected = new List<long>();
        for (var offset = 0; offset < total; offset += pageSize)
        {
            var page = await GetPageAsync($"?offset={offset}&limit={pageSize}");
            collected.AddRange(page.Items.Select(item => item.Id));
        }

        Assert.Equal(expectedIds, collected);
        Assert.Equal(collected.Count, collected.Distinct().Count());
    }

    [Fact]
    public async Task GetPublishedEventNews_WhenFilteredByCategory_ShouldKeepAdminOrderInsideCategory()
    {
        var categoryId = await Fixture.DbContext.EventNews
            .Where(item => item.Status == Status.Published)
            .GroupBy(item => item.CategoryId)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key)
            .FirstOrDefaultAsync();
        Assert.NotEqual(0, categoryId);

        var categoryIds = await Fixture.DbContext.EventNews
            .Where(item => item.Status == Status.Published && item.CategoryId == categoryId)
            .OrderBy(item => item.Id)
            .Select(item => item.Id)
            .Take(2)
            .ToListAsync();
        Assert.Equal(2, categoryIds.Count);

        var desiredOrder = new[] { categoryIds[1], categoryIds[0] };
        await SetPrioritiesAsync(desiredOrder);

        var items = await GetAllItemsAsync($"categoryId={categoryId}");

        Assert.All(items, item => Assert.Equal(categoryId, item.Category.Id));
        var actual = items.Select(item => item.Id).Where(desiredOrder.Contains).ToList();
        Assert.Equal(desiredOrder, actual);
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

    private async Task<List<VictoryCenter.DAL.Entities.EventNews>> GetPublishedEntitiesAsync(int count)
    {
        var entities = await Fixture.DbContext.EventNews
            .AsNoTracking()
            .Where(item => item.Status == Status.Published)
            .OrderBy(item => item.Id)
            .Take(count)
            .ToListAsync();

        Assert.Equal(count, entities.Count);
        return entities;
    }

    private async Task SetPrioritiesAsync(IReadOnlyList<long> idsInDesiredOrder)
    {
        Fixture.DbContext.ChangeTracker.Clear();

        var entities = await Fixture.DbContext.EventNews
            .Where(item => idsInDesiredOrder.Contains(item.Id))
            .ToListAsync();

        var basePriority = await Fixture.DbContext.EventNews.MaxAsync(item => item.Priority) + 1;
        for (var i = 0; i < idsInDesiredOrder.Count; i++)
        {
            entities.Single(item => item.Id == idsInDesiredOrder[i]).Priority = basePriority + i;
        }

        await Fixture.DbContext.SaveChangesAsync();
        Fixture.DbContext.ChangeTracker.Clear();
    }

    private async Task<List<PublishedEventNewsDto>> GetAllItemsAsync(string? extraQuery = null)
    {
        var items = new List<PublishedEventNewsDto>();
        var offset = 0;
        var limit = EventNewsConstants.PublishedTakeMaxValue;

        while (true)
        {
            var query = $"?offset={offset}&limit={limit}";
            if (!string.IsNullOrEmpty(extraQuery))
            {
                query += $"&{extraQuery}";
            }

            var page = await GetPageAsync(query);
            items.AddRange(page.Items);
            offset += limit;

            if (page.Items.Length == 0 || offset >= page.TotalItemsCount)
            {
                break;
            }
        }

        return items;
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
