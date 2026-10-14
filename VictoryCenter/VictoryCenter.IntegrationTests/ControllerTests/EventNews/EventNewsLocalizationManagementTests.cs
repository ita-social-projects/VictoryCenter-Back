using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Admin.Localization.EventNews;
using VictoryCenter.DAL.Enums;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;
using EventNewsEntity = VictoryCenter.DAL.Entities.EventNews;

namespace VictoryCenter.IntegrationTests.ControllerTests.EventNews;

public class EventNewsLocalizationManagementTests : BaseTestClass
{
    private const string BaseUri = "/api/EventNewsLocalizations";

    public EventNewsLocalizationManagementTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task LocalizationManagement_ShouldSupportAuthorizedCrud()
    {
        var language = await Fixture.DbContext.LocalizationLanguages.AsNoTracking().FirstAsync();
        var eventNews = new EventNewsEntity
        {
            Status = Status.Draft,
            CreatedAt = DateTimeOffset.UtcNow
        };
        Fixture.DbContext.EventNews.Add(eventNews);
        await Fixture.DbContext.SaveChangesAsync();
        Fixture.DbContext.ChangeTracker.Clear();

        // 1. Create localization
        var createDto = new CreateAdminEventNewsLocalizationDto
        {
            EntityId = eventNews.Id,
            LanguageId = language.Id,
            Title = "Initial Event Title",
            Description = "Initial Description",
            AdditionalDescription = "Short Info"
        };
        var createResponse = await Fixture.HttpClient.PostAsJsonAsync(BaseUri, createDto);
        Assert.Equal(HttpStatusCode.OK, createResponse.StatusCode);

        var createdLocalization = await createResponse.Content.ReadFromJsonAsync<AdminEventNewsLocalizationDto>();
        Assert.NotNull(createdLocalization);
        Assert.Equal(eventNews.Id, createdLocalization.EntityId);
        Assert.Equal(language.Id, createdLocalization.LocalizationInfoDto.Id);
        Assert.Equal(createDto.Title, createdLocalization.Title);
        Assert.Equal(createDto.Description, createdLocalization.Description);
        Assert.Equal(TranslationStatus.Relevant, createdLocalization.TranslationStatus);

        // 2. Update localization
        var updateDto = new UpdateEventNewsLocalizationDto
        {
            Title = "Updated Event Title",
            Description = "Updated Description",
            AdditionalDescription = "Updated Info"
        };
        var updateResponse = await Fixture.HttpClient.PutAsJsonAsync(
            $"{BaseUri}/{eventNews.Id}/{language.Id}",
            updateDto);
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        var updatedLocalization = await updateResponse.Content.ReadFromJsonAsync<AdminEventNewsLocalizationDto>();
        Assert.NotNull(updatedLocalization);
        Assert.Equal(updateDto.Title, updatedLocalization.Title);
        Assert.Equal(updateDto.Description, updatedLocalization.Description);

        // 3. Delete localization
        var deleteResponse = await Fixture.HttpClient.DeleteAsync(
            $"{BaseUri}/{eventNews.Id}/{language.Id}");
        Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);

        var deleteResult = await deleteResponse.Content.ReadFromJsonAsync<DeleteEventNewsLocalizationDto>();
        Assert.NotNull(deleteResult);
        Assert.Equal(eventNews.Id, deleteResult.EntityId);
        Assert.Equal(language.Id, deleteResult.LanguageId);

