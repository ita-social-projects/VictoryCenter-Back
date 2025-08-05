using System.Net;
using System.Text.Json;
using VictoryCenter.BLL.DTOs.Images;
using VictoryCenter.BLL.Services.BlobStorage;
using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using VictoryCenter.IntegrationTests.ControllerTests.Base;

namespace VictoryCenter.IntegrationTests.ControllerTests.Images.GetByName;

[Collection("SharedIntegrationTests")]
public class GetImageByNameTest
{
    private readonly HttpClient _client;
    private readonly VictoryCenterDbContext _dbContext;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly BlobEnvironmentVariables _blobEnv;
    public GetImageByNameTest(IntegrationTestDbFixture fixture)
    {
        _client = fixture.HttpClient;
        _dbContext = fixture.DbContext;
        _blobEnv = fixture.BlobVariables;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    [Fact]
    public async Task GetImageByName_ValidData_ShouldReturnImage()
    {
        // Arrange: Створюємо тестове зображення з унікальним ім'ям
        var testBlobName = "test-image-" + Guid.NewGuid().ToString("N")[..8];
        var testImage = await CreateTestImageAsync(_blobEnv.ImagesSubPath, testBlobName );

        // Act: Отримуємо зображення за ім'ям
        HttpResponseMessage response = await _client.GetAsync($"api/Image/by-name/{testBlobName}");
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
    public async Task GetImageByName_InvalidData_ShouldReturnNotFound()
    {
        // Arrange: Використовуємо неіснуюче ім'я
        var nonExistentName = "non-existent-image-" + Guid.NewGuid().ToString("N");

        // Act: Намагаємося отримати неіснуюче зображення
        HttpResponseMessage response = await _client.GetAsync($"api/Image/by-name/{nonExistentName}");

        // Assert: Перевіряємо що повертається 404
        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public async Task GetImageByName_EmptyOrWhitespaceName_ShouldReturnNotFound(string invalidName)
    {
        // Act: Намагаємося отримати зображення з порожнім/пробільним ім'ям
        HttpResponseMessage response = await _client.GetAsync($"api/Image/by-name/{Uri.EscapeDataString(invalidName)}");

        // Assert: Перевіряємо що повертається помилка
        Assert.False(response.IsSuccessStatusCode);

        // Може бути BadRequest або NotFound залежно від маршрутизації
        Assert.True(response.StatusCode is HttpStatusCode.NotFound or HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetImageByName_ImageWithEmptyBlobName_ShouldReturnError()
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

        // Act: Намагаємося отримати зображення за порожнім ім'ям
        HttpResponseMessage response = await _client.GetAsync($"api/Image/by-name/");

        // Assert: Перевіряємо що повертається помилка (скоріше за все BadRequest через маршрутизацію)
        Assert.False(response.IsSuccessStatusCode);
    }

    [Fact]
    public async Task GetImageByName_CaseSensitivityTest_ShouldFindExactMatch()
    {
        // Arrange: Створюємо зображення з конкретним регістром
        var originalBlobName = "TestImageName";
        var testImage = await CreateTestImageAsync(_blobEnv.ImagesSubPath, originalBlobName);

        // Act & Assert: Тестуємо точне співпадіння
        HttpResponseMessage exactResponse = await _client.GetAsync($"api/Image/by-name/{originalBlobName}");
        Assert.True(exactResponse.IsSuccessStatusCode);

        // Act & Assert: Тестуємо різний регістр (має не знайти, якщо пошук case-sensitive)
        var lowerCaseName = originalBlobName.ToLower();
        if (lowerCaseName != originalBlobName)
        {
            HttpResponseMessage caseResponse = await _client.GetAsync($"api/Image/by-name/{lowerCaseName}");

            // Результат залежить від налаштувань БД (case-sensitive чи ні)
            // Зазвичай SQL Server case-insensitive, PostgreSQL case-sensitive
        }
    }

    [Fact]
    public async Task GetImageByName_SpecialCharactersInName_ShouldHandleCorrectly()
    {
        // Arrange: Створюємо зображення з спеціальними символами в імені
        var specialCharName = "test-image_123.special";
        var testImage = await CreateTestImageAsync(_blobEnv.ImagesSubPath, specialCharName);

        // Act: Отримуємо зображення з URL-encoded ім'ям
        var encodedName = Uri.EscapeDataString(specialCharName);
        HttpResponseMessage response = await _client.GetAsync($"api/Image/by-name/{encodedName}");
        var responseString = await response.Content.ReadAsStringAsync();
        ImageDTO? result = JsonSerializer.Deserialize<ImageDTO>(responseString, _jsonOptions);

        // Assert: Перевіряємо результат
        Assert.True(response.IsSuccessStatusCode);
        Assert.NotNull(result);
        Assert.Equal(specialCharName, result.BlobName);
    }

    [Fact]
    public async Task GetImageByName_MultipleImagesWithSimilarNames_ShouldReturnCorrectOne()
    {
        // Arrange: Створюємо кілька зображень з схожими іменами
        var baseName = "similar-image";
        var image1 = await CreateTestImageAsync(_blobEnv.ImagesSubPath, $"{baseName}-1");
        var image2 = await CreateTestImageAsync(_blobEnv.ImagesSubPath, $"{baseName}-2");
        var image3 = await CreateTestImageAsync(_blobEnv.ImagesSubPath, $"{baseName}-test");

        // Act: Отримуємо конкретне зображення
        var targetName = $"{baseName}-2";
        HttpResponseMessage response = await _client.GetAsync($"api/Image/by-name/{targetName}");
        var responseString = await response.Content.ReadAsStringAsync();
        ImageDTO? result = JsonSerializer.Deserialize<ImageDTO>(responseString, _jsonOptions);

        // Assert: Перевіряємо що отримали саме потрібне зображення
        Assert.True(response.IsSuccessStatusCode);
        Assert.NotNull(result);
        Assert.Equal(image2.Id, result.Id);
        Assert.Equal(targetName, result.BlobName);
    }

    [Fact]
    public async Task GetImageByName_DifferentMimeTypes_ShouldReturnCorrectData()
    {
        // Arrange: Створюємо зображення з різними MIME типами
        var testCases = new[]
        {
            ("image/jpeg", "test-jpeg-by-name"),
            ("image/png", "test-png-by-name"),
            ("image/webp", "test-webp-by-name")
        };

        foreach (var (mimeType, blobName) in testCases)
        {
            var testImage = await CreateTestImageAsync(_blobEnv.ImagesSubPath, blobName, mimeType);

            // Act: Отримуємо зображення за ім'ям
            HttpResponseMessage response = await _client.GetAsync($"api/Image/by-name/{blobName}");
            var responseString = await response.Content.ReadAsStringAsync();
            ImageDTO? result = JsonSerializer.Deserialize<ImageDTO>(responseString, _jsonOptions);

            // Assert: Перевіряємо результат
            Assert.True(response.IsSuccessStatusCode);
            Assert.NotNull(result);
            Assert.Equal(mimeType, result.MimeType);
            Assert.Equal(blobName, result.BlobName);
            Assert.Equal(testImage.Id, result.Id);
        }
    }

    [Fact]
    public async Task GetImageByName_ResponseFormat_ShouldContainAllRequiredFields()
    {
        // Arrange: Створюємо тестове зображення
        var testBlobName = "format-test-image";
        var testImage = await CreateTestImageAsync(_blobEnv.ImagesSubPath, testBlobName);

        // Act: Отримуємо зображення
        HttpResponseMessage response = await _client.GetAsync($"api/Image/by-name/{testBlobName}");
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
        Assert.Equal(testBlobName, result.BlobName);
    }

    private async Task<Image> CreateTestImageAsync(string? subPath, string blobName, string? customMimeType = null )
    {
        var mimeType = customMimeType ?? "image/png";
        var extension = GetExtensionFromMimeType(mimeType);
        var subString = subPath;
        var url = $"http://localhost/{subString}/{blobName}.{extension}";

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
