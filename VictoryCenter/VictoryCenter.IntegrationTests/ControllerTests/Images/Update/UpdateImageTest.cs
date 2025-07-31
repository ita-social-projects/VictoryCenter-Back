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
        Image? image = await _dbContext.Images.FirstOrDefaultAsync();
        var id = image.Id;

        var extension = image.MimeType.Split("/")[1];
        var filePath = _blobEnvironment.BlobStorePath + image.BlobName + "." + extension;
        var oldHash = ComputeFileHash(filePath);

        var updateImageDto = new UpdateImageDTO
        {
            Base64 = "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR4nGMAAQAABQABDQottAAAAABJRU5ErkJggg==",
            MimeType = "image/png"
        };

        var serializedDto = JsonSerializer.Serialize(updateImageDto);

        HttpResponseMessage response = await _client.PutAsync($"api/image/{id}", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));
        var responseString = await response.Content.ReadAsStringAsync();
        ImageDTO? responseContext = JsonSerializer.Deserialize<ImageDTO>(responseString, _jsonOptions);

        var newExtension = responseContext.MimeType.Split("/")[1];
        var newFilePath = _blobEnvironment.BlobStorePath + responseContext.BlobName + "." + newExtension;
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

        HttpResponseMessage response = await _client.PutAsync($"api/image/{invalidId}", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));
        var responseString = await response.Content.ReadAsStringAsync();
        ImageDTO? responseContext = JsonSerializer.Deserialize<ImageDTO>(responseString, _jsonOptions);

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(response.StatusCode, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateImage_InvalidData_ShouldFail()
    {
        Image? image = await _dbContext.Images.FirstOrDefaultAsync();
        var id = image.Id;

        var updateImageDto = new UpdateImageDTO
        {
            Base64 = "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR4nGMAAQAABQABDQottAAAAABJRU5ErkJggg==",
            MimeType = "image/gif"
        };

        var serializedDto = JsonSerializer.Serialize(updateImageDto);

        HttpResponseMessage response = await _client.PutAsync($"api/image/{id}", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));
        var responseString = await response.Content.ReadAsStringAsync();
        ImageDTO? responseContext = JsonSerializer.Deserialize<ImageDTO>(responseString, _jsonOptions);

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(response.StatusCode, HttpStatusCode.BadRequest);
    }

    private static string ComputeFileHash(string filePath)
    {
        using var sha256 = SHA256.Create();
        using FileStream stream = File.OpenRead(filePath);
        var hash = sha256.ComputeHash(stream);
        return Convert.ToBase64String(hash);
    }
}
