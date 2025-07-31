using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using VictoryCenter.BLL.DTOs.TeamMembers;
using VictoryCenter.BLL.Interfaces.BlobStorage;
using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.IntegrationTests.ControllerTests.Base;
using VictoryCenter.IntegrationTests.Utils.Seeder;
using VictoryCenter.IntegrationTests.Utils.Seeder.CategoriesSeeder;
using VictoryCenter.IntegrationTests.Utils.Seeder.Seeders;

namespace VictoryCenter.IntegrationTests.ControllerTests.TeamMembers.Update;

[Collection("SharedIntegrationTests")]
public class UpdateTeamMemberTests : IAsyncLifetime
{
    private readonly VictoryCenterDbContext _dbContext;
    private readonly HttpClient _httpClient;
    private readonly SeederManager _seederManager;
    private readonly IntegrationTestDbFixture _fixture;
    private readonly IBlobService _blobService;

    private readonly JsonSerializerOptions _jsonOptions;

    public UpdateTeamMemberTests(IntegrationTestDbFixture fixture)
    {
        _httpClient = fixture.HttpClient;
        _dbContext = fixture.DbContext;
        _fixture = fixture;
        _blobService = fixture.BlobService;

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        _seederManager = fixture.SeederManager ?? throw new InvalidOperationException("SeederManager is not registered in the service collection.");
    }

    public async Task InitializeAsync()
    {
        await _seederManager.DisposeAllAsync();
        await _seederManager.SeedAllAsync();

        _seederManager.ClearSeeders();
        _seederManager.ConfigureSeeders(
            new CategoriesSeeder(_fixture.DbContext, _fixture.Factory.Services.GetRequiredService<ILogger<CategoriesSeeder>>(), _blobService),
            new TeamMemberUpdateSeeder(_fixture.DbContext, _fixture.Factory.Services.GetRequiredService<ILogger<TeamMemberUpdateSeeder>>(), _blobService));

        if (!await _fixture.SeederManager.SeedAllAsync())
        {
            throw new InvalidOperationException("Seeding failed for CustomDataTests");
        }
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("Test Description")]
    public async Task UpdateTeamMember_ValidRequest_ShouldUpdateTeamMember(string? testDescription)
    {
        TeamMember existingEntity = await _dbContext.TeamMembers
                                        .Include(tm => tm.Category)
                                        .LastOrDefaultAsync()
                                    ?? throw new InvalidOperationException(
                                        "No TeamMember entity exists in the database.");

        var updateTeamMemberDto = new UpdateTeamMemberDto
        {
            FullName = "Test Name",
            CategoryId = existingEntity.Category.Id,
            Status = existingEntity.Status,
            Description = testDescription,
            Email = existingEntity.Email
        };
        var serializedDto = JsonSerializer.Serialize(updateTeamMemberDto);

        HttpResponseMessage response = await _httpClient.PutAsync($"/api/TeamMembers/{existingEntity.Id}", new StringContent(
                serializedDto, Encoding.UTF8, "application/json"));
        var responseString = await response.Content.ReadAsStringAsync();
        TeamMemberDto? responseContent = JsonSerializer.Deserialize<TeamMemberDto>(responseString, _jsonOptions);

        response.EnsureSuccessStatusCode();
        Assert.NotNull(responseContent);
        Assert.Equal(existingEntity.Id, responseContent.Id);
        Assert.Equal(updateTeamMemberDto.FullName, responseContent.FullName);
        Assert.Equal(updateTeamMemberDto.CategoryId, existingEntity.CategoryId);
        Assert.Equal(updateTeamMemberDto.Status, responseContent.Status);
        Assert.Equal(updateTeamMemberDto.Description, responseContent.Description);
        Assert.Equal(updateTeamMemberDto.Email, responseContent.Email);
    }

