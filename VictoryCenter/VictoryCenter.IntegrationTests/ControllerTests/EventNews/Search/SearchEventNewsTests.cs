using System.Net;
using System.Net.Http.Json;
using System.Text;
using Newtonsoft.Json;
using VictoryCenter.BLL.DTOs.Admin.EventNews;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.DAL.Enums;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.EventNews.Search;

public class SearchEventNewsTests : BaseTestClass
{
    private const string EndpointUri = "/api/EventNews";

    public SearchEventNewsTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task Search_ValidRequest_ShouldReturnOk()
    {
        var response = await Fixture.HttpClient.GetAsync($"{EndpointUri}/search?searchQuery=Title");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Search_SearchQueryTooShort_ShouldReturnBadRequest()
    {
        var response = await Fixture.HttpClient.GetAsync($"{EndpointUri}/search?searchQuery=ab");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Search_ShouldMatchSubstringCaseInsensitivelyAndOrderByPublishedAtDescending()
    {
        var uniqueMarker = $"SrchMrk{Guid.NewGuid():N}";
        var olderEventId = await CreateEventAsync(
            $"{uniqueMarker} Earlier Festival",
            DateTimeOffset.UtcNow.AddDays(-2));
        var newerEventId = await CreateEventAsync(
            $"{uniqueMarker} Later Festival",
            DateTimeOffset.UtcNow.AddDays(-1));

        var response = await Fixture.HttpClient.GetAsync(
            $"{EndpointUri}/search?searchQuery={uniqueMarker.ToLower()}");

        response.EnsureSuccessStatusCode();
        var page = await response.Content.ReadFromJsonAsync<PaginationResult<EventNewsDto>>();

        Assert.NotNull(page);
        Assert.Equal(2, page.TotalItemsCount);
        Assert.Collection(
            page.Items,
            item => Assert.Equal(newerEventId, item.Id),
            item => Assert.Equal(olderEventId, item.Id));
    }

    [Fact]
    public async Task Search_ShouldNotReturnEventsThatDoNotMatchTheQuery()
    {
        var uniqueMarker = $"SrchMrk{Guid.NewGuid():N}";
        await CreateEventAsync($"Unrelated title {Guid.NewGuid():N}", DateTimeOffset.UtcNow);

        var response = await Fixture.HttpClient.GetAsync(
            $"{EndpointUri}/search?searchQuery={uniqueMarker}");

        response.EnsureSuccessStatusCode();
        var page = await response.Content.ReadFromJsonAsync<PaginationResult<EventNewsDto>>();

        Assert.NotNull(page);
        Assert.Empty(page.Items);
        Assert.Equal(0, page.TotalItemsCount);
    }

    private async Task<long> CreateEventAsync(string title, DateTimeOffset publishedAt)
    {
        var createEventNewsDto = new CreateEventNewsDto
        {
            Title = title,
            Description = "Valid event description",
            Status = Status.Published,
            PublishedAt = publishedAt,
            PreviewImageId = 1,
            CategoryIds = [1],
            Localizations =
            [
                new CreateEventNewsLocalizationDto
                {
                    LanguageId = 1,
                    Title = title,
                    Description = "Valid event description",
                },
            ],
        };

        var serializedDto = JsonConvert.SerializeObject(createEventNewsDto);

        var response = await Fixture.HttpClient.PostAsync(
            $"{EndpointUri}/",
            new StringContent(serializedDto, Encoding.UTF8, "application/json"));

        response.EnsureSuccessStatusCode();

        var created = JsonConvert.DeserializeObject<EventNewsDto>(await response.Content.ReadAsStringAsync());

        Assert.NotNull(created);

        return created.Id;
    }
}
