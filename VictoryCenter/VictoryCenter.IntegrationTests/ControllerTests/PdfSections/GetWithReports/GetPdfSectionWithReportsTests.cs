using System.Net;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Admin.PdfSection;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.PdfSections.GetWithReports;

public class GetPdfSectionWithReportsTests : BaseTestClass
{
    public GetPdfSectionWithReportsTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task GetPdfSectionWithReports_SectionExists_ShouldReturnSectionWithReports()
    {
        // Arrange
        var expectedSection = await Fixture.DbContext.PdfSections.FirstAsync();

        // Act
        var response = await Fixture.HttpClient.GetAsync("/api/PdfSection/pdf-section");
        var responseString = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<PdfSectionWithReportsDto>(responseString, JsonOptions);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(result);
        Assert.Equal(expectedSection.Title, result!.Title);
        Assert.Equal(expectedSection.Description, result.Description);
    }

    [Fact]
    public async Task GetPdfSectionWithReports_NoSection_ShouldReturnNotFound()
    {
        // Arrange
        var sections = await Fixture.DbContext.PdfSections.ToListAsync();
        Fixture.DbContext.PdfSections.RemoveRange(sections);
        await Fixture.DbContext.SaveChangesAsync();

        // Act
        var response = await Fixture.HttpClient.GetAsync("/api/PdfSection/pdf-section");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
