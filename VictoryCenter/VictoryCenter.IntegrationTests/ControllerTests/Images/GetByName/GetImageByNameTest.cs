using System.Net;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Admin.Images;
using VictoryCenter.DAL.Entities;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.Images.GetByName;

public class GetImageByNameTest : BaseTestClass
{
    public GetImageByNameTest(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task GetImageByName_ValidData_ShouldReturnImage()
    {
        Image? image = await Fixture.DbContext.Images.FirstOrDefaultAsync();
        var name = image!.BlobName;

        HttpResponseMessage response = await Fixture.HttpClient.GetAsync($"api/Image/by-name/{name}");

        var responseString = await response.Content.ReadAsStringAsync();
        ImageDto? result = JsonSerializer.Deserialize<ImageDto>(responseString, JsonOptions);

        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal(result!.BlobName, image.BlobName);
        Assert.Equal(result.Id, image.Id);
    }

    [Fact]
    public async Task GetImageByName_InvalidData_ShouldReturnError()
    {
        var name = "wrong_name";

        HttpResponseMessage response = await Fixture.HttpClient.GetAsync($"api/Image/by-name/{name}");

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
