using System.Net;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Images;
using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using VictoryCenter.IntegrationTests.ControllerTests.Base;

namespace VictoryCenter.IntegrationTests.ControllerTests.Images.GetByName;

[Collection("SharedIntegrationTests")]
public class GetImageByNameTest
{
    private readonly HttpClient _client;
    private readonly VictoryCenterDbContext _dbContext;
    private readonly JsonSerializerOptions _jsonOptions;

    public GetImageByNameTest(IntegrationTestDbFixture fixture)
    {
        _client = fixture.HttpClient;
        _dbContext = fixture.DbContext;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    [Fact]
    public async Task GetImageByName_ShouldReturnImage()
    {
        Image? image = await _dbContext.Images.FirstOrDefaultAsync();
        var name = image.BlobName;

        HttpResponseMessage responce = await _client.GetAsync($"api/Image/by-name/{name}");

        var responceString = await responce.Content.ReadAsStringAsync();
        ImageDTO? result = JsonSerializer.Deserialize<ImageDTO>(responceString, _jsonOptions);

        Assert.True(responce.IsSuccessStatusCode);
        Assert.Equal(result.BlobName, image.BlobName);
        Assert.Equal(result.Id, image.Id);
    }

    [Fact]
    public async Task GetImageByName_InvalidData_ShouldReturnError()
    {
        var name = "wrong_name";

        HttpResponseMessage responce = await _client.GetAsync($"api/Image/by-name/{name}");

        var responceString = await responce.Content.ReadAsStringAsync();
        ImageDTO? result = JsonSerializer.Deserialize<ImageDTO>(responceString, _jsonOptions);

        Assert.False(responce.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.NotFound, responce.StatusCode);
    }
}
