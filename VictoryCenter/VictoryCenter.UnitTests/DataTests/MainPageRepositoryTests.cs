using Microsoft.EntityFrameworkCore;
using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Models;
using VictoryCenter.DAL.Repositories.Realizations.MainPage;

namespace VictoryCenter.UnitTests.DataTests;

public class MainPageRepositoryTests
{
    public static IEnumerable<object[]> TranslationStates()
    {
        for (var combination = 0; combination < 54; combination++)
        {
            yield return [combination >= 27, combination % 3, (combination / 3) % 3, (combination / 9) % 3];
        }
    }

    [Theory]
    [MemberData(nameof(TranslationStates))]
    public async Task GetLocalizationAsync_ProjectsRequestedLanguageForAllBlockStates(bool rootTranslated, int aboutState, int partnerState, int donationState)
    {
        await using var context = Context();
        var english = new LocalizationLanguage { Code = "en", Name = "English" };
        var ukrainian = new LocalizationLanguage { Code = "uk", Name = "Ukrainian" };
        context.AddRange(english, ukrainian);
        var page = new DAL.Entities.MainPage { Title = "Source", Description = "Source" };
        page.Localizations.Add(new MainPageLocalization { Language = ukrainian, Title = "Other language" });
        if (rootTranslated)
        {
            page.Localizations.Add(new MainPageLocalization { Language = english, Title = null, Description = "Root description" });
        }

        if (aboutState > 0)
        {
            page.MainAboutUs = new MainAboutUs
            {
                Title = "Source", Description = "Source", Localizations =
                [new MainAboutUsLocalization { Language = aboutState == 2 ? english : ukrainian, Title = null, Description = "About description" }]
            };
        }

        if (partnerState > 0)
        {
            page.MainPartners = new MainPartners
            {
                Title = "Source", Description = "Source", Localizations =
                [new MainPartnersLocalization { Language = partnerState == 2 ? english : ukrainian, Title = "Partners", Description = null }]
            };
        }

        if (donationState > 0)
        {
            page.MainDonations = new MainDonations
            {
                Title = "Source", Description = "Source", Localizations =
                [new MainDonationsLocalization { Language = donationState == 2 ? english : ukrainian, Title = "Donations", Description = string.Empty }]
            };
        }

        context.Add(page);
        await context.SaveChangesAsync();
        if (aboutState == 2)
        {
            page.MainAboutUs!.Localizations.Single().TranslationStatus = TranslationStatus.Outdated;
            await context.SaveChangesAsync();
        }

        context.ChangeTracker.Clear();
        var row = await new MainPageRepository(context).GetLocalizationAsync(page.Id, english.Id);

        Assert.NotNull(row);
        AssertTranslation(row.Translation, rootTranslated ? page.Id : null, null, "Root description", TranslationStatus.Relevant, english);
        AssertTranslation(row.AboutUs, aboutState == 2 ? page.MainAboutUs!.Id : null, null, "About description", TranslationStatus.Outdated, english);
        AssertTranslation(row.Partners, partnerState == 2 ? page.MainPartners!.Id : null, "Partners", null, TranslationStatus.Relevant, english);
        AssertTranslation(row.Donations, donationState == 2 ? page.MainDonations!.Id : null, "Donations", string.Empty, TranslationStatus.Relevant, english);
        Assert.Empty(context.ChangeTracker.Entries());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(long.MaxValue)]
    public async Task GetLocalizationAsync_MissingPage_ReturnsNull(long id)
    {
        await using var context = Context();
        Assert.Null(await new MainPageRepository(context).GetLocalizationAsync(id, 1));
    }

    [Fact]
    public async Task GetLocalizationAsync_UnknownLanguage_PreservesPageWithNullTranslations()
    {
        await using var context = Context();
        var page = new DAL.Entities.MainPage { Title = "Source", Description = "Source" };
        context.Add(page);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var row = await new MainPageRepository(context).GetLocalizationAsync(page.Id, long.MaxValue);

        Assert.NotNull(row);
        Assert.Null(row.Translation);
        Assert.Null(row.AboutUs);
        Assert.Null(row.Partners);
        Assert.Null(row.Donations);
        Assert.Empty(context.ChangeTracker.Entries());
    }

    [Fact]
    public async Task GetLocalizationAsync_CancelledRequest_PropagatesCancellation()
    {
        await using var context = Context();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            new MainPageRepository(context).GetLocalizationAsync(1, 1, new CancellationToken(true)));
    }

    private static VictoryCenterDbContext Context() => new(new DbContextOptionsBuilder<VictoryCenterDbContext>()
        .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);

    private static void AssertTranslation(
        MainPageTranslationRow? actual,
        long? entityId,
        string? title,
        string? description,
        TranslationStatus status,
        LocalizationLanguage language)
    {
        if (entityId is null)
        {
            Assert.Null(actual);
            return;
        }

        Assert.NotNull(actual);
        Assert.Equal(entityId, actual.EntityId);
        Assert.Equal(language.Id, actual.LanguageId);
        Assert.Equal(language.Code, actual.LanguageCode);
        Assert.Equal(title, actual.Title);
        Assert.Equal(description, actual.Description);
        Assert.Equal(status, actual.TranslationStatus);
    }
}
