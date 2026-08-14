using System.Net;
using System.Text;
using Newtonsoft.Json;
using VictoryCenter.BLL.DTOs.Admin.EventNews;
using VictoryCenter.BLL.DTOs.Public.EventNews;
using VictoryCenter.DAL.Enums;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.EventNews.Create;

public class CreateEventNewsTests : BaseTestClass
{
    private const string EndpointUri = "/api/EventNews/";

    public CreateEventNewsTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task CreatePublishedEventNews_ShouldCreateEventNews()
    {
        var createEventNewsDto = PublishedDto("Published Event Title");
        var serializedDto = JsonConvert.SerializeObject(createEventNewsDto);

        HttpResponseMessage response = await Fixture.HttpClient.PostAsync(
            EndpointUri,
            new StringContent(serializedDto, Encoding.UTF8, "application/json"));

        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        EventNewsDto? responseContent = JsonConvert.DeserializeObject<EventNewsDto>(responseString);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(responseContent);
        Assert.Equal(Status.Published, responseContent.Status);
        Assert.NotNull(responseContent.Slug);
        Assert.Single(responseContent.Categories);
        Assert.Single(responseContent.Localizations);
    }

    [Fact]
    public async Task CreateDraftEventNews_ShouldCreateEventNews()
    {
        var createEventNewsDto = new CreateEventNewsDto
        {
            Status = Status.Draft,
            Localizations =
            [
                new CreateEventNewsLocalizationDto
                {
                    LanguageId = 1,
                    Title = "Draft Event Title"
                },
            ]
        };

        var serializedDto = JsonConvert.SerializeObject(createEventNewsDto);

        HttpResponseMessage response = await Fixture.HttpClient.PostAsync(
            EndpointUri,
            new StringContent(serializedDto, Encoding.UTF8, "application/json"));

        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        EventNewsDto? responseContent = JsonConvert.DeserializeObject<EventNewsDto>(responseString);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(responseContent);
        Assert.Equal(Status.Draft, responseContent.Status);
        Assert.NotNull(responseContent.Slug);
    }

    [Fact]
    public async Task CreateDraftEventNews_ShouldIgnoreLocalizationWithoutContent()
    {
        var createEventNewsDto = new CreateEventNewsDto
        {
            Status = Status.Draft,
            Localizations = [new CreateEventNewsLocalizationDto { LanguageId = 1 }]
        };

        var serializedDto = JsonConvert.SerializeObject(createEventNewsDto);
        HttpResponseMessage response = await Fixture.HttpClient.PostAsync(
            EndpointUri,
            new StringContent(serializedDto, Encoding.UTF8, "application/json"));

        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        EventNewsDto? responseContent = JsonConvert.DeserializeObject<EventNewsDto>(responseString);

        Assert.NotNull(responseContent);
        Assert.Null(responseContent.Slug);
        Assert.Empty(responseContent.Localizations);
    }

    [Fact]
    public async Task CreateDraftEventNews_ShouldIncrementSlug_WhenSlugAlreadyExists()
    {
        var firstDto = new CreateEventNewsDto
        {
            Status = Status.Draft,
            Localizations =
            [
                new CreateEventNewsLocalizationDto
                {
                    LanguageId = 1,
                    Title = "Duplicate Slug Review Test"
                },
            ]
        };

        var secondDto = firstDto with { };
        HttpResponseMessage firstResponse = await PostAsync(firstDto);
        HttpResponseMessage secondResponse = await PostAsync(secondDto);

        firstResponse.EnsureSuccessStatusCode();
        secondResponse.EnsureSuccessStatusCode();
        var firstContent = JsonConvert.DeserializeObject<EventNewsDto>(
            await firstResponse.Content.ReadAsStringAsync());
        var secondContent = JsonConvert.DeserializeObject<EventNewsDto>(
            await secondResponse.Content.ReadAsStringAsync());

        Assert.NotNull(firstContent);
        Assert.NotNull(secondContent);
        Assert.Equal($"{firstContent.Slug}-1", secondContent.Slug);
    }

    [Fact]
    public async Task CreatePublishedEventNews_ShouldNotCreateEventNews_WhenRequiredFieldsAreMissing()
    {
        var createEventNewsDto = new CreateEventNewsDto
        {
            Status = Status.Published
        };

        var serializedDto = JsonConvert.SerializeObject(createEventNewsDto);

        HttpResponseMessage response = await Fixture.HttpClient.PostAsync(
            EndpointUri,
            new StringContent(serializedDto, Encoding.UTF8, "application/json"));

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreatedDraftEventNews_ShouldNotBeReturnedByPublicPublishedEndpoint()
    {
        var createEventNewsDto = new CreateEventNewsDto
        {
            Status = Status.Draft,
            Localizations =
            [
                new CreateEventNewsLocalizationDto
                {
                    LanguageId = 1,
                    Title = "Private Draft Title"
                },
            ]
        };

        var serializedDto = JsonConvert.SerializeObject(createEventNewsDto);

        HttpResponseMessage createResponse = await Fixture.HttpClient.PostAsync(
            EndpointUri,
            new StringContent(serializedDto, Encoding.UTF8, "application/json"));

        createResponse.EnsureSuccessStatusCode();
        var createResponseString = await createResponse.Content.ReadAsStringAsync();
        EventNewsDto? createdEventNews = JsonConvert.DeserializeObject<EventNewsDto>(createResponseString);

        HttpResponseMessage publicResponse = await Fixture.HttpClient.GetAsync("/api/EventNews/published");

        publicResponse.EnsureSuccessStatusCode();
        var publicResponseString = await publicResponse.Content.ReadAsStringAsync();
        List<PublishedEventNewsDto>? publishedEventNews = JsonConvert.DeserializeObject<List<PublishedEventNewsDto>>(publicResponseString);

        Assert.NotNull(createdEventNews);
        Assert.NotNull(publishedEventNews);
        Assert.DoesNotContain(publishedEventNews, eventNews => eventNews.Slug == createdEventNews.Slug);
    }

    private static CreateEventNewsDto PublishedDto(string title)
    {
        return new CreateEventNewsDto
        {
            Status = Status.Published,
            PublishedAt = DateTimeOffset.UtcNow,
            PreviewImageId = 1,
            CategoryIds = [1],
            Localizations =
            [
                new CreateEventNewsLocalizationDto
                {
                    LanguageId = 1,
                    Title = title,
                    Description = "Valid event description"
                },
            ]
        };
    }

    private async Task<HttpResponseMessage> PostAsync(CreateEventNewsDto dto)
    {
        var serializedDto = JsonConvert.SerializeObject(dto);
        return await Fixture.HttpClient.PostAsync(
            EndpointUri,
            new StringContent(serializedDto, Encoding.UTF8, "application/json"));
    }
}
