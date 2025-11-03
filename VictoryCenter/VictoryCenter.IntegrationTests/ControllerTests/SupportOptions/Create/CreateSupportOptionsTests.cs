using System.Net;
using System.Text;
using Newtonsoft.Json;
using VictoryCenter.BLL.DTOs.Admin.Donate.SupportOptions;
using VictoryCenter.IntegrationTests.Utils.DbFixture;
using VictoryCenter.IntegrationTests.Utils;

namespace VictoryCenter.IntegrationTests.ControllerTests.SupportOptions.Create;

public class CreateSupportOptionsTests : BaseTestClass
{
    public CreateSupportOptionsTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task SupportOptions_ShouldCreate()
    {
        var createDto = new CreateSupportOptionsDto
        {
            Name = "Telegram",
            Value = "@victory_support"
        };
        var serializedDto = JsonConvert.SerializeObject(createDto);

        HttpResponseMessage response = await Fixture.HttpClient.PostAsync("/api/SupportOptions/", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();
        SupportOptionsDto? responseContent = JsonConvert.DeserializeObject<SupportOptionsDto>(responseString);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(responseContent);
        Assert.Equal(createDto.Name, responseContent.Name);
        Assert.Equal(createDto.Value, responseContent.Value);
    }

    [Theory]
    [InlineData(null, "value")]
    [InlineData("", "value")]
    [InlineData("name", null)]
    [InlineData("name", "")]
    public async Task SupportOptions_ShouldNotCreate_InvalidData(string? name, string? value)
    {
        var createDto = new CreateSupportOptionsDto
        {
            Name = name!,
            Value = value!
        };
        var serializedDto = JsonConvert.SerializeObject(createDto);

        HttpResponseMessage response = await Fixture.HttpClient.PostAsync("/api/SupportOptions/", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
