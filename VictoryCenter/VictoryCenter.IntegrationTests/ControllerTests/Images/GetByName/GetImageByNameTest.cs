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

        HttpResponseMessage response = await _client.GetAsync($"api/Image/by-name/{name}");

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

        HttpResponseMessage response = await _client.GetAsync($"api/Image/by-name/{name}");

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
