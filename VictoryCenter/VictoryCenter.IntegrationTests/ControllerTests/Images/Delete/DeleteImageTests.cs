using System.Net;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.DAL.Entities;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.Images.Delete;

public class DeleteImageTests : BaseTestClass
{
    public DeleteImageTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task DeleteImage_ValidData_ShouldDeleteImage()
    {
        Image? image = await Fixture.DbContext.Images.OrderByDescending(i => i.Id).FirstOrDefaultAsync();
        var id = image!.Id;

        string extension = image.MimeType.Split("/")[1];
        string path = Fixture.BlobEnvironmentVariables.BlobStorePath + image.BlobName + "." + extension;

        HttpResponseMessage response = await Fixture.HttpClient.DeleteAsync($"api/Image/{id}");

        Assert.True(response.IsSuccessStatusCode);
        Assert.Null(await Fixture.DbContext.Images.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id));
        Assert.False(File.Exists(path));
    }

    [Fact]
    public async Task DeleteImage_InvalidId_ShouldFail()
    {
        var id = int.MaxValue;

        HttpResponseMessage response = await Fixture.HttpClient.DeleteAsync($"api/Image/{id}");

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
