using System.Net;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Admin.Partners;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.Partners.GetBanner;

public class GetPartnersBannerTests : BaseTestClass
{
    private readonly Uri _endpointUri = new("/api/Partners/banner", UriKind.Relative);

    public GetPartnersBannerTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task GetPartnersBanner_ShouldReturnSeededBanner()
    {
        // Arrange
        var expectedBanner = await Fixture.DbContext.PartnersPageBanners.AsNoTracking().FirstOrDefaultAsync()
            ?? throw new InvalidOperationException("Banner was not seeded correctly.");

        // Act
        var response = await Fixture.HttpClient.GetAsync(_endpointUri);
        var responseString = await response.Content.ReadAsStringAsync();
        var responseContent = JsonSerializer.Deserialize<PartnersPageBannerDto>(responseString, JsonOptions);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(responseContent);
        Assert.Equal(expectedBanner.Title, responseContent.Title);
        Assert.Equal(expectedBanner.Description, responseContent.Description);
        Assert.NotNull(responseContent.Image);
        Assert.Equal(expectedBanner.ImageId, responseContent.Image.Id);
    }
}
