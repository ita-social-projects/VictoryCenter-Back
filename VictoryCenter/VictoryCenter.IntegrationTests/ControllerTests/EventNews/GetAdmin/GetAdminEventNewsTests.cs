using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants.Localization;
using VictoryCenter.BLL.DTOs.Admin.EventNews;
using VictoryCenter.BLL.DTOs.Admin.Localization.EventNewsCategories;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Enums;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.EventNews.GetAdmin;

using EventNewsEntity = VictoryCenter.DAL.Entities.EventNews;

public class GetAdminEventNewsTests : BaseTestClass
{
    private const string EndpointUri = "/api/EventNews";

    public GetAdminEventNewsTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task GetByFilters_ShouldReturnItemsAndTotalCount()
    {
        var response = await Fixture.HttpClient.GetAsync(EndpointUri);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var page = await response.Content.ReadFromJsonAsync<PaginationResult<EventNewsDto>>();

        Assert.NotNull(page);
        Assert.NotEmpty(page.Items);
        Assert.True(page.TotalItemsCount >= page.Items.Length);
    }

    [Fact]
    public async Task GetByFilters_ShouldApplyOffsetAndLimit()
    {
        var allItems = await Fixture.HttpClient
            .GetFromJsonAsync<PaginationResult<EventNewsDto>>(
                $"{EndpointUri}?offset=0&limit=100");

        Assert.NotNull(allItems);

        var response = await Fixture.HttpClient.GetAsync(
            $"{EndpointUri}?offset=1&limit=2");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var page = await response.Content
            .ReadFromJsonAsync<PaginationResult<EventNewsDto>>();

        Assert.NotNull(page);
        Assert.Equal(2, page.Items.Length);
        Assert.Equal(allItems.TotalItemsCount, page.TotalItemsCount);

        Assert.Equal(
            allItems.Items.Skip(1).Take(2).Select(item => item.Id),
            page.Items.Select(item => item.Id));
    }

    [Fact]
    public async Task GetByFilters_WhenOffsetExceedsTotalCount_ShouldReturnEmptyPage()
    {
        var allItems = await Fixture.HttpClient.GetFromJsonAsync<PaginationResult<EventNewsDto>>(EndpointUri);
        Assert.NotNull(allItems);
        var offsetBeyondLastItem = allItems.TotalItemsCount + 1;

        var response = await Fixture.HttpClient.GetAsync(
            $"{EndpointUri}?offset={offsetBeyondLastItem}&limit=1");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var page = await response.Content.ReadFromJsonAsync<PaginationResult<EventNewsDto>>();
        Assert.NotNull(page);
        Assert.Empty(page.Items);
        Assert.Equal(allItems.TotalItemsCount, page.TotalItemsCount);
    }

