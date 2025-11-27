using System.Net;
using System.Text;
using Newtonsoft.Json;
using VictoryCenter.BLL.DTOs.Admin.Donate.UahBankDetails;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.UahBankDetails.Update;

public class UpdateUahBankDetailsTests : BaseTestClass
{
    public UpdateUahBankDetailsTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task UahBankDetails_ShouldUpdate()
    {
        var updateDto = new UpdateUahBankDetailsDto
        {
            Name = "PrivatBank",
            Receiver = "PrivatBank",
            Edrpou = "11111111",
            UkrainianIban = "UA123456789012345678901234567",
            PaymentPurpose = "Donation"
        };
        var serializedDto = JsonConvert.SerializeObject(updateDto);

        HttpResponseMessage response = await Fixture.HttpClient.PutAsync("/api/UahBankDetails/1", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();
        UahBankDetailsDto? responseContent = JsonConvert.DeserializeObject<UahBankDetailsDto>(responseString);

        Assert.NotNull(responseContent);
        Assert.Equal(updateDto.Name, responseContent.Name);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public async Task UahBankDetails_ShouldNotUpdate_NotFound(int id)
    {
        var updateDto = new UpdateUahBankDetailsDto
        {
            Name = "PrivatBank",
            Receiver = "PrivatBank",
            Edrpou = "11111111",
            UkrainianIban = "UA123456789012345678901234567",
            PaymentPurpose = "Donation"
        };
        var serializedDto = JsonConvert.SerializeObject(updateDto);

        HttpResponseMessage response = await Fixture.HttpClient.PutAsync($"/api/UahBankDetails/{id}", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
