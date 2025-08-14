using System.Net;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.DAL.Entities;
using VictoryCenter.IntegrationTests.ControllerTests.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.Images.Delete;

[Collection("SharedIntegrationTests")]
public class DeleteImageTests : IAsyncLifetime
{
    private readonly IntegrationTestDbFixture _fixture;

    public DeleteImageTests(IntegrationTestDbFixture fixture)
    {
        _fixture = fixture;
    }

    public async Task InitializeAsync()
    {
        await _fixture.CreateFreshWebApplication();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task DeleteImage_ValidData_ShouldDeleteImage()
    {
        Image? image = await _fixture.DbContext.Images.OrderByDescending(i => i.Id).FirstOrDefaultAsync();
        var id = image!.Id;

        string extension = image.MimeType.Split("/")[1];
        string path = _fixture.BlobEnvironmentVariables.BlobStorePath + image.BlobName + "." + extension;

        HttpResponseMessage response = await _fixture.HttpClient.DeleteAsync($"api/Image/{id}");

        Assert.True(response.IsSuccessStatusCode);
        Assert.Null(await _fixture.DbContext.Images.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id));
        Assert.False(File.Exists(path));
    }

    [Fact]
    public async Task DeleteImage_InvalidId_ShouldFail()
    {
        var id = int.MaxValue;

        HttpResponseMessage response = await _fixture.HttpClient.DeleteAsync($"api/Image/{id}");

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
