using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Admin.Localization.FaqQuestions;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests;

public class PublicFaqQuestionLocalizationsTests : BaseTestClass
{
    private const string BaseUrl = "/api/FaqQuestionLocalizations";

    public PublicFaqQuestionLocalizationsTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task AnonymousReads_ReturnOnlyPublishedLocalizations()
    {
        var db = Fixture.DbContext;
        var language = await db.LocalizationLanguages.FirstAsync();
        var draft = new FaqQuestion { QuestionText = "Draft", Status = Status.Draft };
        var published = new FaqQuestion { QuestionText = "Published", Status = Status.Published };
        db.FaqLocalizations.AddRange(
            new FaqQuestionLocalization { Entity = draft, LanguageId = language.Id, QuestionText = "Draft translation", AnswerText = "Translated answer" },
            new FaqQuestionLocalization { Entity = published, LanguageId = language.Id, QuestionText = "Published translation", AnswerText = "Translated answer" });
        await db.SaveChangesAsync();

        using var client = Fixture.Factory.CreateClient();
        var draftTranslations = await client.GetFromJsonAsync<FaqQuestionLocalizationDto[]>($"{BaseUrl}/entityId/{draft.Id}");
        var publishedTranslations = await client.GetFromJsonAsync<FaqQuestionLocalizationDto[]>($"{BaseUrl}/entityId/{published.Id}");
        var languageTranslations = await client.GetFromJsonAsync<FaqQuestionLocalizationDto[]>($"{BaseUrl}/languageId/{language.Id}");

        Assert.NotNull(draftTranslations);
        Assert.NotNull(publishedTranslations);
        Assert.NotNull(languageTranslations);
        Assert.Empty(draftTranslations);
        Assert.Equal("Published translation", Assert.Single(publishedTranslations).QuestionText);
        Assert.DoesNotContain(languageTranslations, translation => translation.EntityId == draft.Id);
        Assert.Contains(languageTranslations, translation => translation.EntityId == published.Id);
    }
}
