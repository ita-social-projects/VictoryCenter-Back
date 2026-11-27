using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Admin.Localization.PdfSection;
using VictoryCenter.BLL.DTOs.Admin.PdfSection;
using VictoryCenter.BLL.DTOs.Public.PdfSection;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.PdfSection.Public;

public class GetPublicPdfSectionTests : BaseTestClass
{
    private const string PublicEndpoint = "/api/PdfSection/public";

    public GetPublicPdfSectionTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task GetPublicPdfSection_AnonymousRequest_ShouldReturnSeededSection()
    {
        // Arrange
        var expectedSection = await Fixture.DbContext.PdfSections.AsNoTracking().FirstAsync();
        using var anonymousClient = Fixture.Factory.CreateClient();

        // Act
        var response = await anonymousClient.GetAsync(PublicEndpoint);
        var result = await response.Content.ReadFromJsonAsync<PublicPdfSectionDto>(JsonOptions);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(result);
        Assert.Equal(expectedSection.Title, result!.Title);
        Assert.Equal(expectedSection.Description, result.Description);
    }

    [Fact]
    public async Task GetPublicPdfSection_AfterAdminUpdateAndLocalization_ShouldReturnUpdatedContentWithoutTranslationStatus()
    {
        // Arrange
        var englishLanguage = await Fixture.DbContext.LocalizationLanguages
            .AsNoTracking()
            .FirstAsync(l => l.Code == "en");

        var updateDto = new PdfSectionDto
        {
            Title = "Оновлений заголовок",
            Description = "Оновлений короткий опис для сторінки звітності"
        };
        var updateResponse = await Fixture.HttpClient.PutAsJsonAsync("/api/PdfSection", updateDto);
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        var localizationDto = new CreatePdfSectionLocalizationDto
        {
            LanguageId = englishLanguage.Id,
            Title = "Updated English Title",
            Description = "Updated English Description"
        };
        var locResponse = await Fixture.HttpClient.PostAsJsonAsync("/api/PdfSectionLocalizations", localizationDto);
        Assert.Equal(HttpStatusCode.OK, locResponse.StatusCode);

        using var anonymousClient = Fixture.Factory.CreateClient();

        // Act
        var response = await anonymousClient.GetAsync(PublicEndpoint);
        var rawJson = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<PublicPdfSectionDto>(rawJson, JsonOptions);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(result);
        Assert.Equal(updateDto.Title, result!.Title);
        Assert.Equal(updateDto.Description, result.Description);
        var enLoc = Assert.Single(result.Localizations, l => l.Language.Code == "en");
        Assert.Equal(localizationDto.Title, enLoc.Title);
        Assert.Equal(localizationDto.Description, enLoc.Description);
        Assert.DoesNotContain("translationStatus", rawJson, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task GetPublicPdfSection_NoSection_ShouldReturnNotFound()
    {
        // Arrange
        var sections = await Fixture.DbContext.PdfSections.ToListAsync();
        Fixture.DbContext.PdfSections.RemoveRange(sections);
        await Fixture.DbContext.SaveChangesAsync();
        using var anonymousClient = Fixture.Factory.CreateClient();

        // Act
        var response = await anonymousClient.GetAsync(PublicEndpoint);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetPublicPdfSection_SwaggerDocument_ShouldIncludePublicEndpoint()
    {
        // Act
        var response = await Fixture.HttpClient.GetAsync("/swagger/v1/swagger.json");
        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var paths = document.RootElement.GetProperty("paths");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(paths.TryGetProperty(PublicEndpoint, out var publicPath));
        Assert.True(publicPath.TryGetProperty("get", out _));
    }
}
