using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Images;
using VictoryCenter.DAL.Entities;
using VictoryCenter.IntegrationTests.ControllerTests.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.Images.Update;

[Collection("SharedIntegrationTests")]
public class UpdateImageTest : IAsyncLifetime
{
    private readonly IntegrationTestDbFixture _fixture;
    private readonly JsonSerializerOptions _jsonOptions;

    public UpdateImageTest(IntegrationTestDbFixture fixture)
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
    public async Task UpdateImage_ValidData_ShouldUpdateImage()
    {
        Image? image = await _fixture.DbContext.Images.FirstOrDefaultAsync();
        var id = image.Id;

        var extension = image.MimeType.Split("/")[1];
        string filePath = Path.Combine(_fixture.BlobEnvironmentVariables.FullPath, image.BlobName + "." + extension);
        var oldHash = ComputeFileHash(filePath);

        var updateImageDto = new UpdateImageDTO
        {
            Base64 = "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR4nGMAAQAABQABDQottAAAAABJRU5ErkJggg==",
            MimeType = "image/png"
        };

        var serializedDto = JsonSerializer.Serialize(updateImageDto);

        HttpResponseMessage response = await _fixture.HttpClient.PutAsync($"api/image/{id}", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));
        var responseString = await response.Content.ReadAsStringAsync();
        ImageDTO? responseContext = JsonSerializer.Deserialize<ImageDTO>(responseString, _jsonOptions);

        var newExtension = responseContext.MimeType.Split("/")[1];
        var newFilePath = Path.Combine(_fixture.BlobEnvironmentVariables.FullPath, responseContext.BlobName + "." + newExtension);
        var newHash = ComputeFileHash(newFilePath);

        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal(image.Id, responseContext.Id);
        Assert.Equal(updateImageDto.MimeType, responseContext.MimeType);
        Assert.NotEqual(oldHash, newHash);
    }

    [Fact]
    public async Task UpdateImage_InvalidId_ShouldFail()
    {
        var invalidId = int.MaxValue;

        var updateImageDto = new UpdateImageDTO
        {
            Base64 = "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR4nGMAAQAABQABDQottAAAAABJRU5ErkJggg==",
            MimeType = "image/png"
        };

        var serializedDto = JsonSerializer.Serialize(updateImageDto);

        HttpResponseMessage response = await _fixture.HttpClient.PutAsync($"api/image/{invalidId}", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));
        var responseString = await response.Content.ReadAsStringAsync();
        ImageDTO? responseContext = JsonSerializer.Deserialize<ImageDTO>(responseString, _jsonOptions);

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(response.StatusCode, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateImage_InvalidData_ShouldFail()
    {
        Image? image = await _fixture.DbContext.Images.FirstOrDefaultAsync();
        var id = image.Id;

        var updateImageDto = new UpdateImageDTO
        {
            Base64 = "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR4nGMAAQAABQABDQottAAAAABJRU5ErkJggg==",
            MimeType = "image/gif"
        };

        var serializedDto = JsonSerializer.Serialize(updateImageDto);

        HttpResponseMessage response = await _fixture.HttpClient.PutAsync($"api/image/{id}", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));
        var responseString = await response.Content.ReadAsStringAsync();
        ImageDTO? responseContext = JsonSerializer.Deserialize<ImageDTO>(responseString, _jsonOptions);

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(response.StatusCode, HttpStatusCode.BadRequest);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task UpdateImage_InvalidBase64_ShouldReturnBadRequest(string invalidBase64)
    {
        var testImage = new Image()
        {
            Id = 1100,
            MimeType = "image/png",
            BlobName = "test123",
            CreatedAt = DateTime.Now
        };
        _fixture.DbContext.Images.Add(testImage);

        var updateImageDto = new UpdateImageDTO
        {
            Base64 = invalidBase64,
            MimeType = "image/png"
        };

        var serializedDto = JsonSerializer.Serialize(updateImageDto);

        HttpResponseMessage response = await _fixture.HttpClient.PutAsync(
            $"api/image/{testImage.Id}",
            new StringContent(serializedDto, Encoding.UTF8, "application/json"));

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    private static string ComputeFileHash(string filePath)
    {
        using var sha256 = SHA256.Create();
        using FileStream stream = File.OpenRead(filePath);
        var hash = sha256.ComputeHash(stream);
        return Convert.ToBase64String(hash);
    }
}
