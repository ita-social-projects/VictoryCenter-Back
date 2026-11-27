using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgram;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests;

public class PublicHippotherapyProgramLocalizationsTests : BaseTestClass
{
    private const string BaseUrl = "/api/HippotherapyProgramLocalizations";

    public PublicHippotherapyProgramLocalizationsTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task AnonymousReads_ReturnOnlyPublishedLocalizations()
    {
        var db = Fixture.DbContext;
        var language = await db.LocalizationLanguages.FirstAsync();
        var draft = new HippotherapyProgram { Name = "Draft", Status = Status.Draft };
        var published = new HippotherapyProgram { Name = "Published", Status = Status.Published };
        db.HippotherapyProgramLocalizations.AddRange(
            new HippotherapyProgramLocalization { Entity = draft, LanguageId = language.Id, Name = "Draft translation" },
            new HippotherapyProgramLocalization { Entity = published, LanguageId = language.Id, Name = "Published translation" });
        await db.SaveChangesAsync();

        using var client = Fixture.Factory.CreateClient();
        var draftTranslations = await client.GetFromJsonAsync<HippotherapyProgramLocalizationDto[]>($"{BaseUrl}/entityId/{draft.Id}");
        var publishedTranslations = await client.GetFromJsonAsync<HippotherapyProgramLocalizationDto[]>($"{BaseUrl}/entityId/{published.Id}");
        var languageTranslations = await client.GetFromJsonAsync<HippotherapyProgramLocalizationDto[]>($"{BaseUrl}/languageId/{language.Id}");

        Assert.NotNull(draftTranslations);
        Assert.NotNull(publishedTranslations);
        Assert.NotNull(languageTranslations);
        Assert.Empty(draftTranslations);
        Assert.Equal("Published translation", Assert.Single(publishedTranslations).Name);
        Assert.DoesNotContain(languageTranslations, translation => translation.EntityId == draft.Id);
        Assert.Contains(languageTranslations, translation => translation.EntityId == published.Id);
    }
}
