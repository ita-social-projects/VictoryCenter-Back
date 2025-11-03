using System.Net;
using System.Text;
using Newtonsoft.Json;
using VictoryCenter.BLL.DTOs.Admin.Donate.ForeignBankDetails;
using VictoryCenter.IntegrationTests.Utils.DbFixture;
using VictoryCenter.IntegrationTests.Utils;

namespace VictoryCenter.IntegrationTests.ControllerTests.ForeignBankDetails.Update;

public class UpdateForeignBankDetailsTests : BaseTestClass
{
    public UpdateForeignBankDetailsTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task ForeignBankDetails_ShouldUpdate()
    {
        var updateDto = new UpdateForeignBankDetailsDto
        {
            Name = "UpdatedBank",
            Receiver = "Updated Receiver",
            Iban = "123456789012345678901234567",
            Swift = "12345678901",
            Address = "London, UK"
        };
        var serializedDto = JsonConvert.SerializeObject(updateDto);

        HttpResponseMessage response = await Fixture.HttpClient.PutAsync("/api/ForeignBankDetails/1", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();
        ForeignBankDetailsDto? responseContent = JsonConvert.DeserializeObject<ForeignBankDetailsDto>(responseString);

        Assert.NotNull(responseContent);
        Assert.Equal(updateDto.Name, responseContent.Name);
        Assert.Equal(updateDto.Swift, responseContent.Swift);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public async Task ForeignBankDetails_ShouldNotUpdate_NotFound(int id)
    {
        var updateDto = new UpdateForeignBankDetailsDto
        {
            Name = "SomeBank",
            Receiver = "Some Receiver",
            Iban = "123456789012345678901234567",
            Swift = "12345678901",
            Address = "Berlin"
        };
        var serializedDto = JsonConvert.SerializeObject(updateDto);

        HttpResponseMessage response = await Fixture.HttpClient.PutAsync($"/api/ForeignBankDetails/{id}", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task ForeignBankDetails_ShouldNotUpdate_InvalidName(string? name)
    {
        var updateDto = new UpdateForeignBankDetailsDto
        {
            Name = name!,
            Receiver = "Receiver",
            Iban = "UA1111000011110000111100001111",
            Swift = "BADX",
            Address = "Paris"
        };
        var serializedDto = JsonConvert.SerializeObject(updateDto);

        HttpResponseMessage response = await Fixture.HttpClient.PutAsync("/api/ForeignBankDetails/1", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
