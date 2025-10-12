using System.Net;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Admin.Partners;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.Partners.GetSections;

public class GetPartnersSectionsTests : BaseTestClass
{
    private readonly Uri _endpointUri = new("/api/Partners", UriKind.Relative);

    public GetPartnersSectionsTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task GetPartnersSections_ShouldReturnAllSeededSections()
    {
        // Arrange
        var expectedCount = await Fixture.DbContext.PartnersSections.CountAsync();

        // Act
        var response = await Fixture.HttpClient.GetAsync(_endpointUri);
        var responseString = await response.Content.ReadAsStringAsync();
        var responseContent = JsonSerializer.Deserialize<List<PartnersSectionDto>>(responseString, JsonOptions);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(responseContent);
        Assert.Equal(expectedCount, responseContent.Count);
    }
}
