using System.Text.Json;
using VictoryCenter.BLL.DTOs.Admin.VisitorPages;
using VictoryCenter.IntegrationTests.ControllerTests.DbFixture;
using VictoryCenter.IntegrationTests.Utils;

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
        var responseString = await response.Content.ReadAsStringAsync();
        var responseContent = JsonSerializer.Deserialize<List<VisitorPageDto>>(
            responseString,
            JsonOptions);

        response.EnsureSuccessStatusCode();
        Assert.NotNull(responseContent);
        Assert.IsType<List<VisitorPageDto>>(responseContent);
        Assert.NotEmpty(responseContent);
    }
}
