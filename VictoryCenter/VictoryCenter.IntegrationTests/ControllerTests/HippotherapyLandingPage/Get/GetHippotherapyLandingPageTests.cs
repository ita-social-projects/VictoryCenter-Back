using System.Net;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyLandingPage;
using VictoryCenter.BLL.Enums;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.HippotherapyLandingPage.Get;

public class GetHippotherapyLandingPageTests : BaseTestClass
{
    private readonly Uri _endpointUri = new("/api/HippotherapyPage", UriKind.Relative);

    public GetHippotherapyLandingPageTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task Get_PageIsAutoSeededOnStartup_ShouldReturnOkWithAllSectionsAndLocalizations()
    {
        var (response, content) = await GetPageAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(content);
        Assert.False(string.IsNullOrWhiteSpace(content.IntroSection.Title));
        Assert.False(string.IsNullOrWhiteSpace(content.HippoventionCenterSection.Pros));
        Assert.Equal(4, content.AdvantagesSection.Cards.Count);
        Assert.Equal(4, content.ParticipantsSection.Cards.Count);
        Assert.Equal(4, content.EthicsSection.Principles.Count);
        Assert.NotEmpty(content.ScientificReferencesSection.ScientificReferences);

        Assert.NotNull(content.Localizations);
        Assert.NotNull(content.TranslationStatuses);
    }

