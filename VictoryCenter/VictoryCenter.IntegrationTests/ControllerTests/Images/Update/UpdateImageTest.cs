using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Images;
using VictoryCenter.BLL.Services.BlobStorage;
using VictoryCenter.DAL.Data;
using VictoryCenter.IntegrationTests.ControllerTests.Base;

namespace VictoryCenter.IntegrationTests.ControllerTests.Images.Update;

[Collection("SharedIntegrationTests")]
public class UpdateImageTest
{
    private readonly HttpClient _client;
    private readonly VictoryCenterDbContext _dbContext;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly BlobEnvironmentVariables _blobEnvironment;

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
    public async Task UpdateImage_ShouldUpdateImage()
    {
        var image = await _dbContext.Images.FirstOrDefaultAsync();
        var id = image.Id;

        string extension = image.MimeType.Split("/")[1];
        string filePath = _blobEnvironment.BlobStorePath + image.BlobName + "." + extension;
        string oldHash = ComputeFileHash(filePath);

        var updateImageDto = new UpdateImageDTO()
        {
            Base64 = "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR4nGMAAQAABQABDQottAAAAABJRU5ErkJggg==",
            MimeType = "image/png"
        };

        var serializedDto = JsonSerializer.Serialize(updateImageDto);

        var responce = await _client.PutAsync($"api/image/{id}", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));
        var responceString = await responce.Content.ReadAsStringAsync();
        var responceContext = JsonSerializer.Deserialize<ImageDTO>(responceString, _jsonOptions);

        string newExtension = responceContext.MimeType.Split("/")[1];
        string newFilePath = _blobEnvironment.BlobStorePath + responceContext.BlobName + "." + newExtension;
        string newHash = ComputeFileHash(newFilePath);

        Assert.True(responce.IsSuccessStatusCode);
        Assert.Equal(image.Id, responceContext.Id);
        Assert.Equal(updateImageDto.MimeType, responceContext.MimeType);
        Assert.NotEqual(oldHash, newHash);
    }

    [Fact]
    public async Task UpdateImage_InvalidId_ShouldFail()
    {
        var invalidId = int.MaxValue;

        var updateImageDto = new UpdateImageDTO()
        {
            Base64 = "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR4nGMAAQAABQABDQottAAAAABJRU5ErkJggg==",
            MimeType = "image/png"
        };

        var serializedDto = JsonSerializer.Serialize(updateImageDto);

        var responce = await _client.PutAsync($"api/image/{invalidId}", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));
        var responceString = await responce.Content.ReadAsStringAsync();
        var responceContext = JsonSerializer.Deserialize<ImageDTO>(responceString, _jsonOptions);

        Assert.False(responce.IsSuccessStatusCode);
        Assert.Equal(responce.StatusCode, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateImage_InvalidData_ShouldFail()
    {
        var image = await _dbContext.Images.FirstOrDefaultAsync();
        var id = image.Id;

        var updateImageDto = new UpdateImageDTO()
        {
            Base64 = "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR4nGMAAQAABQABDQottAAAAABJRU5ErkJggg==",
            MimeType = "image/gif"
        };

        var serializedDto = JsonSerializer.Serialize(updateImageDto);

        var responce = await _client.PutAsync($"api/image/{id}", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));
        var responceString = await responce.Content.ReadAsStringAsync();
        var responceContext = JsonSerializer.Deserialize<ImageDTO>(responceString, _jsonOptions);

        Assert.False(responce.IsSuccessStatusCode);
        Assert.Equal(responce.StatusCode, HttpStatusCode.BadRequest);
    }

    private static string ComputeFileHash(string filePath)
    {
        using var sha256 = SHA256.Create();
        using var stream = File.OpenRead(filePath);
        var hash = sha256.ComputeHash(stream);
        return Convert.ToBase64String(hash);
    }
}
