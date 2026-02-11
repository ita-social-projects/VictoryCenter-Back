using System.Net;
using System.Text.Json;
using VictoryCenter.BLL.DTOs.Admin.ReportMediaSettings;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.Report.GetReportMediaSettings;
public class GetReportMediaSettingsTests : BaseTestClass
{
    private readonly Uri _endpointUri = new("/api/Report/report", UriKind.Relative);

    public GetReportMediaSettingsTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task GetReportMediaSettings_ShouldReturnReportSettings()
    {
        // Act
        var response = await Fixture.HttpClient.GetAsync(_endpointUri);
        var responseString = await response.Content.ReadAsStringAsync();
        var responseContent = JsonSerializer.Deserialize<ReportMediaSettingsDto>(responseString, JsonOptions);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(responseContent);
    }
}
