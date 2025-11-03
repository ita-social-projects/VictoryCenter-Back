using Newtonsoft.Json;
using VictoryCenter.BLL.DTOs.Admin.Donate.ForeignBankDetails;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.ForeignBankDetails.GetAll;

public class GetAllForeignBankDetailsTests : BaseTestClass
{
    public GetAllForeignBankDetailsTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task ForeignBankDetails_ShouldReturnAll()
    {
        HttpResponseMessage response = await Fixture.HttpClient.GetAsync("/api/ForeignBankDetails/");
        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();
        IEnumerable<ForeignBankDetailsDto>? responseContent =
            JsonConvert.DeserializeObject<IEnumerable<ForeignBankDetailsDto>>(responseString);

        Assert.NotNull(responseContent);
        Assert.NotEmpty(responseContent);
    }
}
