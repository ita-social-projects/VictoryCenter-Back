using System.Net;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using VictoryCenter.BLL.DTOs.Admin.Localization.WhoWeAreContents;
using VictoryCenter.DAL.Enums;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.Localizations.WhoWeAreContentLocalizations.Create;

public class CreateWhoWeAreContentLocalizationTests : BaseTestClass
{
    public CreateWhoWeAreContentLocalizationTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task CreateWhoWeAreContentLocalization_ShouldReturnOk()
    {
        var titleContent = await Fixture.DbContext.WhoWeAreContents
            .FirstOrDefaultAsync(c => c.ContentType == ContentType.Title)
            ?? throw new InvalidOperationException("Couldn't setup existing entity");

        var language = await Fixture.DbContext.LocalizationLanguages
            .FirstOrDefaultAsync(l => l.Id == 2)
            ?? throw new InvalidOperationException("Couldn't setup existing entity");

        var dtos = new List<CreateWhoWeAreContentLocalizationDto>
        {
            new()
            {
                EntityId = titleContent.Id,
                LanguageId = language.Id,
                Title = "Valid localized title"
            }
        };

        var response = await Fixture.HttpClient.PostAsync(
            $"/api/WhoWeAreContentLocalizations/{(int)SectionType.Main}",
            Serialize(dtos));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task CreateWhoWeAreContentLocalization_ShouldReturnNotFound_WhenEntityIdDoesNotExist()
    {
        var language = await Fixture.DbContext.LocalizationLanguages
            .FirstOrDefaultAsync(l => l.Id == 2)
            ?? throw new InvalidOperationException("Couldn't setup existing entity");

        var dtos = new List<CreateWhoWeAreContentLocalizationDto>
        {
            new()
            {
                EntityId = 99999,
                LanguageId = language.Id,
                Title = "Valid localized title"
            }
        };

        var response = await Fixture.HttpClient.PostAsync(
            $"/api/WhoWeAreContentLocalizations/{(int)SectionType.Main}",
            Serialize(dtos));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateWhoWeAreContentLocalization_ShouldReturnBadRequest_WhenImageContentProvided()
    {
        var imageContent = await Fixture.DbContext.WhoWeAreContents
            .FirstOrDefaultAsync(c => c.ContentType == ContentType.Image)
            ?? throw new InvalidOperationException("Couldn't setup existing entity");

        var language = await Fixture.DbContext.LocalizationLanguages
            .FirstOrDefaultAsync(l => l.Id == 2)
            ?? throw new InvalidOperationException("Couldn't setup existing entity");

        var dtos = new List<CreateWhoWeAreContentLocalizationDto>
        {
            new()
            {
                EntityId = imageContent.Id,
                LanguageId = language.Id
            }
        };

        var response = await Fixture.HttpClient.PostAsync(
            $"/api/WhoWeAreContentLocalizations/{(int)SectionType.Main}",
            Serialize(dtos));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateWhoWeAreContentLocalization_InvalidData_ShouldReturnBadRequest()
    {
        var dtos = new List<CreateWhoWeAreContentLocalizationDto>
        {
            new()
            {
                EntityId = 0, // invalid
                LanguageId = 0, // invalid
                Title = "t"
            }
        };

        var response = await Fixture.HttpClient.PostAsync(
            $"/api/WhoWeAreContentLocalizations/{(int)SectionType.Main}",
            Serialize(dtos));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    private static StringContent Serialize(object obj) =>
        new(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");
}
