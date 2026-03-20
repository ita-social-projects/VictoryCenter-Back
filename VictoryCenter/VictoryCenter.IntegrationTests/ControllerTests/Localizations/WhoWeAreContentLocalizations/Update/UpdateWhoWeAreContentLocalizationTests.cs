using System.Net;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using VictoryCenter.BLL.DTOs.Admin.Localization.WhoWeAreContents;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.Localizations.WhoWeAreContentLocalizations.Update;

public class UpdateWhoWeAreContentLocalizationTests : BaseTestClass
{
    public UpdateWhoWeAreContentLocalizationTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task UpdateWhoWeAreContentLocalization_ShouldReturnOk()
    {
        var titleContent = await Fixture.DbContext.WhoWeAreContents
            .Include(c => c.Section)
            .FirstOrDefaultAsync(c => c.ContentType == ContentType.Title)
            ?? throw new InvalidOperationException("Couldn't setup existing entity");

        var language = await Fixture.DbContext.LocalizationLanguages
            .FirstOrDefaultAsync(l => l.Id == 2)
            ?? throw new InvalidOperationException("Couldn't setup existing entity");

        var existingLocalization = await EnsureLocalizationExists(titleContent.Id, language.Id);

        var dtos = new List<UpdateWhoWeAreContentLocalizationDto>
        {
            new()
            {
                EntityId = existingLocalization.EntityId,
                LanguageId = existingLocalization.LanguageId,
                Title = "Updated localized title"
            }
        };

        var response = await Fixture.HttpClient.PutAsync(
            $"/api/WhoWeAreContentLocalizations/{(int)titleContent.Section.SectionType}",
            Serialize(dtos));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task UpdateWhoWeAreContentLocalization_ShouldReturnNotFound_WhenEntityIdDoesNotExist()
    {
        var section = await Fixture.DbContext.WhoWeAreSections
            .FirstOrDefaultAsync()
            ?? throw new InvalidOperationException("Couldn't setup existing entity");

        var language = await Fixture.DbContext.LocalizationLanguages
            .FirstOrDefaultAsync(l => l.Id == 2)
            ?? throw new InvalidOperationException("Couldn't setup existing entity");

        var dtos = new List<UpdateWhoWeAreContentLocalizationDto>
        {
            new()
            {
                EntityId = 99999,
                LanguageId = language.Id,
                Title = "Updated localized title"
            }
        };

        var response = await Fixture.HttpClient.PutAsync(
            $"/api/WhoWeAreContentLocalizations/{(int)section.SectionType}",
            Serialize(dtos));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateWhoWeAreContentLocalization_ShouldReturnBadRequest_WhenImageContentProvided()
    {
        var imageContent = await Fixture.DbContext.WhoWeAreContents
            .Include(c => c.Section)
            .FirstOrDefaultAsync(c => c.ContentType == ContentType.Image)
            ?? throw new InvalidOperationException("Couldn't setup existing entity");

        var language = await Fixture.DbContext.LocalizationLanguages
            .FirstOrDefaultAsync(l => l.Id == 2)
            ?? throw new InvalidOperationException("Couldn't setup existing entity");

        var dtos = new List<UpdateWhoWeAreContentLocalizationDto>
        {
            new()
            {
                EntityId = imageContent.Id,
                LanguageId = language.Id
            }
        };

        var response = await Fixture.HttpClient.PutAsync(
            $"/api/WhoWeAreContentLocalizations/{(int)imageContent.Section.SectionType}",
            Serialize(dtos));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateWhoWeAreContentLocalization_InvalidData_ShouldReturnBadRequest()
    {
        var dtos = new List<UpdateWhoWeAreContentLocalizationDto>
        {
            new()
            {
                EntityId = 0,
                LanguageId = 0,
                Title = "t"
            }
        };

        var response = await Fixture.HttpClient.PutAsync(
            $"/api/WhoWeAreContentLocalizations/{(int)SectionType.Main}",
            Serialize(dtos));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    private static StringContent Serialize(object obj) =>
        new(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");

    private async Task<WhoWeAreContentLocalization> EnsureLocalizationExists(long entityId, long languageId)
    {
        var existingLocalization = await Fixture.DbContext.WhoWeAreContentLocalizations
            .FirstOrDefaultAsync(l => l.EntityId == entityId && l.LanguageId == languageId);

        if (existingLocalization != null)
        {
            return existingLocalization;
        }

        var localization = new WhoWeAreContentLocalization
        {
            EntityId = entityId,
            LanguageId = languageId,
            Title = "Initial localized title"
        };

        Fixture.DbContext.WhoWeAreContentLocalizations.Add(localization);
        await Fixture.DbContext.SaveChangesAsync();

        return localization;
    }
}
