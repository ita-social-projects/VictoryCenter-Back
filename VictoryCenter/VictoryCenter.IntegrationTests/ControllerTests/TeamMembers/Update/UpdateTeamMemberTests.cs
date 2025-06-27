using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.TeamMembers;
using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Enums;
using VictoryCenter.IntegrationTests.ControllerTests.Base;

namespace VictoryCenter.IntegrationTests.ControllerTests.TeamMembers.Update;

[Collection("SharedIntegrationTests")]
public class UpdateTeamMemberTests
{
    private readonly HttpClient _httpClient;
    private readonly VictoryCenterDbContext _dbContext;

    private readonly JsonSerializerOptions _jsonOptions;

    public UpdateTeamMemberTests(IntegrationTestDbFixture fixture)
    {
        _httpClient = fixture.HttpClient;
        _dbContext = fixture.DbContext;

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("Test Description")]
    public async Task UpdateTeamMember_ValidRequest_ShouldUpdateTeamMember(string? testDescription)
    {
        var existingEntity = await _dbContext.TeamMembers
            .Include(tm => tm.Category)
            .FirstOrDefaultAsync()
            ?? throw new InvalidOperationException("No TeamMember entity exists in the database.");

        var updateTeamMemberDto = new UpdateTeamMemberDto
        {
            Id = existingEntity.Id,
            FirstName = "Test Name",
            CategoryId = existingEntity.Category.Id,
            Status = existingEntity.Status,
            Description = testDescription,
            Email = existingEntity.Email,
        };
        var serializedDto = JsonSerializer.Serialize(updateTeamMemberDto);

        var response = await _httpClient.PutAsync("/api/TeamMembers/", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));
        var responseString = await response.Content.ReadAsStringAsync();
        var responseContent = JsonSerializer.Deserialize<TeamMemberDto>(responseString, _jsonOptions);

        response.EnsureSuccessStatusCode();
        Assert.NotNull(responseContent);
        Assert.Equal(updateTeamMemberDto.Id, responseContent.Id);
        Assert.Equal(updateTeamMemberDto.FirstName, responseContent.FirstName);
        Assert.Equal(updateTeamMemberDto.CategoryId, existingEntity.CategoryId);
        Assert.Equal(updateTeamMemberDto.Status, responseContent.Status);
        Assert.Equal(updateTeamMemberDto.Description, responseContent.Description);
        Assert.Equal(updateTeamMemberDto.Email, responseContent.Email);
    }

    [Fact]
    public async Task UpdateTeamMember_SameInput_ShouldUpdateTeamMember()
    {
        var existingEntity = await _dbContext.TeamMembers
            .Include(tm => tm.Category)
            .FirstOrDefaultAsync()
            ?? throw new InvalidOperationException("No TeamMember entity exists in the database.");

        var updateTeamMemberDto = new UpdateTeamMemberDto
        {
            Id = existingEntity.Id,
            FirstName = existingEntity.FirstName,
            CategoryId = existingEntity.Category.Id,
            Status = existingEntity.Status,
            Description = existingEntity.Description,
            Email = existingEntity.Email
        };
        var serializedDto = JsonSerializer.Serialize(updateTeamMemberDto);

        var response = await _httpClient.PutAsync("/api/TeamMembers/", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));
        var responseString = await response.Content.ReadAsStringAsync();
        var responseContent = JsonSerializer.Deserialize<TeamMemberDto>(responseString, _jsonOptions);

        response.EnsureSuccessStatusCode();
        Assert.NotNull(responseContent);
        Assert.Equal(updateTeamMemberDto.Id, responseContent.Id);
        Assert.Equal(updateTeamMemberDto.FirstName, responseContent.FirstName);
        Assert.Equal(updateTeamMemberDto.CategoryId, existingEntity.CategoryId);
        Assert.Equal(updateTeamMemberDto.Status, responseContent.Status);
        Assert.Equal(updateTeamMemberDto.Description, responseContent.Description);
        Assert.Equal(updateTeamMemberDto.Email, responseContent.Email);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task UpdateTeamMember_InvalidName_ShouldNotUpdateTeamMember(string? testName)
    {
        var existingEntity = await _dbContext.TeamMembers
            .Include(tm => tm.Category)
            .FirstOrDefaultAsync()
            ?? throw new InvalidOperationException("No TeamMember entity exists in the database.");

        var originalFirstName = existingEntity.FirstName;
        var originalCategoryId = existingEntity.CategoryId;
        var originalStatus = existingEntity.Status;
        var originalDescription = existingEntity.Description;
        var originalEmail = existingEntity.Email;
        var originalPriority = existingEntity.Priority;

        var updateTeamMemberDto = new UpdateTeamMemberDto
        {
            Id = existingEntity.Id,
            FirstName = testName,
            CategoryId = existingEntity.Category.Id,
            Status = existingEntity.Status,
            Description = "Test Description",
            Email = existingEntity.Email
        };
        var serializedDto = JsonSerializer.Serialize(updateTeamMemberDto);

        var response = await _httpClient.PutAsync("/api/TeamMembers/", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var reloadedEntity = await _dbContext.TeamMembers
       .AsNoTracking()
       .FirstOrDefaultAsync(tm => tm.Id == existingEntity.Id);
        Assert.NotNull(reloadedEntity);
        Assert.Equal(originalFirstName, reloadedEntity.FirstName);
        Assert.Equal(originalCategoryId, reloadedEntity.CategoryId);
        Assert.Equal(originalStatus, reloadedEntity.Status);
        Assert.Equal(originalDescription, reloadedEntity.Description);
        Assert.Equal(originalEmail, reloadedEntity.Email);
        Assert.Equal(originalPriority, reloadedEntity.Priority);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public async Task UpdateTeamMember_NotFound_ShouldNotUpdateTeamMember(long testId)
    {
        var category = await _dbContext.Categories.FirstOrDefaultAsync() ?? throw new InvalidOperationException("Couldn't setup existing entity");

        var updateTeamMemberDto = new UpdateTeamMemberDto
        {
            Id = testId,
            FirstName = "Test Name",
            CategoryId = category.Id,
            Status = Status.Published,
            Description = "Test Description",
            Email = "test@email.com"
        };
        var serializedDto = JsonSerializer.Serialize(updateTeamMemberDto);

        var response = await _httpClient.PutAsync("/api/TeamMembers/", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
