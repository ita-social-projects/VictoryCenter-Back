using Newtonsoft.Json;
using VictoryCenter.BLL.DTOs.Public.Donate.ForeignBankDetails;
using VictoryCenter.IntegrationTests.Utils.DbFixture;
using VictoryCenter.IntegrationTests.Utils;

namespace VictoryCenter.IntegrationTests.ControllerTests.ForeignBankDetails.GetPublished;
public class GetPublishedForeignBankDetailsTests : BaseTestClass
{
    public GetPublishedForeignBankDetailsTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task ForeignBankDetails_ShouldReturnAll()
    {
        HttpResponseMessage response = await Fixture.HttpClient.GetAsync("/api/ForeignBankDetails/published/");
        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();
        IEnumerable<ForeignBankDetailsDto>? responseContent =
            JsonConvert.DeserializeObject<IEnumerable<ForeignBankDetailsDto>>(responseString);

        Assert.NotNull(responseContent);
        Assert.NotEmpty(responseContent);
    }
}
