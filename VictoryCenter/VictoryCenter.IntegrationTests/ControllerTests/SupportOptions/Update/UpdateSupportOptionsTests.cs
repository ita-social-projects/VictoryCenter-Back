using System.Net;
using System.Text;
using Newtonsoft.Json;
using VictoryCenter.BLL.DTOs.Admin.Donate.SupportOptions;
using VictoryCenter.IntegrationTests.Utils.DbFixture;
using VictoryCenter.IntegrationTests.Utils;

namespace VictoryCenter.IntegrationTests.ControllerTests.SupportOptions.Update;

public class UpdateSupportOptionsTests : BaseTestClass
{
    public UpdateSupportOptionsTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task SupportOptions_ShouldUpdate()
    {
        var updateDto = new UpdateSupportOptionsDto
        {
            Name = "UpdatedName",
            Value = "UpdatedValue"
        };
        var serializedDto = JsonConvert.SerializeObject(updateDto);

        HttpResponseMessage response = await Fixture.HttpClient.PutAsync("/api/SupportOptions/1", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();
        SupportOptionsDto? responseContent = JsonConvert.DeserializeObject<SupportOptionsDto>(responseString);

        Assert.NotNull(responseContent);
        Assert.Equal(updateDto.Name, responseContent.Name);
        Assert.Equal(updateDto.Value, responseContent.Value);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public async Task SupportOptions_ShouldNotUpdate_NotFound(int id)
    {
        var updateDto = new UpdateSupportOptionsDto
        {
            Name = "Name",
            Value = "Value"
        };
        var serializedDto = JsonConvert.SerializeObject(updateDto);

        HttpResponseMessage response = await Fixture.HttpClient.PutAsync($"/api/SupportOptions/{id}", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Theory]
    [InlineData(null, "value")]
    [InlineData("", "value")]
    [InlineData("name", null)]
    [InlineData("name", "")]
    public async Task SupportOptions_ShouldNotUpdate_InvalidData(string? name, string? value)
    {
        var updateDto = new UpdateSupportOptionsDto
        {
            Name = name!,
            Value = value!
        };
        var serializedDto = JsonConvert.SerializeObject(updateDto);

        HttpResponseMessage response = await Fixture.HttpClient.PutAsync("/api/SupportOptions/1", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
