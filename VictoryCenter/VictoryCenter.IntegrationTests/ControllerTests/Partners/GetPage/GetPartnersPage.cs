using System.Net;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Public.Partners;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.Partners.GetPage;

public class GetPublicPartnersPageTests : BaseTestClass
{
    private readonly Uri _endpointUri = new("/api/Partners/page", UriKind.Relative);

    public GetPublicPartnersPageTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task GetPartnersPage_ShouldReturnFullPageDto()
    {
        // Arrange
        var expectedSectionsCount = await Fixture.DbContext.PartnersSections.CountAsync();
        var expectedBannerTitle = (await Fixture.DbContext.PartnersPageBanners.FirstAsync()).Title;

        // Act
        var response = await Fixture.HttpClient.GetAsync(_endpointUri);
        var responseString = await response.Content.ReadAsStringAsync();
        var responseContent = JsonSerializer.Deserialize<PartnersPageDto>(responseString, JsonOptions);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(responseContent);

        Assert.NotNull(responseContent.Banner);
        Assert.Equal(expectedBannerTitle, responseContent.Banner.Title);

        Assert.NotNull(responseContent.Sections);
        Assert.Equal(expectedSectionsCount, responseContent.Sections.Count());
    }
}
