using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Test;
using VictoryCenter.DAL.Data;
using VictoryCenter.IntegrationTests.ControllerTests.Base;
using VictoryCenter.IntegrationTests.Utils;

namespace VictoryCenter.IntegrationTests.ControllerTests;

[Collection("SharedIntegrationTests")]
public class TestControllerTests : IClassFixture<VictoryCenterWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly VictoryCenterDbContext _dbContext;

    public TestControllerTests(IntegrationTestDbFixture fixture)
    {
        _client = fixture.HttpClient;
        _dbContext = fixture.DbContext;
    }

    [Fact]
    public async Task GetAllTestData_ShouldReturnOk()
    {
        var response = await _client.GetAsync("/api/Test/GetAllTestData");
        var responseString = await response.Content.ReadAsStringAsync();
        
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var responseContent = JsonSerializer.Deserialize<IEnumerable<TestDataDto>>(responseString, options);
        
        response.EnsureSuccessStatusCode();
        Assert.NotNull(responseContent);
        Assert.NotEmpty(responseContent);
    }

    [Fact]
    public async Task GetTestDataById_ShouldReturnOk()
    {
        var existingEntity = await _dbContext.TestEntities.FirstOrDefaultAsync();
        
        var response = await _client.GetAsync($"/api/Test/GetTestData/{existingEntity!.Id}");
        var responseString = await response.Content.ReadAsStringAsync();
        
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var responseContent = JsonSerializer.Deserialize<TestDataDto>(responseString, options);
        
        response.EnsureSuccessStatusCode();
        Assert.NotNull(responseContent);
    }

    [Fact]
    public async Task GetTestDataById_ShouldFail_NotFound()
    {
        var response = await _client.GetAsync($"/api/Test/GetTestData/{-1}");
        
        Assert.False(response.IsSuccessStatusCode);
    }

    [Fact]
    public async Task CreateTestData_ShouldReturnOk()
    {
        var createTestDataDto = new CreateTestDataDto() { TestName = "CreateTestName" };
        var serializedDto = JsonSerializer.Serialize(createTestDataDto);
        
        var response = await _client.PostAsync("/api/Test/CreateTestData", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));
        
        Assert.True(response.IsSuccessStatusCode);
    }

    [Fact]
    public async Task UpdateTestData_ShouldReturnOk()
    {
        var existingEntity = await _dbContext.TestEntities.FirstOrDefaultAsync();
        var updateTestDataDto = new UpdateTestDataDto() { Id = existingEntity!.Id, TestName = "UpdateTestName" };
        var serializedDto = JsonSerializer.Serialize(updateTestDataDto);
        
        var response = await _client.PutAsync("/api/Test/UpdateTestData", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));
        
        Assert.True(response.IsSuccessStatusCode);
    }

    [Fact]
    public async Task UpdateTestData_ShouldFail_NotFound()
    {
        var updateTestDataDto = new UpdateTestDataDto() { Id = -1, TestName = "UpdateTestName" };
        var serializedDto = JsonSerializer.Serialize(updateTestDataDto);
        
        var response = await _client.PutAsync("/api/Test/UpdateTestData", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));
        
        Assert.False(response.IsSuccessStatusCode);
    }

    [Fact]
    public async Task DeleteTestData_ShouldReturnOk()
    {
        var existingEntity = await _dbContext.TestEntities.FirstOrDefaultAsync();
        
        var response = await _client.DeleteAsync($"/api/Test/DeleteTestData/{existingEntity!.Id}");
        
        Assert.True(response.IsSuccessStatusCode);
    }

    [Fact]
    public async Task DeleteTestData_ShouldFail_NotFound()
    {
        var response = await _client.DeleteAsync($"/api/Test/DeleteTestData/{-1}");
        
        Assert.False(response.IsSuccessStatusCode);
    }
}
