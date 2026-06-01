using System.Net;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using VictoryCenter.BLL.DTOs.Admin.Localization.MainPage;
using VictoryCenter.DAL.Entities;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;
using MainPageEntity = VictoryCenter.DAL.Entities.MainPage;

namespace VictoryCenter.IntegrationTests.ControllerTests.Localizations.MainPageLocalizations.Create;

public class CreateMainPageLocalizationTests : BaseTestClass
{
    private readonly Uri _endpointUri = new("/api/MainPageLocalizations", UriKind.Relative);

    public CreateMainPageLocalizationTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task CreateMainPageLocalization_ShouldReturnOk()
    {
        var (mainPage, languageId) = await CreateMainPageForLocalizationAsync();
        await RemoveExistingLocalizationsAsync(mainPage, languageId);

        var dto = new CreateMainPageLocalizationDto
        {
            EntityId = mainPage.Id,
            LanguageId = languageId,
            Title = "Created main page title",
            Description = "Created main page description",
            MainAboutUs = new CreateMainAboutUsLocalizationDto
            {
                EntityId = mainPage.MainAboutUs!.Id,
                Title = "Created about us title",
                Description = "Created about us description"
            },
            MainPartners = new CreateMainPartnersLocalizationDto
            {
                EntityId = mainPage.MainPartners!.Id,
                Title = "Created partners title",
                Description = "Created partners description"
            },
            MainDonations = new CreateMainDonationsLocalizationDto
            {
                EntityId = mainPage.MainDonations!.Id,
                Title = "Created donations title",
                Description = "Created donations description"
            }
        };

        var response = await Fixture.HttpClient.PostAsync(_endpointUri, Serialize(dto));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(await Fixture.DbContext.MainDonationsLocalizations
            .AnyAsync(l => l.EntityId == mainPage.MainDonations!.Id && l.LanguageId == languageId));
    }

    [Fact]
    public async Task CreateMainPageLocalization_ShouldReturnBadRequest_WhenMainDonationsIsInvalid()
    {
        var (mainPage, languageId) = await CreateMainPageForLocalizationAsync();
        await RemoveExistingLocalizationsAsync(mainPage, languageId);

        var dto = new CreateMainPageLocalizationDto
        {
            EntityId = mainPage.Id,
            LanguageId = languageId,
            MainDonations = new CreateMainDonationsLocalizationDto
            {
                EntityId = 0,
                Title = "Created donations title",
                Description = "Created donations description"
            }
        };

        var response = await Fixture.HttpClient.PostAsync(_endpointUri, Serialize(dto));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateMainPageLocalization_ShouldReturnNotFound()
    {
        var languageId = await GetExistingLanguageIdAsync();
        var dto = new CreateMainPageLocalizationDto
        {
            EntityId = 99999,
            LanguageId = languageId,
            Title = "Created main page title",
            Description = "Created main page description"
        };

        var response = await Fixture.HttpClient.PostAsync(_endpointUri, Serialize(dto));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private static StringContent Serialize(object obj) =>
        new(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");

    private async Task<(MainPageEntity mainPage, long languageId)> CreateMainPageForLocalizationAsync()
    {
        var languageId = await GetExistingLanguageIdAsync();
        var mainPage = new MainPageEntity
        {
            Title = "Seed MainPage localization title",
            Description = "Seed MainPage localization description",
            MainAboutUs = new MainAboutUs
            {
                Title = "Seed About Us title",
                Description = "Seed About Us description"
            },
            MainPartners = new MainPartners
            {
                Title = "Seed Partners title",
                Description = "Seed Partners description"
            },
            MainDonations = new MainDonations
            {
                Title = "Seed Donations title",
                Description = "Seed Donations description"
            }
        };

        await Fixture.DbContext.MainPages.AddAsync(mainPage);
        await Fixture.DbContext.SaveChangesAsync();
        Fixture.DbContext.ChangeTracker.Clear();

        mainPage = await Fixture.DbContext.MainPages
            .Include(m => m.MainAboutUs)
            .Include(m => m.MainPartners)
            .Include(m => m.MainDonations)
            .SingleAsync(m => m.Id == mainPage.Id);

        return (mainPage, languageId);
    }

    private async Task<long> GetExistingLanguageIdAsync()
    {
        var language = await Fixture.DbContext.LocalizationLanguages.FirstOrDefaultAsync(l => l.Id == 2)
            ?? await Fixture.DbContext.LocalizationLanguages.FirstOrDefaultAsync()
            ?? throw new InvalidOperationException("Couldn't setup existing language");

        return language.Id;
    }

    private async Task RemoveExistingLocalizationsAsync(MainPageEntity mainPage, long languageId)
    {
        Fixture.DbContext.MainPageLocalizations.RemoveRange(
            Fixture.DbContext.MainPageLocalizations.Where(l => l.EntityId == mainPage.Id && l.LanguageId == languageId));
        Fixture.DbContext.MainAboutUsLocalizations.RemoveRange(
            Fixture.DbContext.MainAboutUsLocalizations.Where(l => l.EntityId == mainPage.MainAboutUs!.Id && l.LanguageId == languageId));
        Fixture.DbContext.MainPartnersLocalizations.RemoveRange(
            Fixture.DbContext.MainPartnersLocalizations.Where(l => l.EntityId == mainPage.MainPartners!.Id && l.LanguageId == languageId));
        Fixture.DbContext.MainDonationsLocalizations.RemoveRange(
            Fixture.DbContext.MainDonationsLocalizations.Where(l => l.EntityId == mainPage.MainDonations!.Id && l.LanguageId == languageId));

        await Fixture.DbContext.SaveChangesAsync();
    }
}
