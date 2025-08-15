using System.Net;
using System.Text;
using System.Text.Json;
using VictoryCenter.BLL.DTOs.Admin.Images;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.Images.Create;

public class CreateImageTests : BaseTestClass
{
    public CreateImageTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task CreateImage_ValidData_ShouldCreateImage()
    {
        var createImageDto = new CreateImageDto
        {
            Base64 =
                "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR4nGNgYAAAAAMAAWgmWQ0AAAAASUVORK5CYII=",

            MimeType = "image/jpg"
        };

        var serializedDto = JsonSerializer.Serialize(createImageDto);

        HttpResponseMessage response = await Fixture.HttpClient.PostAsync("api/Image", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));

        var responseString = await response.Content.ReadAsStringAsync();
        ImageDto? responseContext = JsonSerializer.Deserialize<ImageDto>(responseString, JsonOptions);
        string extension = responseContext!.MimeType.Split("/")[1];
        string path = Path.Combine(Fixture.BlobEnvironmentVariables.BlobStorePath, responseContext.BlobName + "." + extension);

        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal(createImageDto.Base64, responseContext.Base64);
        Assert.Equal(createImageDto.MimeType, responseContext.MimeType);
        Assert.Equal(createImageDto.Base64, responseContext.Base64);
        Assert.True(File.Exists(path));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task CreateImage_InvalidData_ShouldReturnError(string? mimeType)
    {
        var createImageDto = new CreateImageDto
        {
            Base64 =
                "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR4nGNgYAAAAAMAAWgmWQ0AAAAASUVORK5CYII=",

            MimeType = mimeType!
        };

        var serializedDto = JsonSerializer.Serialize(createImageDto);

        HttpResponseMessage response = await Fixture.HttpClient.PostAsync("api/Image", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
