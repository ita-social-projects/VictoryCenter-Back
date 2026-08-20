using System.Net;
using System.Text;
using System.Text.Json;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Images;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;
using VictoryCenter.IntegrationTests.Utils.Images;

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
            Base64 = ImageTestData.CreateBase64("image/jpg"),
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
            Base64 = ImageTestData.CreateBase64("image/png"),
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
            Base64 = ImageTestData.CreateBase64(mimeType),
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

        string expectedExtension = GetExtensionFromMimeType(mimeType);
        string filePath = Path.Combine(Fixture.BlobEnvironmentVariables.FullPath, $"{responseContext.BlobName}.{expectedExtension}");
        Assert.True(File.Exists(filePath));

        Assert.Contains($".{expectedExtension}", responseContext.Url);
    }

    [Fact]
    public async Task CreateImage_Base64EncodedText_ShouldReturnBadRequest()
    {
        var dto = new CreateImageDto
        {
            Base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes("This is not an image")),
            MimeType = "image/png"
        };

        HttpResponseMessage response = await PostImageAsync(dto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateImage_MimeTypeDoesNotMatchContent_ShouldReturnBadRequest()
    {
        var dto = new CreateImageDto
        {
            Base64 = ImageTestData.CreateBase64("image/png"),
            MimeType = "image/jpeg"
        };

        HttpResponseMessage response = await PostImageAsync(dto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateImage_CorruptedImage_ShouldReturnBadRequest()
    {
        byte[] png = Convert.FromBase64String(ImageTestData.CreateBase64("image/png"));
        var dto = new CreateImageDto
        {
            Base64 = Convert.ToBase64String(png[.. (png.Length / 2)]),
            MimeType = "image/png"
        };

        HttpResponseMessage response = await PostImageAsync(dto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateImage_DimensionsExceedLimit_ShouldReturnBadRequest()
    {
        var dto = new CreateImageDto
        {
            Base64 = ImageTestData.CreateBase64("image/png", 10001, 1),
            MimeType = "image/png"
        };

        HttpResponseMessage response = await PostImageAsync(dto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateImage_Base64PayloadExceedsEncodedImageLimit_ShouldReturnBadRequest()
    {
        var dto = new CreateImageDto
        {
            Base64 = new string('A', ImageConstants.MaxBase64Length + 4),
            MimeType = "image/png"
        };

        HttpResponseMessage response = await PostImageAsync(dto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    private async Task<HttpResponseMessage> PostImageAsync(CreateImageDto dto)
    {
        string json = JsonSerializer.Serialize(dto);
        return await Fixture.HttpClient.PostAsync(
            "api/Image",
            new StringContent(json, Encoding.UTF8, "application/json"));
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
