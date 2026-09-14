using System.Net;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyLandingPage;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.HippotherapyLandingPage.Get;

public class GetPublicHippotherapyLandingPageTests : BaseTestClass
{
    private readonly Uri _endpointUri = new("/api/HippotherapyPage/public", UriKind.Relative);

    public GetPublicHippotherapyLandingPageTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task Get_WithoutAuthorization_ShouldReturnOkWithAllSections()
    {
        using var anonymousClient = Fixture.Factory.CreateClient();

        var response = await anonymousClient.GetAsync(_endpointUri);
        var responseString = await response.Content.ReadAsStringAsync();
        var responseContent = JsonSerializer.Deserialize<HippotherapyLandingPageDto>(responseString, JsonOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(responseContent);
        Assert.False(string.IsNullOrWhiteSpace(responseContent.IntroSection.Title));
        Assert.False(string.IsNullOrWhiteSpace(responseContent.HippoventionCenterSection.Pros));
        Assert.Equal(4, responseContent.AdvantagesSection.Cards.Count);
        Assert.Equal(4, responseContent.ParticipantsSection.Cards.Count);
        Assert.Equal(4, responseContent.EthicsSection.Principles.Count);
        Assert.NotEmpty(responseContent.ScientificReferencesSection.ScientificReferences);
    }

    [Fact]
    public async Task Get_NoRowInDatabase_ShouldReturnNotFound()
    {
        var seededRows = await Fixture.DbContext.HippotherapyLandingPages.ToListAsync();
        Fixture.DbContext.HippotherapyLandingPages.RemoveRange(seededRows);
        await Fixture.DbContext.SaveChangesAsync();
        Fixture.DbContext.ChangeTracker.Clear();

        using var anonymousClient = Fixture.Factory.CreateClient();

        var response = await anonymousClient.GetAsync(_endpointUri);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Get_AdminEndpointWithoutAuthorization_ShouldStillReturnUnauthorized()
    {
        using var anonymousClient = Fixture.Factory.CreateClient();

        var response = await anonymousClient.GetAsync(new Uri("/api/HippotherapyPage", UriKind.Relative));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
