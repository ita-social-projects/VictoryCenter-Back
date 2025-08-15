using System.Net;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Admin.Images;
using VictoryCenter.DAL.Entities;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.Images.GetById;

public class GetImageByIdTest : BaseTestClass
{
    public GetImageByIdTest(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task GetImageById_ValidData_ShouldReturnImage()
    {
        Image? image = await Fixture.DbContext.Images.FirstOrDefaultAsync();
        var id = image!.Id;

        HttpResponseMessage response = await Fixture.HttpClient.GetAsync($"api/Image/{id}");

        var responseString = await response.Content.ReadAsStringAsync();
        ImageDto? result = JsonSerializer.Deserialize<ImageDto>(responseString, JsonOptions);

        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal(result!.BlobName, image.BlobName);
        Assert.Equal(result.Id, image.Id);
    }

    [Fact]
    public async Task GetImageById_InvalidData_ShouldReturnError()
    {
        var id = int.MaxValue;

        HttpResponseMessage response = await Fixture.HttpClient.GetAsync($"api/Image/{id}");

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
