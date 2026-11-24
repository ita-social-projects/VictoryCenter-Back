using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Admin.Localization.TeamMembers;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests;

public class PublicTeamMemberLocalizationsTests : BaseTestClass
{
    private const string BaseUrl = "/api/TeamMemberLocalizations";

    public PublicTeamMemberLocalizationsTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task AnonymousReads_ReturnOnlyPublishedMemberLocalizations()
    {
        var db = Fixture.DbContext;
        var category = await db.TeamCategories.FirstAsync();
        var language = await db.LocalizationLanguages.FirstAsync();
        var draft = new TeamMember { CategoryId = category.Id, FullName = "Draft member", Status = Status.Draft };
        var published = new TeamMember { CategoryId = category.Id, FullName = "Published member", Status = Status.Published };
        db.TeamMemberLocalizations.AddRange(
            new TeamMemberLocalization { Entity = draft, LanguageId = language.Id, FullName = "Draft translation" },
            new TeamMemberLocalization { Entity = published, LanguageId = language.Id, FullName = "Published translation" });
        await db.SaveChangesAsync();

        using var client = Fixture.Factory.CreateClient();
        var draftTranslations = await client.GetFromJsonAsync<TeamMemberLocalizationDto[]>($"{BaseUrl}/entityId/{draft.Id}");
        var publishedTranslations = await client.GetFromJsonAsync<TeamMemberLocalizationDto[]>($"{BaseUrl}/entityId/{published.Id}");
        var languageTranslations = await client.GetFromJsonAsync<TeamMemberLocalizationDto[]>($"{BaseUrl}/languageId/{language.Id}");

        Assert.NotNull(draftTranslations);
        Assert.NotNull(publishedTranslations);
        Assert.NotNull(languageTranslations);
        Assert.Empty(draftTranslations);
        Assert.Equal("Published translation", Assert.Single(publishedTranslations).FullName);
        Assert.DoesNotContain(languageTranslations, translation => translation.EntityId == draft.Id);
        Assert.Contains(languageTranslations, translation => translation.EntityId == published.Id);
    }
}
