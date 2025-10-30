using System.Net;
using System.Text;
using Newtonsoft.Json;
using VictoryCenter.BLL.DTOs.Admin.Donate.CorrespondentBankDetails;
using VictoryCenter.IntegrationTests.Utils.DbFixture;
using VictoryCenter.IntegrationTests.Utils;

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
            Iban = "123456789012345678901234567",
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

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task CorrespondentBankDetails_ShouldNotCreate_InvalidName(string? name)
    {
        var createDto = new CreateCorrespondentBankDetailsDto
        {
            Name = name!,
            Swift = "aaaaa",
            Account = "BADACCOUNT",
            Iban = "1223212122",
            ForeignBankDetailsId = 1
        };

        var serializedDto = JsonConvert.SerializeObject(createDto);
        HttpResponseMessage response = await Fixture.HttpClient.PostAsync("/api/CorrespondentBankDetails/", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
