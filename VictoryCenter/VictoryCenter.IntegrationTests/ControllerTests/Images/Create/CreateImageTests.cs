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
        string path = Path.Combine(_fixture.BlobEnvironmentVariables.FullPath, responseContext.BlobName + "." + extension);

        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal(createImageDto.MimeType, responseContext.MimeType);
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

    [Theory]
    [InlineData("image/jpeg")]
    [InlineData("image/jpg")]
    [InlineData("image/png")]
    [InlineData("image/webp")]
    public async Task CreateImage_DifferentMimeTypes_ShouldCreateImageWithCorrectExtension(string mimeType)
    {
        var createImageDto = new CreateImageDTO
        {
            Base64 = "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR4nGNgYAAAAAMAAWgmWQ0AAAAASUVORK5CYII=",
            MimeType = mimeType
        };

        var serializedDto = JsonSerializer.Serialize(createImageDto);
        HttpResponseMessage response = await _fixture.HttpClient.PostAsync(
            "api/Image",
            new StringContent(serializedDto, Encoding.UTF8, "application/json"));

        var responseString = await response.Content.ReadAsStringAsync();
        ImageDTO? responseContext = JsonSerializer.Deserialize<ImageDTO>(responseString, _jsonOptions);

        Assert.True(response.IsSuccessStatusCode);
        Assert.NotNull(responseContext);
        Assert.Equal(mimeType, responseContext.MimeType);

        // Перевіряємо що файл створено з правильним розширенням
        string expectedExtension = GetExtensionFromMimeType(mimeType);
        string filePath = Path.Combine(_fixture.BlobEnvironmentVariables.FullPath, $"{responseContext.BlobName}.{expectedExtension}");
        Assert.True(File.Exists(filePath));

        // Перевіряємо що URL містить правильне розширення
        Assert.Contains($".{expectedExtension}", responseContext.Url);
    }

    private static string GetExtensionFromMimeType(string mimeType)
    {
        return mimeType.ToLower() switch
        {
            "image/jpeg" => "jpg",
            "image/jpg" => "jpg",
            "image/png" => "png",
            "image/webp" => "webp",
            _ => "jpg"
        };
    }
}
