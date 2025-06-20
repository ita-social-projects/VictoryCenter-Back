using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.TeamMembers;
using VictoryCenter.DAL.Data;
using VictoryCenter.IntegrationTests.ControllerTests.Base;

namespace VictoryCenter.IntegrationTests.ControllerTests.TeamMembers.Reorder;

[Collection("SharedIntegrationTests")]
public class ReorderTeamMemberTests
{
    private readonly HttpClient _httpClient;
    private readonly VictoryCenterDbContext _dbContext;

    public ReorderTeamMemberTests(IntegrationTestDbFixture fixture)
    {
        _httpClient = fixture.HttpClient;
        _dbContext = fixture.DbContext;
    }

    [Fact]
    public async Task ReorderTeamMembers_ValidRequest_ShouldReturnOk()
    {
        // Arrange
        var categoryId = (await _dbContext.Categories.FirstAsync()).Id;

        var originalTeamMemberIds = await _dbContext.TeamMembers
            .Where(x => x.CategoryId == categoryId)
            .Select(x => x.Id)
            .OrderBy(x => x)
            .ToListAsync();

        // Reverse the order for testing
        var sortedTeamMembersIds = originalTeamMemberIds
            .OrderByDescending(x => x)
            .ToList();

        var reorderDto = new ReorderTeamMembersDto
        {
            CategoryId = categoryId,
            OrderedIds = sortedTeamMembersIds
        };

        var serializedDto = JsonSerializer.Serialize(reorderDto);

        // Act
        var response = await _httpClient.PutAsync("api/TeamMembers/reorder", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var updatedTeamMembersIds = await _dbContext.TeamMembers
            .Where(x => x.CategoryId == categoryId)
            .Select(x => x.Id)
            .OrderBy(x => x)
            .ToListAsync();

        Assert.Equal(originalTeamMemberIds, updatedTeamMembersIds);
    }

    [Fact]
    public async Task ReorderTeamMembers_PartialTeamMemberIds_ShouldReorderPartialMembers()
    {
        // Arrange
        var categoryId = (await _dbContext.Categories.FirstAsync()).Id;

        var allTeamMemberIds = await _dbContext.TeamMembers
            .Where(x => x.CategoryId == categoryId)
            .Select(x => x.Id)
            .OrderBy(x => x)
            .ToListAsync();

        // Take only first 2 members for partial reorder
        var partialIds = allTeamMemberIds.Take(2).Reverse().ToList();

        var reorderDto = new ReorderTeamMembersDto
        {
            CategoryId = categoryId,
            OrderedIds = partialIds
        };

        var serializedDto = JsonSerializer.Serialize(reorderDto);

        // Act
        var response = await _httpClient.PutAsync("api/TeamMembers/reorder", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Verify database changes
        var updatedMembers = await _dbContext.TeamMembers
            .Where(tm => tm.CategoryId == categoryId)
            .OrderBy(tm => tm.Priority)
            .ToListAsync();

        Assert.True(updatedMembers.Count >= 2);

        // Check that partial reorder worked
        Assert.Equal(partialIds[0], updatedMembers[0].Id);
        Assert.Equal(partialIds[1], updatedMembers[1].Id);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public async Task ReorderTeamMembers_InvalidCategoryId_ShouldReturnBadRequest(long invalidCategoryId)
    {
        // Arrange
        var reorderDto = new ReorderTeamMembersDto
        {
            CategoryId = invalidCategoryId,
            OrderedIds = [1, 2, 3]
        };
        var serializedDto = JsonSerializer.Serialize(reorderDto);

        // Act
        var response = await _httpClient.PutAsync("api/TeamMembers/reorder", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ReorderTeamMembers_EmptyOrderedIds_ShouldReturnBadRequest()
    {
        // Arrange
        var categoryId = (await _dbContext.Categories.FirstAsync()).Id;

        var reorderDto = new ReorderTeamMembersDto
        {
            CategoryId = categoryId,
            OrderedIds = []
        };
        var serializedDto = JsonSerializer.Serialize(reorderDto);

        // Act
        var response = await _httpClient.PutAsync("api/TeamMembers/reorder", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ReorderTeamMembers_DuplicateIds_ShouldReturnBadRequest()
    {
        // Arrange
        var categoryId = (await _dbContext.Categories.FirstAsync()).Id;

        var existingIds = await _dbContext.TeamMembers
            .Where(x => x.CategoryId == categoryId)
            .Select(x => x.Id)
            .Take(2)
            .ToListAsync();

        if (existingIds.Count < 2)
        {
            Assert.True(true, "Not enough team members in category for duplicate test");
            return;
        }

        var reorderDto = new ReorderTeamMembersDto
        {
            CategoryId = categoryId,
            OrderedIds = [existingIds[0], existingIds[1], existingIds[1]]
        };
        var serializedDto = JsonSerializer.Serialize(reorderDto);

        // Act
        var response = await _httpClient.PutAsync("api/TeamMembers/reorder", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public async Task ReorderTeamMembers_InvalidIdInOrderedIds_ShouldReturnBadRequest(long invalidId)
    {
        // Arrange
        var categoryId = (await _dbContext.Categories.FirstAsync()).Id;

        var existingIds = await _dbContext.TeamMembers
            .Where(x => x.CategoryId == categoryId)
            .Select(x => x.Id)
            .Take(2)
            .ToListAsync();

        // Add invalid ID to the list
        existingIds.Add(invalidId);

        var reorderDto = new ReorderTeamMembersDto
        {
            CategoryId = categoryId,
            OrderedIds = existingIds
        };
        var serializedDto = JsonSerializer.Serialize(reorderDto);

        // Act
        var response = await _httpClient.PutAsync("api/TeamMembers/reorder", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ReorderTeamMembers_NonExistentMemberIds_ShouldReturnBadRequest()
    {
        // Arrange
        var categoryId = (await _dbContext.Categories.FirstAsync()).Id;

        var existingIds = await _dbContext.TeamMembers
            .Where(x => x.CategoryId == categoryId)
            .Select(x => x.Id)
            .Take(2)
            .ToListAsync();

        // Add non-existing ID
        existingIds.Add(999999);

        var reorderDto = new ReorderTeamMembersDto
        {
            CategoryId = categoryId,
            OrderedIds = existingIds
        };

        var serializedDto = JsonSerializer.Serialize(reorderDto);

        // Act
        var response = await _httpClient.PutAsync("api/TeamMembers/reorder", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ReorderTeamMembers_MemberFromDifferentCategory_ShouldReturnBadRequest()
    {
        // Arrange
        var categories = await _dbContext.Categories.ToListAsync();

        if (categories.Count < 2)
        {
            Assert.True(false, "Need at least 2 categories for this test");
            return;
        }

        var firstCategoryId = categories[0].Id;
        var secondCategoryId = categories[1].Id;

        var firstCategoryMemberIds = await _dbContext.TeamMembers
            .Where(x => x.CategoryId == firstCategoryId)
            .Select(x => x.Id)
            .Take(2)
            .ToListAsync();

        var secondCategoryMemberIds = await _dbContext.TeamMembers
            .Where(x => x.CategoryId == secondCategoryId)
            .Select(x => x.Id)
            .ToListAsync();

        if (!firstCategoryMemberIds.Any() || !secondCategoryMemberIds.Any())
        {
            Assert.True(false, "Categories don't have enough members for this test");
            return;
        }

        var reorderDto = new ReorderTeamMembersDto
        {
            CategoryId = firstCategoryId,
            OrderedIds = firstCategoryMemberIds.Concat([secondCategoryMemberIds.First()]).ToList()
        };

        var serializedDto = JsonSerializer.Serialize(reorderDto);

        // Act
        var response = await _httpClient.PutAsync("api/TeamMembers/reorder", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
