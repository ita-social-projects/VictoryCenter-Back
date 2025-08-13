using System.Net;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Images;
using VictoryCenter.DAL.Entities;
using VictoryCenter.IntegrationTests.ControllerTests.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.Images.GetById;

[Collection("SharedIntegrationTests")]
public class GetImageByIdTest : IAsyncLifetime
{
    private readonly IntegrationTestDbFixture _fixture;
    private readonly JsonSerializerOptions _jsonOptions;

    public GetImageByIdTest(IntegrationTestDbFixture fixture)
    {
        _fixture = fixture;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task InitializeAsync()
    {
        await _fixture.CreateFreshWebApplication();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task GetImageById_ValidData_ShouldReturnImage()
    {
        Image? image = await _fixture.DbContext.Images.FirstOrDefaultAsync();
        var id = image.Id;

        HttpResponseMessage response = await _fixture.HttpClient.GetAsync($"api/Image/{id}");

        var responseString = await response.Content.ReadAsStringAsync();
        ImageDTO? result = JsonSerializer.Deserialize<ImageDTO>(responseString, _jsonOptions);

        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal(result.BlobName, image.BlobName);
        Assert.Equal(result.Id, image.Id);
    }

    [Fact]
    public async Task GetImageById_InvalidData_ShouldReturnError()
    {
        var id = int.MaxValue;

        HttpResponseMessage response = await _fixture.HttpClient.GetAsync($"api/Image/{id}");

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
