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
    public async Task DeleteImage_ShouldDeleteImage()
    {
        Image? image = await _dbContext.Images.FirstOrDefaultAsync();
        var id = image.Id;

        string extension = image.MimeType.Split("/")[1];
        string path = _blobEnvironment.BlobStorePath + image.BlobName + "." + extension;

        HttpResponseMessage response = await _client.DeleteAsync($"api/Image/{id}");

        Assert.True(response.IsSuccessStatusCode);
        Assert.Null(await _dbContext.Images.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id));
        Assert.False(File.Exists(path));
    }

    [Fact]
    public async Task DeleteImage_InvalidId_ShouldFail()
    {
        var id = int.MaxValue;

        HttpResponseMessage response = await _client.DeleteAsync($"api/Image/{id}");

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
