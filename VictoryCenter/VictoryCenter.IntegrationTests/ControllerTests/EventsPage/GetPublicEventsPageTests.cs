using System.Net;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using VictoryCenter.BLL.DTOs.Public.EventsPage;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.EventsPage;

public class GetPublicEventsPageTests : BaseTestClass
{
    private const string Url = "/api/EventsPage/public";

    public GetPublicEventsPageTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task GetPublic_ShouldReturnOkWithIntroSection()
    {
        var intro = await Fixture.DbContext.EventsIntroSections.AsNoTracking().FirstAsync();

        var page = await GetPageAsync();

        Assert.Equal(intro.EventsBlockTitle, page.EventsBlockTitle);
        Assert.Equal(intro.PageDescription, page.PageDescription);
    }

    [Fact]
    public async Task GetPublic_ShouldReturnAllCategoriesOrderedByName()
    {
        var expected = await Fixture.DbContext.EventNewsCategories
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => new { c.Id, c.Name })
            .ToListAsync();

        var page = await GetPageAsync();

        Assert.NotEmpty(page.Categories);
        Assert.Equal(
            expected.Select(c => (c.Id, c.Name)),
            page.Categories.Select(c => (c.Id, c.Name)));
    }

    [Fact]
    public async Task GetPublic_ShouldReturnCategoryLocalizations()
    {
        var category = await Fixture.DbContext.EventNewsCategories.FirstAsync();
        var language = await Fixture.DbContext.LocalizationLanguages.FirstAsync(l => l.Code == "en");
        Fixture.DbContext.EventNewsCategoryLocalizations.Add(new EventNewsCategoryLocalization
        {
            EntityId = category.Id,
            LanguageId = language.Id,
            Name = "Localized category name",
            CreatedAt = TimeProvider.System.GetUtcNow()
        });
        await Fixture.DbContext.SaveChangesAsync();
        Fixture.DbContext.ChangeTracker.Clear();

        var page = await GetPageAsync();

        var returned = Assert.Single(page.Categories, c => c.Id == category.Id);
        var localization = Assert.Single(returned.Localizations, l => l.Language.Code == language.Code);
        Assert.Equal("Localized category name", localization.Name);
    }

    [Fact]
    public async Task GetPublic_WhenIntroSectionIsMissing_ShouldReturnNotFound()
    {
        var sections = await Fixture.DbContext.EventsIntroSections.AsNoTracking().ToListAsync();
        Fixture.DbContext.EventsIntroSections.RemoveRange(
            await Fixture.DbContext.EventsIntroSections.ToListAsync());
        await Fixture.DbContext.SaveChangesAsync();
        Fixture.DbContext.ChangeTracker.Clear();

        try
        {
            var response = await Fixture.HttpClient.GetAsync(Url);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
        finally
        {
            Fixture.DbContext.EventsIntroSections.AddRange(sections);
            await Fixture.DbContext.SaveChangesAsync();
            Fixture.DbContext.ChangeTracker.Clear();
        }
    }

    private async Task<PublicEventsPageDto> GetPageAsync()
    {
        var response = await Fixture.HttpClient.GetAsync(Url);
        response.EnsureSuccessStatusCode();
        var page = JsonConvert.DeserializeObject<PublicEventsPageDto>(await response.Content.ReadAsStringAsync());
        Assert.NotNull(page);
        return page;
    }
}
