using System.Net;
using System.Net.Http.Json;
using VictoryCenter.BLL.DTOs.Admin.EventNews;
using VictoryCenter.BLL.DTOs.Admin.Localization.EventNewsCategories;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.DAL.Enums;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.EventNews.GetAdmin;

public class GetAdminEventNewsTests : BaseTestClass
{
    private const string EndpointUri = "/api/EventNews";

    public GetAdminEventNewsTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task GetByFilters_ShouldReturnItemsAndTotalCountInDeterministicOrder()
    {
        var response = await Fixture.HttpClient.GetAsync(EndpointUri);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var page = await response.Content.ReadFromJsonAsync<PaginationResult<EventNewsDto>>();
        Assert.NotNull(page);
        Assert.NotEmpty(page.Items);
        Assert.True(page.TotalItemsCount >= page.Items.Length);
        Assert.Equal(
            page.Items.Select(item => item.Id).OrderByDescending(id => id),
            page.Items.Select(item => item.Id));
    }

    [Fact]
    public async Task GetByFilters_ShouldApplyOffsetAndLimit()
    {
        var allItems = await Fixture.HttpClient.GetFromJsonAsync<PaginationResult<EventNewsDto>>(EndpointUri);

        var response = await Fixture.HttpClient.GetAsync($"{EndpointUri}?offset=1&limit=2");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var page = await response.Content.ReadFromJsonAsync<PaginationResult<EventNewsDto>>();
        Assert.NotNull(allItems);
        Assert.NotNull(page);
        Assert.Equal(2, page.Items.Length);
        Assert.Equal(allItems.TotalItemsCount, page.TotalItemsCount);
        Assert.Equal(allItems.Items.Skip(1).Take(2).Select(item => item.Id), page.Items.Select(item => item.Id));
    }

    [Fact]
    public async Task GetByFilters_ShouldFilterByAssignedCategory()
    {
        var allItems = await Fixture.HttpClient.GetFromJsonAsync<PaginationResult<EventNewsDto>>(EndpointUri);
        var categoryId = allItems!.Items.SelectMany(item => item.Categories).First().Id;

        var response = await Fixture.HttpClient.GetAsync($"{EndpointUri}?categoryId={categoryId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var page = await response.Content.ReadFromJsonAsync<PaginationResult<EventNewsDto>>();
        Assert.NotNull(page);
        Assert.NotEmpty(page.Items);
        Assert.Equal(page.Items.Length, page.TotalItemsCount);
        Assert.All(page.Items, item => Assert.Contains(item.Categories, category => category.Id == categoryId));
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
    public async Task GetByFilters_WhenFilterIsInvalid_ShouldReturnBadRequest(string query)
    {
        var response = await Fixture.HttpClient.GetAsync($"{EndpointUri}?{query}");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
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
                Resource = "https://example.com/admin-event",
                PublishedAt = DateTimeOffset.UtcNow,
                Status = Status.Published,
                PreviewImageId = 1,
                BackgroundImageId = 2,
                CategoryIds = [categoryId],
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

        var category = Assert.Single(eventNews.Categories);
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
}
