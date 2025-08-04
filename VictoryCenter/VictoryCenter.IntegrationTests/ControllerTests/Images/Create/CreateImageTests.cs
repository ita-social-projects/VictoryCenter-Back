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
    public async Task CreateImage_ValidData_ShouldCreateImage()
    {
        var createImageDto = new CreateImageDTO
        {
            Base64 = "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR4nGNgYAAAAAMAAWgmWQ0AAAAASUVORK5CYII=",
            MimeType = "image/png"
        };

        var serializedDto = JsonSerializer.Serialize(createImageDto);
        HttpResponseMessage response = await _httpClient.PostAsync(
            "api/Image",
            new StringContent(serializedDto, Encoding.UTF8, "application/json"));

        var responseString = await response.Content.ReadAsStringAsync();
        ImageDTO? responseContext = JsonSerializer.Deserialize<ImageDTO>(responseString, _jsonOptions);

        // Перевіряємо що відповідь успішна
        Assert.True(response.IsSuccessStatusCode);
        Assert.NotNull(responseContext);

        // Перевіряємо поля, які тепер є в ImageDTO
        Assert.NotNull(responseContext.BlobName);
        Assert.NotEmpty(responseContext.BlobName);
        Assert.Equal(createImageDto.MimeType, responseContext.MimeType);
        Assert.NotNull(responseContext.Url);
        Assert.NotEmpty(responseContext.Url);
        Assert.True(responseContext.CreatedAt > DateTime.MinValue);

        // Перевіряємо що файл створено у blob storage
        string extension = GetExtensionFromMimeType(createImageDto.MimeType);
        string filePath = Path.Combine(_blobEnvironment.BlobStorePath, $"{responseContext.BlobName}.{extension}");
        Assert.True(File.Exists(filePath));

        // Перевіряємо що URL правильно сформований
        Assert.Contains(responseContext.BlobName, responseContext.Url);
        Assert.Contains($".{extension}", responseContext.Url);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task CreateImage_InvalidMimeType_ShouldReturnBadRequest(string mimeType)
    {
        var createImageDto = new CreateImageDTO
        {
            Base64 = "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR4nGNgYAAAAAMAAWgmWQ0AAAAASUVORK5CYII=",
            MimeType = mimeType
        };

        var serializedDto = JsonSerializer.Serialize(createImageDto);
        HttpResponseMessage response = await _httpClient.PostAsync(
            "api/Image",
            new StringContent(serializedDto, Encoding.UTF8, "application/json"));

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("invalid_base64")]
    public async Task CreateImage_InvalidBase64_ShouldReturnBadRequest(string base64)
    {
        var createImageDto = new CreateImageDTO
        {
            Base64 = base64,
            MimeType = "image/png"
        };

        var serializedDto = JsonSerializer.Serialize(createImageDto);
        HttpResponseMessage response = await _httpClient.PostAsync(
            "api/Image",
            new StringContent(serializedDto, Encoding.UTF8, "application/json"));

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateImage_ValidDataWithDataPrefix_ShouldCreateImage()
    {
        var createImageDto = new CreateImageDTO
        {
            Base64 = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR4nGNgYAAAAAMAAWgmWQ0AAAAASUVORK5CYII=",
            MimeType = "image/png"
        };

        var serializedDto = JsonSerializer.Serialize(createImageDto);
        HttpResponseMessage response = await _httpClient.PostAsync(
            "api/Image",
            new StringContent(serializedDto, Encoding.UTF8, "application/json"));

        var responseString = await response.Content.ReadAsStringAsync();
        ImageDTO? responseContext = JsonSerializer.Deserialize<ImageDTO>(responseString, _jsonOptions);

        Assert.True(response.IsSuccessStatusCode);
        Assert.NotNull(responseContext);
        Assert.NotNull(responseContext.BlobName);
        Assert.Equal(createImageDto.MimeType, responseContext.MimeType);

        // Перевіряємо що файл створено
        string extension = GetExtensionFromMimeType(createImageDto.MimeType);
        string filePath = Path.Combine(_blobEnvironment.BlobStorePath, $"{responseContext.BlobName}.{extension}");
        Assert.True(File.Exists(filePath));
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
        HttpResponseMessage response = await _httpClient.PostAsync(
            "api/Image",
            new StringContent(serializedDto, Encoding.UTF8, "application/json"));

        var responseString = await response.Content.ReadAsStringAsync();
        ImageDTO? responseContext = JsonSerializer.Deserialize<ImageDTO>(responseString, _jsonOptions);

        Assert.True(response.IsSuccessStatusCode);
        Assert.NotNull(responseContext);
        Assert.Equal(mimeType, responseContext.MimeType);

        // Перевіряємо що файл створено з правильним розширенням
        string expectedExtension = GetExtensionFromMimeType(mimeType);
        string filePath = Path.Combine(_blobEnvironment.BlobStorePath, $"{responseContext.BlobName}.{expectedExtension}");
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
