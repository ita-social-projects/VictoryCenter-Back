using System.Net;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.DAL.Entities;
using VictoryCenter.IntegrationTests.ControllerTests.Base;

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
        await _fixture.CreateFreshDatabase();

        var count = await _fixture.DbContext.TeamMembers.CountAsync();

        if (count == 0)
        {
            throw new InvalidOperationException("No TeamMembers were seeded. Check your seeder implementation.");
        }
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task DeleteImage_ValidData_ShouldDeleteImage()
    {
        Image? image = await _fixture.DbContext.Images.OrderByDescending(i => i.Id).FirstOrDefaultAsync();
        var id = image.Id;

        string extension = image.MimeType.Split("/")[1];
        string path = _fixture._blobEnvironmentVariables.BlobStorePath + image.BlobName + "." + extension;

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
