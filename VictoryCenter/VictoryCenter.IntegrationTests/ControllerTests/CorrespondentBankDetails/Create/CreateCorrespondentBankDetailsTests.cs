using System.Net;
using System.Text;
using Newtonsoft.Json;
using VictoryCenter.BLL.DTOs.Admin.Donate.CorrespondentBankDetails;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.CorrespondentBankDetails.Create;

public class CreateCorrespondentBankDetailsTests : BaseTestClass
{
    public CreateCorrespondentBankDetailsTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task CorrespondentBankDetails_ShouldCreate()
    {
        var createDto = new CreateCorrespondentBankDetailsDto
        {
            Name = "NewCorrespondentBank",
            Swift = "aaaaaaaaaaa",
            Account = "ACC9999999999",
            ForeignIban = "123456789012345678901234567",
            ForeignBankDetailsId = 1
        };

        var serializedDto = JsonConvert.SerializeObject(createDto);
        HttpResponseMessage response = await Fixture.HttpClient.PostAsync("/api/CorrespondentBankDetails/", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));

        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();
        CorrespondentBankDetailsDto? responseContent = JsonConvert.DeserializeObject<CorrespondentBankDetailsDto>(responseString);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(responseContent);
        Assert.Equal(createDto.Name, responseContent.Name);
        Assert.Equal(createDto.Swift, responseContent.Swift);
    }
}
