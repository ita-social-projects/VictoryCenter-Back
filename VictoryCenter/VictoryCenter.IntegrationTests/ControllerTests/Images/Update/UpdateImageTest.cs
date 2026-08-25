using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Images;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.DAL.Entities;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;
using VictoryCenter.IntegrationTests.Utils.Images;

namespace VictoryCenter.IntegrationTests.ControllerTests.Images.Update;

public class UpdateImageTest : BaseTestClass
{
    public UpdateImageTest(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task UpdateImage_ValidData_ShouldUpdateImage()
    {
        Image? image = await Fixture.DbContext.Images.FirstOrDefaultAsync();
        var id = image!.Id;

        var extension = image.MimeType.Split("/")[1];
        string filePath = Path.Combine(Fixture.BlobEnvironmentVariables.FullPath, image.BlobName + "." + extension);
        var oldHash = ComputeFileHash(filePath);

        var updateImageDto = new UpdateImageDto
        {
            Base64 = ImageTestData.CreateBase64("image/png"),
            MimeType = "image/png"
        };

        var serializedDto = JsonSerializer.Serialize(updateImageDto);

        HttpResponseMessage response = await Fixture.HttpClient.PutAsync($"api/image/{id}", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));
        var responseString = await response.Content.ReadAsStringAsync();
        ImageDto? responseContext = JsonSerializer.Deserialize<ImageDto>(responseString, JsonOptions);

        var newExtension = responseContext!.MimeType.Split("/")[1];
        var newFilePath = Path.Combine(Fixture.BlobEnvironmentVariables.FullPath, responseContext.BlobName + "." + newExtension);
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

        var updateImageDto = new UpdateImageDto
        {
            Base64 = ImageTestData.CreateBase64("image/png"),
            MimeType = "image/png"
        };

        var serializedDto = JsonSerializer.Serialize(updateImageDto);

        HttpResponseMessage response = await Fixture.HttpClient.PutAsync($"api/image/{invalidId}", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateImage_InvalidData_ShouldFail()
    {
        Image? image = await Fixture.DbContext.Images.FirstOrDefaultAsync();
        var id = image!.Id;

        var updateImageDto = new UpdateImageDto
        {
            Base64 = ImageTestData.CreateBase64("image/png"),
            MimeType = "image/gif"
        };

        var serializedDto = JsonSerializer.Serialize(updateImageDto);

        HttpResponseMessage response = await Fixture.HttpClient.PutAsync($"api/image/{id}", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task UpdateImage_InvalidBase64_ShouldReturnBadRequest(string? invalidBase64)
    {
        var testImage = new Image
        {
            Id = 1100,
            MimeType = "image/png",
            BlobName = "test123",
            CreatedAt = DateTimeOffset.UtcNow
        };
        Fixture.DbContext.Images.Add(testImage);

        var updateImageDto = new UpdateImageDto
        {
            Base64 = invalidBase64,
            MimeType = "image/png"
        };

        var serializedDto = JsonSerializer.Serialize(updateImageDto);

        HttpResponseMessage response = await Fixture.HttpClient.PutAsync(
            $"api/image/{testImage.Id}",
            new StringContent(serializedDto, Encoding.UTF8, "application/json"));

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateImage_MimeTypeDoesNotMatchContent_ShouldReturnBadRequest()
    {
        Image? image = await Fixture.DbContext.Images.FirstOrDefaultAsync();
        var dto = new UpdateImageDto
        {
            Base64 = ImageTestData.CreateBase64("image/png"),
            MimeType = "image/webp"
        };

        HttpResponseMessage response = await Fixture.HttpClient.PutAsync(
            $"api/image/{image!.Id}",
            new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateImage_DataUrl_ShouldReturnBadRequest()
    {
        Image? image = await Fixture.DbContext.Images.FirstOrDefaultAsync();
        string base64 = ImageTestData.CreateBase64(ImageMimeTypes.Png);
        var dto = new UpdateImageDto
        {
            Base64 = $"data:{ImageMimeTypes.Png};base64,{base64}",
            MimeType = ImageMimeTypes.Png
        };

        HttpResponseMessage response = await Fixture.HttpClient.PutAsync(
            $"api/image/{image!.Id}",
            new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json"));

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
