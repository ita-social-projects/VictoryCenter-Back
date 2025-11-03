using Newtonsoft.Json;
using VictoryCenter.BLL.DTOs.Public.Donate.UahBankDetails;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.UahBankDetails.GetPublished;

public class GetPublishedUahBankDetailsTests : BaseTestClass
{
    public GetPublishedUahBankDetailsTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task UahBankDetails_ShouldReturnAll()
    {
        HttpResponseMessage response = await Fixture.HttpClient.GetAsync("/api/UahBankDetails/published/");
        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();
        IEnumerable<PublishedUahBankDetailsDto>? responseContent =
            JsonConvert.DeserializeObject<IEnumerable<PublishedUahBankDetailsDto>>(responseString);

        Assert.NotNull(responseContent);
        Assert.NotEmpty(responseContent);
    }
}