    [Fact]
    public async Task Get_NoRowInDatabase_ShouldReturnNotFound()
    {
        var seededRows = await Fixture.DbContext.HippotherapyLandingPages.ToListAsync();
        Fixture.DbContext.HippotherapyLandingPages.RemoveRange(seededRows);
        await Fixture.DbContext.SaveChangesAsync();
        Fixture.DbContext.ChangeTracker.Clear();

        var response = await Fixture.HttpClient.GetAsync(_endpointUri);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Get_PageWithMissingSection_ShouldReturnNotFoundInsteadOfServerError()
    {
        var page = await Fixture.DbContext.HippotherapyLandingPages
            .Include(p => p.IntroSection)
            .FirstAsync();
        Fixture.DbContext.Remove(page.IntroSection);
        await Fixture.DbContext.SaveChangesAsync();
        Fixture.DbContext.ChangeTracker.Clear();

        var response = await Fixture.HttpClient.GetAsync(_endpointUri);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Get_WhenIntroIsTranslated_ShouldReturnLocalizationAndStatusForThatLanguageOnly()
    {
        var (introSectionId, language) = await PrepareIntroTranslationAsync("Intro EN", "Desc EN");

        var (response, content) = await GetPageAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(content);

        var localization = Assert.Single(content.Localizations, l => l.LocalizationInfoDto.Id == language.Id);
        Assert.NotNull(localization.IntroSection);
        Assert.Equal("Intro EN", localization.IntroSection.Title);
        Assert.Equal("Desc EN", localization.IntroSection.Description);
        Assert.Equal(TranslationStatus.Relevant, localization.TranslationStatus);

        var introStatus = Assert.Single(
            content.TranslationStatuses,
            s => s.Block == HippotherapyLandingPageBlock.Intro && s.LanguageId == language.Id);
        Assert.Equal(TranslationStatus.Relevant, introStatus.TranslationStatus);
        Assert.True(introSectionId > 0);
    }

    [Fact]
    public async Task Get_WhenIntroTranslationIsOutdated_ShouldMarkLanguageAndBlockAsOutdated()
    {
        var (_, language) = await PrepareIntroTranslationAsync("Intro EN", "Desc EN", TranslationStatus.Outdated);

        var (_, content) = await GetPageAsync();

        Assert.NotNull(content);
        var localization = Assert.Single(content.Localizations, l => l.LocalizationInfoDto.Id == language.Id);
        Assert.Equal(TranslationStatus.Outdated, localization.TranslationStatus);
        var introStatus = Assert.Single(
            content.TranslationStatuses,
            s => s.Block == HippotherapyLandingPageBlock.Intro && s.LanguageId == language.Id);
        Assert.Equal(TranslationStatus.Outdated, introStatus.TranslationStatus);
    }

    [Fact]
    public async Task Get_WhenLanguageHasNoTranslation_ShouldNotReturnLocalizationForIt()
    {
        var language = await Fixture.DbContext.LocalizationLanguages.FirstAsync(l => l.Code == "en");
        await RemoveAllTranslationsForLanguageAsync(language.Id);

        var (_, content) = await GetPageAsync();

        Assert.NotNull(content);
        Assert.DoesNotContain(content.Localizations, l => l.LocalizationInfoDto.Id == language.Id);
        Assert.DoesNotContain(content.TranslationStatuses, s => s.LanguageId == language.Id);
    }

    [Fact]
    public async Task Get_WhenTranslationsExist_ShouldReturnAllItemsOfNestedSectionsWithDistinctIds()
    {
        var (_, content) = await GetPageAsync();

        Assert.NotNull(content);
        foreach (var localization in content.Localizations)
        {
            if (localization.AdvantagesSection is { } advantages)
            {
                Assert.Equal(content.AdvantagesSection.Cards.Count, advantages.Cards.Count);
                Assert.Equal(advantages.Cards.Count, advantages.Cards.Select(c => c.CardId).Distinct().Count());
            }

            if (localization.ParticipantsSection is { } participants)
            {
                Assert.Equal(content.ParticipantsSection.Cards.Count, participants.Cards.Count);
            }

            if (localization.EthicsSection is { } ethics)
            {
                Assert.Equal(content.EthicsSection.Principles.Count, ethics.Principles.Count);
            }
        }
    }

    [Fact]
    public async Task Get_WithoutAuthorization_ShouldReturnUnauthorized()
    {
        using var anonymousClient = Fixture.Factory.CreateClient();

        var response = await anonymousClient.GetAsync(_endpointUri);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    private async Task<(HttpResponseMessage Response, HippotherapyLandingPageDto? Content)> GetPageAsync()
    {
        var response = await Fixture.HttpClient.GetAsync(_endpointUri);
        var responseString = await response.Content.ReadAsStringAsync();
        var content = response.IsSuccessStatusCode
            ? JsonSerializer.Deserialize<HippotherapyLandingPageDto>(responseString, JsonOptions)
            : null;

        return (response, content);
    }

    private async Task<(long IntroSectionId, LocalizationLanguage Language)> PrepareIntroTranslationAsync(
        string title,
        string description,
        TranslationStatus status = TranslationStatus.Relevant)
    {
        var page = await Fixture.DbContext.HippotherapyLandingPages
            .Include(p => p.IntroSection)
            .FirstAsync();
        var language = await Fixture.DbContext.LocalizationLanguages.FirstAsync(l => l.Code == "en");

        var existing = await Fixture.DbContext.Set<HippotherapyLandingPageIntroSectionLocalization>()
            .Where(l => l.EntityId == page.IntroSection.Id && l.LanguageId == language.Id)
            .ToListAsync();
        Fixture.DbContext.RemoveRange(existing);

        Fixture.DbContext.Add(new HippotherapyLandingPageIntroSectionLocalization
        {
            EntityId = page.IntroSection.Id,
            LanguageId = language.Id,
            Title = title,
            Description = description,
            TranslationStatus = status,
            CreatedAt = DateTimeOffset.UtcNow,
        });
        await Fixture.DbContext.SaveChangesAsync();
        Fixture.DbContext.ChangeTracker.Clear();

        return (page.IntroSection.Id, language);
    }

    private async Task RemoveAllTranslationsForLanguageAsync(long languageId)
    {
        var intro = await Fixture.DbContext.Set<HippotherapyLandingPageIntroSectionLocalization>()
            .Where(l => l.LanguageId == languageId)
            .ToListAsync();
        Fixture.DbContext.RemoveRange(intro);
        await Fixture.DbContext.SaveChangesAsync();
        Fixture.DbContext.ChangeTracker.Clear();
    }
}
