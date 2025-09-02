using System.Net;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Images;
using VictoryCenter.DAL.Entities;
using VictoryCenter.IntegrationTests.ControllerTests.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.Images.GetByName;

[Collection("SharedIntegrationTests")]
public class GetImageByNameTest : IAsyncLifetime
{
    private readonly IntegrationTestDbFixture _fixture;
    private readonly JsonSerializerOptions _jsonOptions;

    public GetImageByNameTest(IntegrationTestDbFixture fixture)
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

    public async Task DisposeAsync() => await _fixture.DisposeAsync();

    [Fact]
    public async Task GetImageByName_ValidData_ShouldReturnImage()
    {
        Image? image = await _fixture.DbContext.Images.FirstOrDefaultAsync();
        var name = image.BlobName;

        HttpResponseMessage response = await _fixture.HttpClient.GetAsync($"api/Image/by-name/{name}");

        var responseString = await response.Content.ReadAsStringAsync();
        ImageDTO? result = JsonSerializer.Deserialize<ImageDTO>(responseString, _jsonOptions);

        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal(result.BlobName, image.BlobName);
        Assert.Equal(result.Id, image.Id);
    }

    [Fact]
    public async Task GetImageByName_InvalidData_ShouldReturnError()
    {
        var name = "wrong_name";

        HttpResponseMessage response = await _fixture.HttpClient.GetAsync($"api/Image/by-name/{name}");

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public async Task GetImageByName_EmptyOrWhitespaceName_ShouldReturnNotFound(string invalidName)
    {
        HttpResponseMessage response = await _fixture.HttpClient.GetAsync($"api/Image/by-name/{Uri.EscapeDataString(invalidName)}");

        Assert.False(response.IsSuccessStatusCode);

        Assert.True(response.StatusCode is HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetImageByName_ImageWithEmptyBlobName_ShouldReturnError()
    {
        var imageWithEmptyBlobName = new Image
        {
            BlobName = "",
            MimeType = "image/png",
            Url = "http://test.com/empty.png",
            CreatedAt = DateTime.UtcNow
        };

        _fixture.DbContext.Images.Add(imageWithEmptyBlobName);
        await _fixture.DbContext.SaveChangesAsync();

        HttpResponseMessage response = await _fixture.HttpClient.GetAsync($"api/Image/by-name/");

        Assert.False(response.IsSuccessStatusCode);
        Assert.True(response.StatusCode is HttpStatusCode.BadRequest);
    }
}
