using System.Net;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Services.BlobStorage;
using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using VictoryCenter.IntegrationTests.ControllerTests.Base;

namespace VictoryCenter.IntegrationTests.ControllerTests.Images.Delete;

[Collection("SharedIntegrationTests")]
public class DeleteImageTests
{
    private readonly HttpClient _client;
    private readonly VictoryCenterDbContext _dbContext;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly BlobEnvironmentVariables _blobEnvironment;

    public DeleteImageTests(IntegrationTestDbFixture fixture)
    {
        _client = fixture.HttpClient;
        _dbContext = fixture.DbContext;
        _blobEnvironment = fixture.BlobVariables;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    [Fact]
    public async Task DeleteImage_ValidData_ShouldDeleteImage()
    {
        // Arrange: Створюємо тестове зображення
        var testImage = await CreateTestImageAsync();
        var imageId = testImage.Id;

        string extension = GetExtensionFromMimeType(testImage.MimeType);
        string filePath = Path.Combine(_blobEnvironment.BlobStorePath, $"{testImage.BlobName}.{extension}");

        // Переконуємося що файл існує перед видаленням
        Assert.True(File.Exists(filePath), "Test file should exist before deletion");

        // Act: Видаляємо зображення
        HttpResponseMessage response = await _client.DeleteAsync($"api/Image/{imageId}");

        // Assert: Перевіряємо результат
        Assert.True(response.IsSuccessStatusCode);

        // Перевіряємо що запис видалено з бази даних
        var deletedImage = await _dbContext.Images.AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == imageId);
        Assert.Null(deletedImage);

        // Перевіряємо що файл видалено з файлової системи
        Assert.False(File.Exists(filePath), "File should be deleted from storage");
    }

    [Fact]
    public async Task DeleteImage_InvalidId_ShouldReturnNotFound()
    {
        // Arrange: Використовуємо неіснуючий ID
        var nonExistentId = long.MaxValue;

        // Act: Намагаємося видалити неіснуюче зображення
        HttpResponseMessage response = await _client.DeleteAsync($"api/Image/{nonExistentId}");

        // Assert: Перевіряємо що повертається 404
        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteImage_ImageWithoutBlobFile_ShouldDeleteFromDatabaseOnly()
    {
        // Arrange: Створюємо зображення в базі даних без відповідного файлу
        var imageWithoutFile = new Image
        {
            BlobName = "non-existent-file",
            MimeType = "image/png",
            Url = "http://test.com/non-existent-file.png",
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.Images.Add(imageWithoutFile);
        await _dbContext.SaveChangesAsync();

        var imageId = imageWithoutFile.Id;

        // Act: Видаляємо зображення
        HttpResponseMessage response = await _client.DeleteAsync($"api/Image/{imageId}");

        // Assert: Перевіряємо що операція успішна навіть без файлу
        Assert.True(response.IsSuccessStatusCode);

        // Перевіряємо що запис видалено з бази даних
        var deletedImage = await _dbContext.Images.AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == imageId);
        Assert.Null(deletedImage);
    }

    [Fact]
    public async Task DeleteImage_MultipleImages_ShouldDeleteOnlySpecified()
    {
        // Arrange: Створюємо два тестових зображення
        var image1 = await CreateTestImageAsync("test-image-1");
        var image2 = await CreateTestImageAsync("test-image-2");

        string filePath1 = Path.Combine(_blobEnvironment.BlobStorePath, $"{image1.BlobName}.png");
        string filePath2 = Path.Combine(_blobEnvironment.BlobStorePath, $"{image2.BlobName}.png");

        // Act: Видаляємо тільки перше зображення
        HttpResponseMessage response = await _client.DeleteAsync($"api/Image/{image1.Id}");

        // Assert: Перевіряємо результат
        Assert.True(response.IsSuccessStatusCode);

        // Перше зображення має бути видалене
        var deletedImage1 = await _dbContext.Images.AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == image1.Id);
        Assert.Null(deletedImage1);
        Assert.False(File.Exists(filePath1));

        // Друге зображення має залишитися
        var remainingImage2 = await _dbContext.Images.AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == image2.Id);
        Assert.NotNull(remainingImage2);
        Assert.True(File.Exists(filePath2));

        // Cleanup: Видаляємо друге зображення
        await _client.DeleteAsync($"api/Image/{image2.Id}");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public async Task DeleteImage_InvalidIdValues_ShouldReturnNotFound(long invalidId)
    {
        // Act: Намагаємося видалити зображення з некоректним ID
        HttpResponseMessage response = await _client.DeleteAsync($"api/Image/{invalidId}");

        // Assert: Перевіряємо що повертається 404
        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private async Task<Image> CreateTestImageAsync(string? customBlobName = null)
    {
        var blobName = customBlobName ?? Guid.NewGuid().ToString().Replace("-", "");
        var mimeType = "image/png";
        var extension = GetExtensionFromMimeType(mimeType);

        // Створюємо запис в базі даних
        var image = new Image
        {
            BlobName = blobName,
            MimeType = mimeType,
            Url = $"http://test.com/{blobName}.{extension}",
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.Images.Add(image);
        await _dbContext.SaveChangesAsync();

        // Створюємо тестовий файл
        var filePath = Path.Combine(_blobEnvironment.BlobStorePath, $"{blobName}.{extension}");
        Directory.CreateDirectory(_blobEnvironment.BlobStorePath);

        // Створюємо мінімальний PNG файл (1x1 pixel)
        var testImageBytes = Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR4nGNgYAAAAAMAAWgmWQ0AAAAASUVORK5CYII=");
        await File.WriteAllBytesAsync(filePath, testImageBytes);

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