        Fixture.DbContext.ChangeTracker.Clear();
        var existsInDb = await Fixture.DbContext.EventNewsLocalizations.AnyAsync(l =>
            l.EntityId == eventNews.Id && l.LanguageId == language.Id);
        Assert.False(existsInDb);
    }

    [Fact]
    public async Task LocalizationManagement_ShouldReturnNotFoundForMissingResources()
    {
        const long missingId = long.MaxValue;
        var language = await Fixture.DbContext.LocalizationLanguages.AsNoTracking().FirstAsync();
        var eventNews = await Fixture.DbContext.EventNews.AsNoTracking().FirstAsync();

        // Create with missing entity
        var createMissingEntityResponse = await Fixture.HttpClient.PostAsJsonAsync(
            BaseUri,
            new CreateAdminEventNewsLocalizationDto
            {
                EntityId = missingId,
                LanguageId = language.Id,
                Title = "Missing Entity"
            });
        Assert.Equal(HttpStatusCode.NotFound, createMissingEntityResponse.StatusCode);

        // Create with missing language
        var createMissingLangResponse = await Fixture.HttpClient.PostAsJsonAsync(
            BaseUri,
            new CreateAdminEventNewsLocalizationDto
            {
                EntityId = eventNews.Id,
                LanguageId = missingId,
                Title = "Missing Language"
            });
        Assert.Equal(HttpStatusCode.NotFound, createMissingLangResponse.StatusCode);

        // Update missing localization
        var updateResponse = await Fixture.HttpClient.PutAsJsonAsync(
            $"{BaseUri}/{missingId}/{language.Id}",
            new UpdateEventNewsLocalizationDto { Title = "Update Missing" });
        Assert.Equal(HttpStatusCode.NotFound, updateResponse.StatusCode);

        // Delete missing localization
        var deleteResponse = await Fixture.HttpClient.DeleteAsync(
            $"{BaseUri}/{missingId}/{language.Id}");
        Assert.Equal(HttpStatusCode.NotFound, deleteResponse.StatusCode);
    }

    [Fact]
    public async Task LocalizationManagement_ShouldReturnBadRequestForInvalidInput()
    {
        // DELETE with entityId=0 or languageId=0 should trigger validation and return 400 BadRequest
        var deleteInvalidResponse = await Fixture.HttpClient.DeleteAsync($"{BaseUri}/0/0");
        Assert.Equal(HttpStatusCode.BadRequest, deleteInvalidResponse.StatusCode);

        // PUT with entityId=0 or languageId=0
        var updateInvalidRouteResponse = await Fixture.HttpClient.PutAsJsonAsync(
            $"{BaseUri}/0/0",
            new UpdateEventNewsLocalizationDto { Title = "Valid Title" });
        Assert.Equal(HttpStatusCode.BadRequest, updateInvalidRouteResponse.StatusCode);

        // PUT with empty Title
        var updateInvalidBodyResponse = await Fixture.HttpClient.PutAsJsonAsync(
            $"{BaseUri}/1/1",
            new UpdateEventNewsLocalizationDto { Title = string.Empty });
        Assert.Equal(HttpStatusCode.BadRequest, updateInvalidBodyResponse.StatusCode);

        // POST with empty Title
        var createInvalidTitleResponse = await Fixture.HttpClient.PostAsJsonAsync(
            BaseUri,
            new CreateAdminEventNewsLocalizationDto
            {
                EntityId = 1,
                LanguageId = 1,
                Title = string.Empty
            });
        Assert.Equal(HttpStatusCode.BadRequest, createInvalidTitleResponse.StatusCode);

        // POST with non-positive EntityId/LanguageId
        var createInvalidIdsResponse = await Fixture.HttpClient.PostAsJsonAsync(
            BaseUri,
            new CreateAdminEventNewsLocalizationDto
            {
                EntityId = 0,
                LanguageId = 0,
                Title = "Valid Title"
            });
        Assert.Equal(HttpStatusCode.BadRequest, createInvalidIdsResponse.StatusCode);
    }

    [Fact]
    public async Task LocalizationMutations_ShouldRequireAuthorization()
    {
        using var anonymousClient = Fixture.Factory.CreateClient();

        var createResponse = await anonymousClient.PostAsJsonAsync(
            BaseUri,
            new CreateAdminEventNewsLocalizationDto
            {
                EntityId = 1,
                LanguageId = 1,
                Title = "Unauthorized"
            });
        Assert.Equal(HttpStatusCode.Unauthorized, createResponse.StatusCode);

        var updateResponse = await anonymousClient.PutAsJsonAsync(
            $"{BaseUri}/1/1",
            new UpdateEventNewsLocalizationDto { Title = "Unauthorized" });
        Assert.Equal(HttpStatusCode.Unauthorized, updateResponse.StatusCode);

        var deleteResponse = await anonymousClient.DeleteAsync($"{BaseUri}/1/1");
        Assert.Equal(HttpStatusCode.Unauthorized, deleteResponse.StatusCode);
    }
}
