using Newtonsoft.Json;
using VictoryCenter.BLL.DTOs.Admin.Donate.UahBankDetails;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.UahBankDetails.GetAll;

public class GetAllUahBankDetailsTests : BaseTestClass
{
    public GetAllUahBankDetailsTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task UahBankDetails_ShouldReturnAll()
    {
        HttpResponseMessage response = await Fixture.HttpClient.GetAsync("/api/UahBankDetails/");
        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();
        IEnumerable<UahBankDetailsDto>? responseContent =
            JsonConvert.DeserializeObject<IEnumerable<UahBankDetailsDto>>(responseString);

        Assert.NotNull(responseContent);
        Assert.NotEmpty(responseContent);
    }
}
