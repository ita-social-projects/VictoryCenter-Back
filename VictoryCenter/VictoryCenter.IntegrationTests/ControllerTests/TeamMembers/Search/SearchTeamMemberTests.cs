using System.Net;
using System.Text.Json;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VictoryCenter.BLL.DTOs.TeamMembers;
using VictoryCenter.DAL.Data;
using VictoryCenter.IntegrationTests.ControllerTests.Base;

namespace VictoryCenter.IntegrationTests.ControllerTests.TeamMembers.Search;

[Collection("SharedIntegrationTests")]
public class SearchTeamMemberTests
{
    private readonly HttpClient _httpClient;
    private readonly VictoryCenterDbContext _dbContext;
    private readonly IMapper _mapper;

    public SearchTeamMemberTests(IntegrationTestDbFixture fixture)
    {
        _httpClient = fixture.HttpClient;
        _dbContext = fixture.DbContext;
        _mapper = fixture.Factory.Services.GetService<IMapper>();
    }

    [Fact]
    public async Task SearchTeamMembers_ValidRequest_ShouldReturnOk()
    {
        // Arrange
        string fullName = $"FirstName1 LastName1";
        var expectedTeamMembers = (await _dbContext.TeamMembers
            .Include(tm => tm.Category)
            .Where(tm => tm.FullName.StartsWith(fullName))
            .ToListAsync())
            .Select(_mapper.Map<TeamMemberDto>)
            .ToList();

        // Act
        var response = await _httpClient.GetAsync($"api/TeamMembers/search?fullname={fullName}");
        var responseString = await response.Content.ReadAsStringAsync();

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var responseContent = JsonSerializer.Deserialize<List<TeamMemberDto>>(responseString, options);

        // Assert
        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(responseContent);
        Assert.Equal(responseContent, expectedTeamMembers);
    }

    [Fact]
    public async Task SearchTeamMembers_InvalidRequest_ShouldReturnBadRequest()
    {
        // Arrange
        string fullName = $"";

        // Act
        var response = await _httpClient.GetAsync($"api/TeamMembers/search?fullname={fullName}");

        // Assert
        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
