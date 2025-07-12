using System.Net;
using System.Text;
using System.Text.Json;
using VictoryCenter.BLL.DTOs.Images;
using VictoryCenter.BLL.Services.BlobStorage;
using VictoryCenter.IntegrationTests.ControllerTests.Base;

namespace VictoryCenter.IntegrationTests.ControllerTests.Images.Create;

[Collection("SharedIntegrationTests")]
public class CreateImageTests
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly BlobEnvironmentVariables _blobEnvironment;

    public CreateImageTests(IntegrationTestDbFixture fixture)
    {
        _httpClient = fixture.HttpClient;
        _blobEnvironment = fixture.BlobVariables;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    [Fact]
    public async Task CreateImage_ShouldCreateImage()
    {
        var createImageDto = new CreateImageDTO
        {
            Base64 =
                "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR4nGNgYAAAAAMAAWgmWQ0AAAAASUVORK5CYII=",

            MimeType = "image/jpg"
        };

        var serializedDto = JsonSerializer.Serialize(createImageDto);

        HttpResponseMessage responce = await _httpClient.PostAsync("api/Image", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));

        var responceString = await responce.Content.ReadAsStringAsync();
        ImageDTO? responceContext = JsonSerializer.Deserialize<ImageDTO>(responceString, _jsonOptions);
        string extension = responceContext.MimeType.Split("/")[1];
        string path = _blobEnvironment.BlobStorePath + responceContext.BlobName + "." + extension;

        Assert.True(responce.IsSuccessStatusCode);
        Assert.Equal(createImageDto.Base64, responceContext.Base64);
        Assert.Equal(createImageDto.MimeType, responceContext.MimeType);
        Assert.Equal(createImageDto.Base64, responceContext.Base64);
        Assert.True(File.Exists(path));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task CreateImage_InvalidData_ShouldReturnError(string mimeType)
    {
        var createImageDto = new CreateImageDTO
        {
            Base64 =
                "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR4nGNgYAAAAAMAAWgmWQ0AAAAASUVORK5CYII=",

            MimeType = mimeType
        };

        var serializedDto = JsonSerializer.Serialize(createImageDto);

        HttpResponseMessage responce = await _httpClient.PostAsync("api/Image", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));

        var responceString = await responce.Content.ReadAsStringAsync();
        ImageDTO? responceContext = JsonSerializer.Deserialize<ImageDTO>(responceString, _jsonOptions);

        Assert.False(responce.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, responce.StatusCode);
    }
}
