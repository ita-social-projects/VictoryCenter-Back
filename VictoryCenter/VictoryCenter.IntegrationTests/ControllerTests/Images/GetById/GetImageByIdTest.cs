using System.Net;
using System.Text.Json;
using VictoryCenter.BLL.DTOs.Images;
using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using VictoryCenter.IntegrationTests.ControllerTests.Base;

namespace VictoryCenter.IntegrationTests.ControllerTests.Images.GetById;

[Collection("SharedIntegrationTests")]
public class GetImageByIdTest
{
    private readonly HttpClient _client;
    private readonly VictoryCenterDbContext _dbContext;
    private readonly JsonSerializerOptions _jsonOptions;

    public GetImageByIdTest(IntegrationTestDbFixture fixture)
    {
        _client = fixture.HttpClient;
        _dbContext = fixture.DbContext;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    [Fact]
    public async Task GetImageById_ValidData_ShouldReturnImage()
    {
        // Arrange: Створюємо тестове зображення
        var testImage = await CreateTestImageAsync();
        var imageId = testImage.Id;

        // Act: Отримуємо зображення за ID
        HttpResponseMessage response = await _client.GetAsync($"api/Image/{imageId}");
        var responseString = await response.Content.ReadAsStringAsync();
        ImageDTO? result = JsonSerializer.Deserialize<ImageDTO>(responseString, _jsonOptions);

        // Assert: Перевіряємо результат
        Assert.True(response.IsSuccessStatusCode);
        Assert.NotNull(result);

        // Перевіряємо всі поля ImageDTO
        Assert.Equal(testImage.Id, result.Id);
        Assert.Equal(testImage.BlobName, result.BlobName);
        Assert.Equal(testImage.MimeType, result.MimeType);
        Assert.Equal(testImage.Url, result.Url);
        Assert.Equal(testImage.CreatedAt, result.CreatedAt);
    }

    [Fact]
    public async Task GetImageById_InvalidData_ShouldReturnNotFound()
    {
        // Arrange: Використовуємо неіснуючий ID
        var nonExistentId = long.MaxValue;

        // Act: Намагаємося отримати неіснуюче зображення
        HttpResponseMessage response = await _client.GetAsync($"api/Image/{nonExistentId}");

        // Assert: Перевіряємо що повертається 404
        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public async Task GetImageById_InvalidIdValues_ShouldReturnNotFound(long invalidId)
    {
        // Act: Намагаємося отримати зображення з некоректним ID
        HttpResponseMessage response = await _client.GetAsync($"api/Image/{invalidId}");

        // Assert: Перевіряємо що повертається 404
        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetImageById_ImageWithEmptyBlobName_ShouldReturnError()
    {
        // Arrange: Створюємо зображення з порожнім BlobName
        var imageWithEmptyBlobName = new Image
        {
            BlobName = "", // Порожнє значення
            MimeType = "image/png",
            Url = "http://test.com/empty.png",
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.Images.Add(imageWithEmptyBlobName);
        await _dbContext.SaveChangesAsync();

        // Act: Намагаємося отримати зображення
        HttpResponseMessage response = await _client.GetAsync($"api/Image/{imageWithEmptyBlobName.Id}");

        // Assert: Перевіряємо що повертається помилка
        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetImageById_DifferentMimeTypes_ShouldReturnCorrectData()
    {
        // Arrange: Створюємо зображення з різними MIME типами
        var testCases = new[]
        {
            ("image/jpeg", "test-jpeg"),
            ("image/png", "test-png"),
            ("image/webp", "test-webp")
        };

        foreach (var (mimeType, blobName) in testCases)
        {
            var testImage = await CreateTestImageAsync(blobName, mimeType);

            // Act: Отримуємо зображення
            HttpResponseMessage response = await _client.GetAsync($"api/Image/{testImage.Id}");
            var responseString = await response.Content.ReadAsStringAsync();
            ImageDTO? result = JsonSerializer.Deserialize<ImageDTO>(responseString, _jsonOptions);

            // Assert: Перевіряємо результат
            Assert.True(response.IsSuccessStatusCode);
            Assert.NotNull(result);
            Assert.Equal(mimeType, result.MimeType);
            Assert.Equal(blobName, result.BlobName);
        }
    }

    [Fact]
    public async Task GetImageById_ResponseFormat_ShouldContainAllRequiredFields()
    {
        // Arrange: Створюємо тестове зображення
        var testImage = await CreateTestImageAsync();

        // Act: Отримуємо зображення
        HttpResponseMessage response = await _client.GetAsync($"api/Image/{testImage.Id}");
        var responseString = await response.Content.ReadAsStringAsync();
        ImageDTO? result = JsonSerializer.Deserialize<ImageDTO>(responseString, _jsonOptions);

        // Assert: Перевіряємо що всі обов'язкові поля присутні
        Assert.True(response.IsSuccessStatusCode);
        Assert.NotNull(result);

        // Перевіряємо що всі поля не null і мають валідні значення
        Assert.True(result.Id > 0);
        Assert.NotNull(result.BlobName);
        Assert.NotEmpty(result.BlobName);
        Assert.NotNull(result.MimeType);
        Assert.NotEmpty(result.MimeType);
        Assert.NotNull(result.Url);
        Assert.NotEmpty(result.Url);
        Assert.True(result.CreatedAt > DateTime.MinValue);

        // Перевіряємо формат даних
        Assert.StartsWith("image/", result.MimeType);
        Assert.Contains("http", result.Url);
    }

    private async Task<Image> CreateTestImageAsync(string? customBlobName = null, string? customMimeType = null)
    {
        var blobName = customBlobName ?? Guid.NewGuid().ToString().Replace("-", "");
        var mimeType = customMimeType ?? "image/png";
        var extension = GetExtensionFromMimeType(mimeType);
        var url = $"http://test.com/{blobName}.{extension}";

        var image = new Image
        {
            BlobName = blobName,
            MimeType = mimeType,
            Url = url,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.Images.Add(image);
        await _dbContext.SaveChangesAsync();

        return image;
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
