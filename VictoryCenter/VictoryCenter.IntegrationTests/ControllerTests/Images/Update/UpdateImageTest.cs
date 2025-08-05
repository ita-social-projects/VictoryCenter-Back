using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Images;
using VictoryCenter.BLL.Services.BlobStorage;
using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using VictoryCenter.IntegrationTests.ControllerTests.Base;

namespace VictoryCenter.IntegrationTests.ControllerTests.Images.Update;

[Collection("SharedIntegrationTests")]
public class UpdateImageTest
{
    private readonly BlobEnvironmentVariables _blobEnvironment;
    private readonly HttpClient _client;
    private readonly VictoryCenterDbContext _dbContext;
    private readonly JsonSerializerOptions _jsonOptions;

    public UpdateImageTest(IntegrationTestDbFixture fixture)
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
    public async Task UpdateImage_ValidData_ShouldUpdateImage()
    {
        var originalImage = await CreateTestImageAsync("update-test-original", "image/jpeg");
        var originalExtension = GetExtensionFromMimeType(originalImage.MimeType);
        var originalFilePath = Path.Combine(_blobEnvironment.FullPath, $"{originalImage.BlobName}.{originalExtension}");

        await CreatePhysicalTestFileAsync(originalFilePath, "original-content");
        var oldHash = ComputeFileHash(originalFilePath);

        var updateImageDto = new UpdateImageDTO
        {
            Base64 = "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR4nGMAAQAABQABDQottAAAAABJRU5ErkJggg==",
            MimeType = "image/png"
        };

        var serializedDto = JsonSerializer.Serialize(updateImageDto);

        HttpResponseMessage response = await _client.PutAsync(
            $"api/image/{originalImage.Id}",
            new StringContent(serializedDto, Encoding.UTF8, "application/json"));
        var responseString = await response.Content.ReadAsStringAsync();
        ImageDTO? responseContext = JsonSerializer.Deserialize<ImageDTO>(responseString, _jsonOptions);

        Assert.True(response.IsSuccessStatusCode);
        Assert.NotNull(responseContext);

        Assert.Equal(originalImage.Id, responseContext.Id);
        Assert.Equal(updateImageDto.MimeType, responseContext.MimeType);

        Assert.Equal(originalImage.BlobName, responseContext.BlobName);

        Assert.NotNull(responseContext.Url);
        Assert.Contains(responseContext.BlobName, responseContext.Url);

        var newExtension = GetExtensionFromMimeType(updateImageDto.MimeType);
        var newFilePath = Path.Combine(_blobEnvironment.FullPath, $"{responseContext.BlobName}.{newExtension}");
        Assert.True(File.Exists(newFilePath));

        var newHash = ComputeFileHash(newFilePath);
        Assert.NotEqual(oldHash, newHash);

        if (originalExtension != newExtension)
        {
            Assert.False(File.Exists(originalFilePath));
        }
    }

