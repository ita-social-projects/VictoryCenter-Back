using System.Net;
using System.Text;
using Newtonsoft.Json;
using VictoryCenter.BLL.DTOs.Admin.Donate.UahBankDetails;
using VictoryCenter.IntegrationTests.Utils.DbFixture;
using VictoryCenter.IntegrationTests.Utils;

namespace VictoryCenter.IntegrationTests.ControllerTests.UahBankDetails.Create;
public class CreateUahBankDetailsTests : BaseTestClass
{
    public CreateUahBankDetailsTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task UahBankDetails_ShouldCreateUahBankDetails()
    {
        var createDto = new CreateUahBankDetailsDto
        {
            Name = "PrivatBank",
            Receiver = "PrivatBank",
            Edrpou = "11111111",
            Iban = "UA123456789012345678901234567",
            PaymentPurpose = "Donation"
        };
        var serializedDto = JsonConvert.SerializeObject(createDto);

        HttpResponseMessage response = await Fixture.HttpClient.PostAsync("/api/UahBankDetails/", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();
        UahBankDetailsDto? responseContent = JsonConvert.DeserializeObject<UahBankDetailsDto>(responseString);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(responseContent);
        Assert.Equal(createDto.Name, responseContent.Name);
        Assert.Equal(createDto.Receiver, responseContent.Receiver);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task UahBankDetails_ShouldNotCreateUahBankDetails_InvalidBankName(string? bankName)
    {
        var createDto = new CreateUahBankDetailsDto
        {
            Name = bankName!,
            Receiver = "PrivatBank",
            Edrpou = "111111112",
            Iban = "1234567890123456789012345672",
            PaymentPurpose = "Donation"
        };
        var serializedDto = JsonConvert.SerializeObject(createDto);

        HttpResponseMessage response = await Fixture.HttpClient.PostAsync("/api/UahBankDetails/", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
