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
    public async Task GetImageById_ValidData_ShouldReturnImage()
    {
        Image? image = await _dbContext.Images.FirstOrDefaultAsync();
        var id = image.Id;

        HttpResponseMessage response = await _client.GetAsync($"api/Image/{id}");

        var responseString = await response.Content.ReadAsStringAsync();
        ImageDto? result = JsonSerializer.Deserialize<ImageDto>(responseString, _jsonOptions);

        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal(result.BlobName, image.BlobName);
        Assert.Equal(result.Id, image.Id);
    }

    [Fact]
    public async Task GetImageById_InvalidData_ShouldReturnError()
    {
        var id = int.MaxValue;

        HttpResponseMessage response = await _client.GetAsync($"api/Image/{id}");

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
