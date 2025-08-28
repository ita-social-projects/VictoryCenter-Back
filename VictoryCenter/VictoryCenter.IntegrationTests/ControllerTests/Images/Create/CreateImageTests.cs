using System.Net;
using System.Text;
using System.Text.Json;
using VictoryCenter.BLL.DTOs.Images;

using VictoryCenter.IntegrationTests.ControllerTests.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.Images.Create;

[Collection("SharedIntegrationTests")]
public class CreateImageTests : IAsyncLifetime
{
    private readonly IntegrationTestDbFixture _fixture;
    private readonly JsonSerializerOptions _jsonOptions;

    public CreateImageTests(IntegrationTestDbFixture fixture)
    {
        _fixture = fixture;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task InitializeAsync()
    {
        await _fixture.CreateFreshWebApplication();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task CreateImage_ValidData_ShouldCreateImage()
    {
        var createImageDto = new CreateImageDTO
        {
            Base64 =
                "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR4nGNgYAAAAAMAAWgmWQ0AAAAASUVORK5CYII=",

            MimeType = "image/jpg"
        };

        var serializedDto = JsonSerializer.Serialize(createImageDto);

        HttpResponseMessage response = await _fixture.HttpClient.PostAsync("api/Image", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));

        var responseString = await response.Content.ReadAsStringAsync();
        ImageDTO? responseContext = JsonSerializer.Deserialize<ImageDTO>(responseString, _jsonOptions);
        string extension = responseContext.MimeType.Split("/")[1];
        string path = Path.Combine(_fixture.BlobEnvironmentVariables.BlobStorePath, responseContext.BlobName + "." + extension);

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
    public async Task CreateImage_InvalidData_ShouldReturnError(string mimeType)
    {
        var createImageDto = new CreateImageDTO
        {
            Base64 =
                "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR4nGNgYAAAAAMAAWgmWQ0AAAAASUVORK5CYII=",

            MimeType = mimeType
        };

        var serializedDto = JsonSerializer.Serialize(createImageDto);

        HttpResponseMessage response = await _fixture.HttpClient.PostAsync("api/Image", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));

        var responseString = await response.Content.ReadAsStringAsync();
        ImageDTO? responseContext = JsonSerializer.Deserialize<ImageDTO>(responseString, _jsonOptions);

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