    [Fact]
    public async Task GetByFilters_ShouldFilterByAssignedCategory()
    {
        var allItems = await Fixture.HttpClient.GetFromJsonAsync<PaginationResult<EventNewsDto>>(EndpointUri);
        var categoryId = allItems!.Items.First().Category.Id;

        var response = await Fixture.HttpClient.GetAsync($"{EndpointUri}?categoryId={categoryId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var page = await response.Content.ReadFromJsonAsync<PaginationResult<EventNewsDto>>();
        Assert.NotNull(page);
        Assert.NotEmpty(page.Items);
        Assert.True(page.TotalItemsCount >= page.Items.Length);
        Assert.All(page.Items, item => Assert.Equal(categoryId, item.Category.Id));
    }

    [Fact]
    public async Task GetByFilters_WhenNoItemsMatch_ShouldReturnEmptyPage()
    {
        var response = await Fixture.HttpClient.GetAsync($"{EndpointUri}?categoryId={long.MaxValue}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var page = await response.Content.ReadFromJsonAsync<PaginationResult<EventNewsDto>>();
        Assert.NotNull(page);
        Assert.Empty(page.Items);
        Assert.Equal(0, page.TotalItemsCount);
    }

    [Theory]
    [InlineData("offset=-1")]
    [InlineData("limit=0")]
    [InlineData("categoryId=0")]
    [InlineData("translationStatusFilter=999")]
    public async Task GetByFilters_WhenFilterIsInvalid_ShouldReturnBadRequest(string query)
    {
        var response = await Fixture.HttpClient.GetAsync($"{EndpointUri}?{query}");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetByFilters_WhenTranslationIsMissing_ShouldFilterBeforePaginationAndCount()
    {
        var testData = await CreateTranslationFilterTestDataAsync();

        var response = await Fixture.HttpClient.GetAsync(
            $"{EndpointUri}?categoryId={testData.CategoryId}" +
            $"&status={Status.Draft}&translationStatusFilter={TranslationStatusFilter.Missing}" +
            "&offset=0&limit=1");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var page = await response.Content.ReadFromJsonAsync<PaginationResult<EventNewsDto>>();
        Assert.NotNull(page);
        Assert.Equal(1, page.TotalItemsCount);
        Assert.Equal(testData.MissingEventId, Assert.Single(page.Items).Id);
    }

    [Fact]
    public async Task GetByFilters_WhenTranslationIsOutdated_ShouldReturnMatchingLocalizationStatus()
    {
        var testData = await CreateTranslationFilterTestDataAsync();

        var response = await Fixture.HttpClient.GetAsync(
            $"{EndpointUri}?categoryId={testData.CategoryId}" +
            $"&translationStatusFilter={TranslationStatusFilter.Outdated}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var page = await response.Content.ReadFromJsonAsync<PaginationResult<EventNewsDto>>();
        Assert.NotNull(page);
        Assert.Equal(1, page.TotalItemsCount);
        var item = Assert.Single(page.Items);
        Assert.Equal(testData.OutdatedEventId, item.Id);
        Assert.Contains(
            item.Localizations,
            localization => localization.TranslationStatus == TranslationStatus.Outdated);
    }

    [Fact]
    public async Task GetById_ShouldReturnCompleteEventNewsData()
    {
        const long categoryId = 1;
        const long languageId = 1;
        var categoryLocalizationResponse = await Fixture.HttpClient.PostAsJsonAsync(
            "/api/EventNewsCategoryLocalizations",
            new CreateEventNewsCategoryLocalizationDto
            {
                EntityId = categoryId,
                LanguageId = languageId,
                Name = "Localized category"
            });
        categoryLocalizationResponse.EnsureSuccessStatusCode();

        var createResponse = await Fixture.HttpClient.PostAsJsonAsync(
            EndpointUri,
            new CreateEventNewsDto
            {
                Title = "Admin event title",
                Description = "Admin event description",
                Resource = "https://example.com/admin-event",
                PublishedAt = DateTimeOffset.UtcNow,
                Status = Status.Published,
                PreviewImageId = 1,
                BackgroundImageId = 2,
                CategoryId = categoryId,
                Localizations =
                [
                    new CreateEventNewsLocalizationDto
                    {
                        LanguageId = languageId,
                        Title = "Admin event title",
                        Description = "Admin event description"
                    },
                ]
            });
        createResponse.EnsureSuccessStatusCode();
        var created = await createResponse.Content.ReadFromJsonAsync<EventNewsDto>();

        var response = await Fixture.HttpClient.GetAsync($"{EndpointUri}/{created!.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var eventNews = await response.Content.ReadFromJsonAsync<EventNewsDto>();
        Assert.NotNull(eventNews);
        Assert.Equal(created.Id, eventNews.Id);
        Assert.NotNull(eventNews.Slug);
        Assert.Equal("https://example.com/admin-event", eventNews.Resource);
        Assert.Equal(Status.Published, eventNews.Status);
        Assert.NotNull(eventNews.PreviewImage);
        Assert.Equal(1, eventNews.PreviewImage.Id);
        Assert.NotNull(eventNews.BackgroundImage);
        Assert.Equal(2, eventNews.BackgroundImage.Id);

        var category = eventNews.Category;
        Assert.Equal(categoryId, category.Id);
        var categoryLocalization = Assert.Single(category.Localizations);
        Assert.Equal(languageId, categoryLocalization.Language.Id);
        Assert.Equal("Localized category", categoryLocalization.Name);

        var localization = Assert.Single(eventNews.Localizations);
        Assert.Equal(languageId, localization.Language.Id);
        Assert.Equal("Admin event title", localization.Title);
        Assert.Equal("Admin event description", localization.Description);
        Assert.Equal(TranslationStatus.Relevant, localization.TranslationStatus);
    }

    [Fact]
    public async Task GetById_WhenItemDoesNotExist_ShouldReturnNotFound()
    {
        var response = await Fixture.HttpClient.GetAsync($"{EndpointUri}/{long.MaxValue}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Theory]
    [InlineData("/api/EventNews")]
    [InlineData("/api/EventNews/1")]
    public async Task GetEndpoints_ShouldRequireAuthorization(string endpoint)
    {
        using var anonymousClient = Fixture.Factory.CreateClient();

        var response = await anonymousClient.GetAsync(endpoint);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetByFilters_WhenCategorySpecified_ShouldOrderItemsByPriority()
    {
        // Arrange
        var categoryId = await Fixture.DbContext.EventNews
            .GroupBy(eventNews => eventNews.CategoryId)
            .Where(group => group.Count() >= 3)
            .Select(group => group.Key)
            .FirstAsync();

        var items = await Fixture.DbContext.EventNews
            .Where(eventNews => eventNews.CategoryId == categoryId)
            .Take(3)
            .ToArrayAsync();

        items[0].Priority = -1;
        items[1].Priority = -2;
        items[2].Priority = -3;
        await Fixture.DbContext.SaveChangesAsync();

        items[0].Priority = 3;
        items[1].Priority = 1;
        items[2].Priority = 2;

        await Fixture.DbContext.SaveChangesAsync();

        // Act
        var response = await Fixture.HttpClient.GetAsync(
            $"{EndpointUri}?categoryId={categoryId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var page = await response.Content
            .ReadFromJsonAsync<PaginationResult<EventNewsDto>>();

        Assert.NotNull(page);

        var testedIds = items
            .Select(item => item.Id)
            .ToHashSet();

        var testedItems = page.Items
            .Where(item => testedIds.Contains(item.Id))
            .ToArray();

        Assert.Equal(3, testedItems.Length);

        Assert.Equal(
            [items[1].Id, items[2].Id, items[0].Id],
            testedItems.Select(item => item.Id));
    }

    private async Task<TranslationFilterTestData> CreateTranslationFilterTestDataAsync()
    {
        var languageIds = await Fixture.DbContext.LocalizationLanguages
            .Where(language => language.Id != LocalizationLanguageConstants.PrimaryLanguageId)
            .Select(language => language.Id)
            .ToArrayAsync();
        Assert.NotEmpty(languageIds);

        var category = new EventNewsCategory
        {
            Name = $"Filter-{Guid.NewGuid():N}"[..20],
            CreatedAt = DateTimeOffset.UtcNow
        };
        var completeEvent = EventNews("Complete translation", Status.Published, category, 1);
        var missingEvent = EventNews("Missing translation", Status.Draft, category, 2);
        var outdatedEvent = EventNews("Outdated translation", Status.Published, category, 3);

        Fixture.DbContext.EventNewsCategories.Add(category);
        Fixture.DbContext.EventNews.AddRange(completeEvent, missingEvent, outdatedEvent);
        foreach (var languageId in languageIds)
        {
            completeEvent.Localizations.Add(Localization(languageId, TranslationStatus.Relevant));
            outdatedEvent.Localizations.Add(Localization(
                languageId,
                languageId == languageIds[0]
                    ? TranslationStatus.Outdated
                    : TranslationStatus.Relevant));
        }

        await Fixture.DbContext.SaveChangesAsync();

        return new TranslationFilterTestData(
            category.Id,
            missingEvent.Id,
            outdatedEvent.Id);
    }

    private static EventNewsEntity EventNews(
        string title,
        Status status,
        EventNewsCategory category,
        long priority)
    {
        return new EventNewsEntity
        {
            Title = title,
            Slug = $"{title.Replace(' ', '-').ToLowerInvariant()}-{Guid.NewGuid():N}",
            Status = status,
            Category = category,
            Priority = priority,
            CreatedAt = DateTimeOffset.UtcNow
        };
    }

    private static EventNewsLocalization Localization(
        long languageId,
        TranslationStatus translationStatus)
    {
        return new EventNewsLocalization
        {
            LanguageId = languageId,
            Title = "Localized event title",
            TranslationStatus = translationStatus,
            CreatedAt = DateTimeOffset.UtcNow
        };
    }

    private sealed record TranslationFilterTestData(
        long CategoryId,
        long MissingEventId,
        long OutdatedEventId);
}
