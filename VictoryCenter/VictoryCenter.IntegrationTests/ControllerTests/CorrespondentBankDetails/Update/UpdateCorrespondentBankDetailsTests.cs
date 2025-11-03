using System.Net;
using System.Text;
using Newtonsoft.Json;
using VictoryCenter.BLL.DTOs.Admin.Donate.CorrespondentBankDetails;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.CorrespondentBankDetails.Update;

public class UpdateCorrespondentBankDetailsTests : BaseTestClass
{
    public UpdateCorrespondentBankDetailsTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task CorrespondentBankDetails_ShouldUpdate()
    {
        var updateDto = new UpdateCorrespondentBankDetailsDto
        {
            Name = "UpdatedCorrespondentBank",
            Swift = "aaaaaaaaaaa",
            Account = "UPDACC123456",
            Iban = "123456789012345678901234567",
            ForeignBankDetailsId = 1
        };

        var serializedDto = JsonConvert.SerializeObject(updateDto);
        HttpResponseMessage response = await Fixture.HttpClient.PutAsync("/api/CorrespondentBankDetails/1", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));

        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();
        CorrespondentBankDetailsDto? responseContent = JsonConvert.DeserializeObject<CorrespondentBankDetailsDto>(responseString);

        Assert.NotNull(responseContent);
        Assert.Equal(updateDto.Name, responseContent.Name);
        Assert.Equal(updateDto.Swift, responseContent.Swift);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public async Task CorrespondentBankDetails_ShouldNotUpdate_NotFound(int id)
    {
        var updateDto = new UpdateCorrespondentBankDetailsDto
        {
            Name = "SomeBank",
            Swift = "aaaaaaaaaaa",
            Account = "SOMEACC12345",
            Iban = "123456789012345678901234567",
            ForeignBankDetailsId = 1
        };

        var serializedDto = JsonConvert.SerializeObject(updateDto);
        HttpResponseMessage response = await Fixture.HttpClient.PutAsync($"/api/CorrespondentBankDetails/{id}", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task CorrespondentBankDetails_ShouldNotUpdate_InvalidName(string? name)
    {
        var updateDto = new UpdateCorrespondentBankDetailsDto
        {
            Name = name!,
            Swift = "dsdssss",
            Account = "BADACCOUNT",
            Iban = "122211",
            ForeignBankDetailsId = 1
        };

        var serializedDto = JsonConvert.SerializeObject(updateDto);
        HttpResponseMessage response = await Fixture.HttpClient.PutAsync("/api/CorrespondentBankDetails/1", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
