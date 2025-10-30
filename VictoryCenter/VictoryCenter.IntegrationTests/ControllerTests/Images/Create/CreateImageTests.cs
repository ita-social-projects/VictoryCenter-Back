using System.Net;
using System.Text;
using System.Text.Json;
using VictoryCenter.BLL.DTOs.Admin.Images;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.Images.Create;

public class CreateImageTests : BaseTestClass
{
    public CreateImageTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task CreateImage_ValidData_ShouldCreateImage()
    {
        var createImageDto = new CreateImageDto
        {
            Base64 =
                "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR4nGNgYAAAAAMAAWgmWQ0AAAAASUVORK5CYII=",

            MimeType = "image/jpg"
        };

        var serializedDto = JsonSerializer.Serialize(createImageDto);

        HttpResponseMessage response = await Fixture.HttpClient.PostAsync("api/Image", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));

        var responseString = await response.Content.ReadAsStringAsync();
        ImageDto? responseContext = JsonSerializer.Deserialize<ImageDto>(responseString, JsonOptions);
        string extension = responseContext!.MimeType.Split("/")[1];
        string path = Path.Combine(Fixture.BlobEnvironmentVariables.FullPath, responseContext.BlobName + "." + extension);

        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal(createImageDto.MimeType, responseContext.MimeType);
        Assert.True(File.Exists(path));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task CreateImage_InvalidData_ShouldReturnError(string? mimeType)
    {
        var createImageDto = new CreateImageDto
        {
            Base64 =
                "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR4nGNgYAAAAAMAAWgmWQ0AAAAASUVORK5CYII=",

            MimeType = mimeType!
        };

        var serializedDto = JsonSerializer.Serialize(createImageDto);

        HttpResponseMessage response = await Fixture.HttpClient.PostAsync("api/Image", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));

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
        var createImageDto = new CreateImageDto
        {
            Base64 = "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR4nGNgYAAAAAMAAWgmWQ0AAAAASUVORK5CYII=",
            MimeType = mimeType
        };

        var serializedDto = JsonSerializer.Serialize(createImageDto);
        HttpResponseMessage response = await Fixture.HttpClient.PostAsync(
            "api/Image",
            new StringContent(serializedDto, Encoding.UTF8, "application/json"));

        var responseString = await response.Content.ReadAsStringAsync();
        ImageDto? responseContext = JsonSerializer.Deserialize<ImageDto>(responseString, JsonOptions);

        Assert.True(response.IsSuccessStatusCode);
        Assert.NotNull(responseContext);
        Assert.Equal(mimeType, responseContext.MimeType);

        // Перевіряємо що файл створено з правильним розширенням
        string expectedExtension = GetExtensionFromMimeType(mimeType);
        string filePath = Path.Combine(Fixture.BlobEnvironmentVariables.FullPath, $"{responseContext.BlobName}.{expectedExtension}");
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
