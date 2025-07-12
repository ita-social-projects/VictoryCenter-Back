using System.Net;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Images;
using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using VictoryCenter.IntegrationTests.ControllerTests.Base;

namespace VictoryCenter.IntegrationTests.ControllerTests.Images.GetById;

[Collection("SharedIntegrationTests")]
public class GetImageByIdTest
{
    private readonly HttpClient _client;
    private readonly VictoryCenterDbContext _dbContext;
    private readonly JsonSerializerOptions _jsonOptions;

    public GetImageByIdTest(IntegrationTestDbFixture fixture)
    {
        _client = fixture.HttpClient;
        _dbContext = fixture.DbContext;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    [Fact]
    public async Task GetImageById_ShouldReturnImage()
    {
        Image? image = await _dbContext.Images.FirstOrDefaultAsync();
        var id = image.Id;

        HttpResponseMessage responce = await _client.GetAsync($"api/Image/{id}");

        var responceString = await responce.Content.ReadAsStringAsync();
        ImageDTO? result = JsonSerializer.Deserialize<ImageDTO>(responceString, _jsonOptions);

        Assert.True(responce.IsSuccessStatusCode);
        Assert.Equal(result.BlobName, image.BlobName);
        Assert.Equal(result.Id, image.Id);
    }

    [Fact]
    public async Task GetImageById_InvalidData_ShouldReturnError()
    {
        var id = int.MaxValue;

        HttpResponseMessage responce = await _client.GetAsync($"api/Image/{id}");

        var responceString = await responce.Content.ReadAsStringAsync();
        ImageDTO? result = JsonSerializer.Deserialize<ImageDTO>(responceString, _jsonOptions);

        Assert.False(responce.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.NotFound, responce.StatusCode);
    }
}