    [Fact]
    public async Task UpdateTeamMember_SameInput_ShouldUpdateTeamMember()
    {
        TeamMember existingEntity = await _dbContext.TeamMembers
                                        .Include(tm => tm.Category)
                                        .LastOrDefaultAsync()
                                    ?? throw new InvalidOperationException(
                                        "No TeamMember entity exists in the database.");

        var updateTeamMemberDto = new UpdateTeamMemberDto
        {
            FullName = existingEntity.FullName,
            CategoryId = existingEntity.Category.Id,
            Status = existingEntity.Status,
            Description = existingEntity.Description,
            Email = existingEntity.Email
        };
        var serializedDto = JsonSerializer.Serialize(updateTeamMemberDto);

        HttpResponseMessage response = await _httpClient.PutAsync($"/api/TeamMembers/{existingEntity.Id}", new StringContent(
                serializedDto, Encoding.UTF8, "application/json"));
        var responseString = await response.Content.ReadAsStringAsync();
        TeamMemberDto? responseContent = JsonSerializer.Deserialize<TeamMemberDto>(responseString, _jsonOptions);

        response.EnsureSuccessStatusCode();
        Assert.NotNull(responseContent);
        Assert.Equal(existingEntity.Id, responseContent.Id);
        Assert.Equal(updateTeamMemberDto.FullName, responseContent.FullName);
        Assert.Equal(updateTeamMemberDto.CategoryId, existingEntity.CategoryId);
        Assert.Equal(updateTeamMemberDto.Status, responseContent.Status);
        Assert.Equal(updateTeamMemberDto.Description, responseContent.Description);
        Assert.Equal(updateTeamMemberDto.Email, responseContent.Email);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task UpdateTeamMember_InvalidFullName_ShouldNotUpdateTeamMember(string? testName)
    {
        TeamMember existingEntity = await _dbContext.TeamMembers
                                        .Include(tm => tm.Category)
                                        .FirstOrDefaultAsync()
                                    ?? throw new InvalidOperationException(
                                        "No TeamMember entity exists in the database.");

        var originalFullName = existingEntity.FullName;
        var originalCategoryId = existingEntity.CategoryId;
        Status originalStatus = existingEntity.Status;
        var originalDescription = existingEntity.Description;
        var originalEmail = existingEntity.Email;
        var originalPriority = existingEntity.Priority;

        var updateTeamMemberDto = new UpdateTeamMemberDto
        {
            FullName = testName,
            CategoryId = existingEntity.Category.Id,
            Status = existingEntity.Status,
            Description = "Test Description",
            Email = existingEntity.Email
        };
        var serializedDto = JsonSerializer.Serialize(updateTeamMemberDto);

        HttpResponseMessage response = await _httpClient.PutAsync($"/api/TeamMembers/{existingEntity.Id}", new StringContent(
                serializedDto, Encoding.UTF8, "application/json"));

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        TeamMember? reloadedEntity = await _dbContext.TeamMembers
            .AsNoTracking()
            .FirstOrDefaultAsync(tm => tm.Id == existingEntity.Id);
        Assert.NotNull(reloadedEntity);
        Assert.Equal(originalFullName, reloadedEntity.FullName);
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
        Category category = await _dbContext.Categories.FirstOrDefaultAsync() ??
                            throw new InvalidOperationException("Couldn't setup existing entity");

        var updateTeamMemberDto = new UpdateTeamMemberDto
        {
            FullName = "Test Name",
            CategoryId = category.Id,
            Status = Status.Published,
            Description = "Test Description",
            Email = "test@email.com"
        };
        var serializedDto = JsonSerializer.Serialize(updateTeamMemberDto);

        HttpResponseMessage response = await _httpClient.PutAsync($"/api/TeamMembers/{testId}", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateTeamMember_InvalidCategoryId_ShouldNotUpdateTeamMember()
    {
        Category category = await _dbContext.Categories.FirstOrDefaultAsync() ??
                            throw new InvalidOperationException("Couldn't setup existing entity");

        var wrongId = int.MaxValue;
        var updateTeamMemberDto = new UpdateTeamMemberDto
        {
            FullName = "Test Name",
            CategoryId = wrongId,
            Status = Status.Published,
            Description = "Test Description",
            Email = "test@email.com"
        };
        var serializedDto = JsonSerializer.Serialize(updateTeamMemberDto);

        HttpResponseMessage response = await _httpClient.PutAsync($"/api/TeamMembers/{wrongId}", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
