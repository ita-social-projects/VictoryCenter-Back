using System.Net;
using System.Text;
using Newtonsoft.Json;
using VictoryCenter.BLL.DTOs.Admin.Donate.ForeignBankDetails;
using VictoryCenter.DAL.Enums;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.ForeignBankDetails.Create;

public class CreateForeignBankDetailsTests : BaseTestClass
{
    public CreateForeignBankDetailsTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task ForeignBankDetails_ShouldCreate()
    {
        var createDto = new CreateForeignBankDetailsDto
        {
            Name = "NewForeignBank",
            Receiver = "Charity Org",
            Iban = "123456789012345678901234567",
            Swift = "12345678901",
            Address = "New York, USA",
            Currency = BankCurrency.Usd,
        };
        var serializedDto = JsonConvert.SerializeObject(createDto);

        HttpResponseMessage response = await Fixture.HttpClient.PostAsync("/api/ForeignBankDetails/", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();
        ForeignBankDetailsDto? responseContent = JsonConvert.DeserializeObject<ForeignBankDetailsDto>(responseString);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(responseContent);
        Assert.Equal(createDto.Name, responseContent.Name);
        Assert.Equal(createDto.Iban, responseContent.Iban);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task ForeignBankDetails_ShouldNotCreate_InvalidName(string? name)
    {
        var createDto = new CreateForeignBankDetailsDto
        {
            Name = name!,
            Receiver = "Charity Org",
            Iban = "UA000000000000000000000000000",
            Swift = "BADX",
            Address = "Invalid",
        };
        var serializedDto = JsonConvert.SerializeObject(createDto);

        HttpResponseMessage response = await Fixture.HttpClient.PostAsync("/api/ForeignBankDetails/", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
