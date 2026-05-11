using System.Net;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using VictoryCenter.BLL.DTOs.Admin.Localization.MainPage;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;
using MainPageEntity = VictoryCenter.DAL.Entities.MainPage;

namespace VictoryCenter.IntegrationTests.ControllerTests.Localizations.MainPageLocalizations.Update;

public class UpdateMainPageLocalizationTests : BaseTestClass
{
    public UpdateMainPageLocalizationTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task UpdateMainPageLocalization_ShouldReturnOk()
    {
        var (mainPage, languageId) = await EnsureMainPageWithLocalizationsAsync();
        var dto = new UpdateMainPageLocalizationDto
        {
            Title = "Updated main page title",
            Description = "Updated main page description",
            MainAboutUs = new UpdateMainAboutUsLocalizationDto
            {
                Title = "Updated about us title",
                Description = "Updated about us description"
            },
            MainPartners = new UpdateMainPartnersLocalizationDto
            {
                Title = "Updated partners title",
                Description = "Updated partners description"
            }
        };

        var response = await Fixture.HttpClient.PutAsync(
            $"/api/MainPageLocalizations/{mainPage.Id}/{languageId}",
            Serialize(dto));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(response.IsSuccessStatusCode);
    }

    [Fact]
    public async Task UpdateMainPageLocalization_ShouldReturnNotFound()
    {
        var languageId = await GetExistingLanguageIdAsync();
        var dto = new UpdateMainPageLocalizationDto
        {
            Title = "Updated main page title",
            Description = "Updated main page description"
        };

        var response = await Fixture.HttpClient.PutAsync(
            $"/api/MainPageLocalizations/99999/{languageId}",
            Serialize(dto));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateMainPageLocalization_ShouldReturnBadRequest()
    {
        var (mainPage, languageId) = await EnsureMainPageWithLocalizationsAsync();
        var dto = new UpdateMainPageLocalizationDto
        {
            Title = "t",
            Description = "Updated main page description"
        };

        var response = await Fixture.HttpClient.PutAsync(
            $"/api/MainPageLocalizations/{mainPage.Id}/{languageId}",
            Serialize(dto));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    private static StringContent Serialize(object obj) =>
        new(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");

    private async Task<(MainPageEntity mainPage, long languageId)> EnsureMainPageWithLocalizationsAsync()
    {
        var languageId = await GetExistingLanguageIdAsync();

        var mainPage = await Fixture.DbContext.MainPages
            .Include(m => m.MainAboutUs)
            .Include(m => m.MainPartners)
            .FirstOrDefaultAsync();

        if (mainPage is null)
        {
            mainPage = new MainPageEntity
            {
                Title = "Seed MainPage title",
                Description = "Seed MainPage description",
                MainAboutUs = new MainAboutUs
                {
                    Title = "Seed About Us title",
                    Description = "Seed About Us description"
                },
                MainPartners = new MainPartners
                {
                    Title = "Seed Partners title",
                    Description = "Seed Partners description"
                }
            };

            await Fixture.DbContext.MainPages.AddAsync(mainPage);
            await Fixture.DbContext.SaveChangesAsync();
        }

        if (mainPage.MainAboutUs is null)
        {
            mainPage.MainAboutUs = new MainAboutUs
            {
                Title = "Seed About Us title",
                Description = "Seed About Us description",
                MainPageId = mainPage.Id
            };

            await Fixture.DbContext.MainAboutUs.AddAsync(mainPage.MainAboutUs);
            await Fixture.DbContext.SaveChangesAsync();
        }

        if (mainPage.MainPartners is null)
        {
            mainPage.MainPartners = new MainPartners
            {
                Title = "Seed Partners title",
                Description = "Seed Partners description",
                MainPageId = mainPage.Id
            };

            await Fixture.DbContext.MainPartners.AddAsync(mainPage.MainPartners);
            await Fixture.DbContext.SaveChangesAsync();
        }

        await EnsureLocalizationExistsAsync(mainPage.Id, languageId);
        await EnsureAboutUsLocalizationExistsAsync(mainPage.MainAboutUs.Id, languageId);
        await EnsurePartnersLocalizationExistsAsync(mainPage.MainPartners.Id, languageId);

        return (mainPage, languageId);
    }

    private async Task<long> GetExistingLanguageIdAsync()
    {
        var language = await Fixture.DbContext.LocalizationLanguages.FirstOrDefaultAsync(l => l.Id == 2)
            ?? await Fixture.DbContext.LocalizationLanguages.FirstOrDefaultAsync()
            ?? throw new InvalidOperationException("Couldn't setup existing language");

        return language.Id;
    }

    private async Task EnsureLocalizationExistsAsync(long entityId, long languageId)
    {
        var existing = await Fixture.DbContext.MainPageLocalizations
            .FirstOrDefaultAsync(l => l.EntityId == entityId && l.LanguageId == languageId);

        if (existing is not null)
        {
            return;
        }

        Fixture.DbContext.MainPageLocalizations.Add(new MainPageLocalization
        {
            EntityId = entityId,
            LanguageId = languageId,
            Title = "Seed localized title",
            Description = "Seed localized description"
        });

        await Fixture.DbContext.SaveChangesAsync();
    }

    private async Task EnsureAboutUsLocalizationExistsAsync(long entityId, long languageId)
    {
        var existing = await Fixture.DbContext.MainAboutUsLocalizations
            .FirstOrDefaultAsync(l => l.EntityId == entityId && l.LanguageId == languageId);

        if (existing is not null)
        {
            return;
        }

        Fixture.DbContext.MainAboutUsLocalizations.Add(new MainAboutUsLocalization
        {
            EntityId = entityId,
            LanguageId = languageId,
            Title = "Seed about us localized title",
            Description = "Seed about us localized description"
        });

        await Fixture.DbContext.SaveChangesAsync();
    }

    private async Task EnsurePartnersLocalizationExistsAsync(long entityId, long languageId)
    {
        var existing = await Fixture.DbContext.MainPartnersLocalizations
            .FirstOrDefaultAsync(l => l.EntityId == entityId && l.LanguageId == languageId);

        if (existing is not null)
        {
            return;
        }

        Fixture.DbContext.MainPartnersLocalizations.Add(new MainPartnersLocalization
        {
            EntityId = entityId,
            LanguageId = languageId,
            Title = "Seed partners localized title",
            Description = "Seed partners localized description"
        });

        await Fixture.DbContext.SaveChangesAsync();
    }
}
