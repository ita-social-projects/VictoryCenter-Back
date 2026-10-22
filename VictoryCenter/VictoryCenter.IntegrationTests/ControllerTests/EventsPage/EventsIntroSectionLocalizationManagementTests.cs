using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Admin.Localization.EventsIntroSection;
using VictoryCenter.DAL.Enums;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.EventsPage;

public class EventsIntroSectionLocalizationManagementTests : BaseTestClass
{
    private const string Endpoint = "/api/EventsIntroSectionLocalizations";

    public EventsIntroSectionLocalizationManagementTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task LocalizationManagement_ShouldSupportAuthorizedCrud()
    {
        var suffix = Guid.NewGuid().ToString("N")[..8];
        var title = $"Title{suffix}";
        var updatedTitle = $"Upd{suffix}";
        var description = $"Description text for {suffix} goes here.";
        var section = await Fixture.DbContext.EventsIntroSections.AsNoTracking().SingleAsync();
        var language = await Fixture.DbContext.LocalizationLanguages.AsNoTracking().FirstAsync();

        var createResponse = await Fixture.HttpClient.PostAsJsonAsync(
            Endpoint,
            new CreateEventsIntroSectionLocalizationDto
            {
                EntityId = section.Id,
                LanguageId = language.Id,
                PageDescription = description
            });
        Assert.Equal(HttpStatusCode.OK, createResponse.StatusCode);
        var localization = await createResponse.Content
            .ReadFromJsonAsync<EventsIntroSectionLocalizationDto>();
        Assert.NotNull(localization);
        Assert.Equal(section.Id, localization.EntityId);
        Assert.Equal(language.Id, localization.LocalizationInfoDto.Id);
        Assert.Equal(language.Code, localization.LocalizationInfoDto.Code);
        Assert.Equal(TranslationStatus.Relevant, localization.TranslationStatus);
        Assert.Null(localization.EventsBlockTitle);
        Assert.Equal(description, localization.PageDescription);

        var updateResponse = await Fixture.HttpClient.PutAsJsonAsync(
            $"{Endpoint}/{section.Id}/{language.Id}",
            new UpdateEventsIntroSectionLocalizationDto
            {
                EventsBlockTitle = updatedTitle,
            });
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        var getResponse = await Fixture.HttpClient.GetAsync($"{Endpoint}/{section.Id}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        var localizations = await getResponse.Content
            .ReadFromJsonAsync<List<EventsIntroSectionLocalizationDto>>();
        var updatedLocalization = Assert.Single(localizations!);
        Assert.Equal(updatedTitle, updatedLocalization.EventsBlockTitle);
        Assert.Equal(description, updatedLocalization.PageDescription);

        var deleteResponse = await Fixture.HttpClient.DeleteAsync($"{Endpoint}/{section.Id}/{language.Id}");
        Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);

        var getAfterDeleteResponse = await Fixture.HttpClient.GetAsync($"{Endpoint}/{section.Id}");
        Assert.Equal(HttpStatusCode.OK, getAfterDeleteResponse.StatusCode);
        var localizationsAfterDelete = await getAfterDeleteResponse.Content
            .ReadFromJsonAsync<List<EventsIntroSectionLocalizationDto>>();
        Assert.Empty(localizationsAfterDelete!);
    }

    [Fact]
    public async Task LocalizationManagement_ShouldReturnNotFoundForMissingResources()
    {
        const long missingId = long.MaxValue;
        var language = await Fixture.DbContext.LocalizationLanguages.AsNoTracking().FirstAsync();

        var createResponse = await Fixture.HttpClient.PostAsJsonAsync(
            Endpoint,
            new CreateEventsIntroSectionLocalizationDto
            {
                EntityId = missingId,
                LanguageId = language.Id,
                EventsBlockTitle = "Missing title text",
                PageDescription = "Missing description text goes here."
            });
        var getResponse = await Fixture.HttpClient.GetAsync($"{Endpoint}/{missingId}");
        var updateResponse = await Fixture.HttpClient.PutAsJsonAsync(
            $"{Endpoint}/{missingId}/{language.Id}",
            new UpdateEventsIntroSectionLocalizationDto
            {
                EventsBlockTitle = "Missing title text",
                PageDescription = "Missing description text goes here."
            });
        var deleteResponse = await Fixture.HttpClient.DeleteAsync($"{Endpoint}/{missingId}/{language.Id}");

        Assert.Equal(HttpStatusCode.NotFound, createResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        var emptyLocalizations = await getResponse.Content
            .ReadFromJsonAsync<List<EventsIntroSectionLocalizationDto>>();
        Assert.Empty(emptyLocalizations!);
        Assert.Equal(HttpStatusCode.NotFound, updateResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, deleteResponse.StatusCode);
    }

    [Fact]
    public async Task LocalizationMutations_ShouldRequireAuthorization()
    {
        using var anonymousClient = Fixture.Factory.CreateClient();

        var response = await anonymousClient.PostAsJsonAsync(
            Endpoint,
            new CreateEventsIntroSectionLocalizationDto
            {
                EntityId = 1,
                LanguageId = 1,
                EventsBlockTitle = "Unauthorized",
                PageDescription = "Unauthorized description text goes here."
            });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

        var updateResponse = await anonymousClient.PutAsJsonAsync(
            $"{Endpoint}/1/1",
            new UpdateEventsIntroSectionLocalizationDto { EventsBlockTitle = "Unauthorized" });

        Assert.Equal(HttpStatusCode.Unauthorized, updateResponse.StatusCode);
    }

    [Fact]
    public async Task LocalizationManagement_ShouldReturnBadRequest_WhenNoFieldsAreProvided()
    {
        var section = await Fixture.DbContext.EventsIntroSections.AsNoTracking().SingleAsync();
        var language = await Fixture.DbContext.LocalizationLanguages.AsNoTracking().FirstAsync();

        var createResponse = await Fixture.HttpClient.PostAsJsonAsync(
            Endpoint,
            new CreateEventsIntroSectionLocalizationDto { EntityId = section.Id, LanguageId = language.Id });
        var updateResponse = await Fixture.HttpClient.PutAsJsonAsync(
            $"{Endpoint}/{section.Id}/{language.Id}",
            new UpdateEventsIntroSectionLocalizationDto());

        Assert.Equal(HttpStatusCode.BadRequest, createResponse.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, updateResponse.StatusCode);
    }
}
