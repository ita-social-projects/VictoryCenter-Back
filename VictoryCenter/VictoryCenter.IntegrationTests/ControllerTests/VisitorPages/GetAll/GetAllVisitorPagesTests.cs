using System.Net;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.VisitorPages.GetAll;

public class GetAllVisitorPagesTests : BaseTestClass
{
    public GetAllVisitorPagesTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task GetAllVisitorPages_ShouldReturnAllVisitorPages()
    {
        var response = await Fixture.HttpClient.GetAsync(new Uri("/api/faq/pages", UriKind.Relative));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