    [Fact]
    public async Task UpdateImage_InvalidId_ShouldReturnNotFound()
    {
        var invalidId = long.MaxValue;

        var updateImageDto = new UpdateImageDTO
        {
            Base64 = "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR4nGMAAQAABQABDQottAAAAABJRU5ErkJggg==",
            MimeType = "image/png"
        };

        var serializedDto = JsonSerializer.Serialize(updateImageDto);

        HttpResponseMessage response = await _client.PutAsync(
            $"api/image/{invalidId}",
            new StringContent(serializedDto, Encoding.UTF8, "application/json"));

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public async Task UpdateImage_InvalidIdValues_ShouldReturnNotFound(long invalidId)
    {
        // Arrange
        var updateImageDto = new UpdateImageDTO
        {
            Base64 = "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR4nGMAAQAABQABDQottAAAAABJRU5ErkJggg==",
            MimeType = "image/png"
        };

        var serializedDto = JsonSerializer.Serialize(updateImageDto);

        HttpResponseMessage response = await _client.PutAsync(
            $"api/image/{invalidId}",
            new StringContent(serializedDto, Encoding.UTF8, "application/json"));

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateImage_UnsupportedMimeType_ShouldReturnBadRequest()
    {
        var testImage = await CreateTestImageAsync("update-test-unsupported");

        var updateImageDto = new UpdateImageDTO
        {
            Base64 = "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR4nGMAAQAABQABDQottAAAAABJRU5ErkJggg==",
            MimeType = "image/gif"
        };

        var serializedDto = JsonSerializer.Serialize(updateImageDto);

        HttpResponseMessage response = await _client.PutAsync(
            $"api/image/{testImage.Id}",
            new StringContent(serializedDto, Encoding.UTF8, "application/json"));

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task UpdateImage_InvalidBase64_ShouldReturnBadRequest(string invalidBase64)
    {
        var testImage = await CreateTestImageAsync("update-test-invalid-base64");

        var updateImageDto = new UpdateImageDTO
        {
            Base64 = invalidBase64,
            MimeType = "image/png"
        };

        var serializedDto = JsonSerializer.Serialize(updateImageDto);

        HttpResponseMessage response = await _client.PutAsync(
            $"api/image/{testImage.Id}",
            new StringContent(serializedDto, Encoding.UTF8, "application/json"));

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task UpdateImage_InvalidMimeType_ShouldReturnBadRequest(string invalidMimeType)
    {
        var testImage = await CreateTestImageAsync("update-test-invalid-mime");

        var updateImageDto = new UpdateImageDTO
        {
            Base64 = "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR4nGMAAQAABQABDQottAAAAABJRU5ErkJggg==",
            MimeType = invalidMimeType
        };

        var serializedDto = JsonSerializer.Serialize(updateImageDto);

        HttpResponseMessage response = await _client.PutAsync(
            $"api/image/{testImage.Id}",
            new StringContent(serializedDto, Encoding.UTF8, "application/json"));

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateImage_ChangeFromJpegToPng_ShouldUpdateCorrectly()
    {
        var originalImage = await CreateTestImageAsync("jpeg-to-png-test", "image/jpeg");
        var originalFilePath = Path.Combine(_blobEnvironment.FullPath, $"{originalImage.BlobName}.jpg");
        await CreatePhysicalTestFileAsync(originalFilePath, "jpg-content");

        var updateImageDto = new UpdateImageDTO
        {
            Base64 = "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR4nGMAAQAABQABDQottAAAAABJRU5ErkJggg==",
            MimeType = "image/png"
        };

        var serializedDto = JsonSerializer.Serialize(updateImageDto);

        HttpResponseMessage response = await _client.PutAsync(
            $"api/image/{originalImage.Id}",
            new StringContent(serializedDto, Encoding.UTF8, "application/json"));
        var responseString = await response.Content.ReadAsStringAsync();
        ImageDTO? result = JsonSerializer.Deserialize<ImageDTO>(responseString, _jsonOptions);

        Assert.True(response.IsSuccessStatusCode);
        Assert.NotNull(result);
        Assert.Equal("image/png", result.MimeType);

        var newFilePath = Path.Combine(_blobEnvironment.FullPath, $"{result.BlobName}.png");
        Assert.True(File.Exists(newFilePath));

        Assert.False(File.Exists(originalFilePath));
    }

    [Fact]
    public async Task UpdateImage_WithDataUrlPrefix_ShouldUpdateCorrectly()
    {
        var testImage = await CreateTestImageAsync("data-url-test");

        var updateImageDto = new UpdateImageDTO
        {
            Base64 = "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR4nGMAAQAABQABDQottAAAAABJRU5ErkJggg==",
            MimeType = "image/png"
        };

        var serializedDto = JsonSerializer.Serialize(updateImageDto);

        HttpResponseMessage response = await _client.PutAsync(
            $"api/image/{testImage.Id}",
            new StringContent(serializedDto, Encoding.UTF8, "application/json"));
        var responseString = await response.Content.ReadAsStringAsync();
        ImageDTO? result = JsonSerializer.Deserialize<ImageDTO>(responseString, _jsonOptions);

        Assert.True(response.IsSuccessStatusCode);
        Assert.NotNull(result);
        Assert.Equal("image/png", result.MimeType);

        var filePath = Path.Combine(_blobEnvironment.FullPath, $"{result.BlobName}.png");
        Assert.True(File.Exists(filePath));
    }

    [Fact]
    public async Task UpdateImage_DatabaseAndFileSystem_ShouldBeConsistent()
    {
        var originalImage = await CreateTestImageAsync("consistency-test", "image/jpeg");
        var originalCreatedAt = originalImage.CreatedAt;

        var updateImageDto = new UpdateImageDTO
        {
            Base64 = "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR4nGMAAQAABQABDQottAAAAABJRU5ErkJggg==",
            MimeType = "image/png"
        };

        var serializedDto = JsonSerializer.Serialize(updateImageDto);
        HttpResponseMessage response = await _client.PutAsync(
            $"api/image/{originalImage.Id}",
            new StringContent(serializedDto, Encoding.UTF8, "application/json"));
        var responseString = await response.Content.ReadAsStringAsync();
        ImageDTO? result = JsonSerializer.Deserialize<ImageDTO>(responseString, _jsonOptions);

        Assert.True(response.IsSuccessStatusCode);
        Assert.NotNull(result);

        var updatedImageInDb = await _dbContext.Images.AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == originalImage.Id);
        Assert.NotNull(updatedImageInDb);
        Assert.Equal("image/png", updatedImageInDb.MimeType);
        Assert.Equal(originalImage.BlobName, updatedImageInDb.BlobName);
        Assert.Equal(originalCreatedAt, updatedImageInDb.CreatedAt);

        var filePath = Path.Combine(_blobEnvironment.FullPath, $"{updatedImageInDb.BlobName}.png");
        Assert.True(File.Exists(filePath));
    }

    private async Task<Image> CreateTestImageAsync(string blobName, string? customMimeType = null)
    {
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

    private async Task CreatePhysicalTestFileAsync(string filePath, string content)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
        var contentBytes = Encoding.UTF8.GetBytes(content);
        await File.WriteAllBytesAsync(filePath, contentBytes);
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

    private static string ComputeFileHash(string filePath)
    {
        using var sha256 = SHA256.Create();
        using FileStream stream = File.OpenRead(filePath);
        var hash = sha256.ComputeHash(stream);
        return Convert.ToBase64String(hash);
    }
}
