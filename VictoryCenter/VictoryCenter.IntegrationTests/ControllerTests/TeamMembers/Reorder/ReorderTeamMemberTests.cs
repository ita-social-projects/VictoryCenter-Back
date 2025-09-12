using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Admin.TeamMembers;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.TeamMembers.Reorder;

public class ReorderTeamMemberTests : BaseTestClass
{
    public ReorderTeamMemberTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task ReorderTeamMembers_ValidRequest_ShouldReturnOk()
    {
        // Arrange
        var categoryId = (await Fixture.DbContext.Categories.FirstAsync()).Id;

        var originalTeamMemberIdsOrder = await Fixture.DbContext.TeamMembers
            .Where(x => x.CategoryId == categoryId)
            .OrderBy(x => x.Priority)
            .Select(x => x.Id)
            .ToListAsync();

        // Reverse the order for testing
        var reverseTeamMembersIdsOrder = originalTeamMemberIdsOrder.ToList();
        reverseTeamMembersIdsOrder.Reverse();

        var reorderDto = new ReorderTeamMembersDto
        {
            CategoryId = categoryId,
            OrderedIds = reverseTeamMembersIdsOrder
        };

        var serializedDto = JsonSerializer.Serialize(reorderDto);

        // Act
        var response = await Fixture.HttpClient.PutAsync("api/TeamMembers/reorder", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var updatedTeamMembersIdsOrder = await Fixture.DbContext.TeamMembers
            .Where(x => x.CategoryId == categoryId)
            .OrderBy(x => x.Priority)
            .Select(x => x.Id)
            .ToListAsync();

        Assert.Equal(reverseTeamMembersIdsOrder, updatedTeamMembersIdsOrder);
    }

    [Fact]
    public async Task ReorderTeamMembers_PartialTeamMemberIds_ShouldReorderPartialMembers()
    {
        // Arrange
        var categoryId = (await Fixture.DbContext.Categories.FirstAsync()).Id;

        var allTeamMemberIdsOrder = await Fixture.DbContext.TeamMembers
            .Where(x => x.CategoryId == categoryId)
            .OrderBy(x => x.Priority)
            .Select(x => x.Id)
            .ToListAsync();

        // Take only first 2 members for partial reorder
        var partialIds = allTeamMemberIdsOrder.Take(2).Reverse().ToList();

        var reorderDto = new ReorderTeamMembersDto
        {
            CategoryId = categoryId,
            OrderedIds = partialIds
        };

        var serializedDto = JsonSerializer.Serialize(reorderDto);

        // Act
        var response = await Fixture.HttpClient.PutAsync("api/TeamMembers/reorder", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Verify database changes
        var updatedMembers = await Fixture.DbContext.TeamMembers
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
        var response = await Fixture.HttpClient.PutAsync("api/TeamMembers/reorder", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ReorderTeamMembers_EmptyOrderedIds_ShouldReturnBadRequest()
    {
        // Arrange
        var categoryId = (await Fixture.DbContext.Categories.FirstAsync()).Id;

        var reorderDto = new ReorderTeamMembersDto
        {
            CategoryId = categoryId,
            OrderedIds = []
        };
        var serializedDto = JsonSerializer.Serialize(reorderDto);

        // Act
        var response = await Fixture.HttpClient.PutAsync("api/TeamMembers/reorder", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ReorderTeamMembers_DuplicateIds_ShouldReturnBadRequest()
    {
        // Arrange
        var categoryId = (await Fixture.DbContext.Categories.FirstAsync()).Id;

        var existingIds = await Fixture.DbContext.TeamMembers
            .Where(x => x.CategoryId == categoryId)
            .Select(x => x.Id)
            .Take(2)
            .ToListAsync();

        if (existingIds.Count < 2)
        {
            Assert.Fail("Not enough team members in category for duplicate test");
        }

        var reorderDto = new ReorderTeamMembersDto
        {
            CategoryId = categoryId,
            OrderedIds = [existingIds[0], existingIds[1], existingIds[1]]
        };
        var serializedDto = JsonSerializer.Serialize(reorderDto);

        // Act
        var response = await Fixture.HttpClient.PutAsync("api/TeamMembers/reorder", new StringContent(
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
        var categoryId = (await Fixture.DbContext.Categories.FirstAsync()).Id;

        var existingIds = await Fixture.DbContext.TeamMembers
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
        var response = await Fixture.HttpClient.PutAsync("api/TeamMembers/reorder", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ReorderTeamMembers_NonExistentMemberIds_ShouldReturnBadRequest()
    {
        // Arrange
        var categoryId = (await Fixture.DbContext.Categories.FirstAsync()).Id;

        var existingIds = await Fixture.DbContext.TeamMembers
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
        var response = await Fixture.HttpClient.PutAsync("api/TeamMembers/reorder", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ReorderTeamMembers_MemberFromDifferentCategory_ShouldReturnBadRequest()
    {
        // Arrange
        var categories = await Fixture.DbContext.Categories.ToListAsync();

        if (categories.Count < 2)
        {
            Assert.Fail("Need at least 2 categories for this test");
        }

        var firstCategoryId = categories[0].Id;
        var secondCategoryId = categories[1].Id;

        var firstCategoryMemberIds = await Fixture.DbContext.TeamMembers
            .Where(x => x.CategoryId == firstCategoryId)
            .Select(x => x.Id)
            .Take(2)
            .ToListAsync();

        var secondCategoryMemberIds = await Fixture.DbContext.TeamMembers
            .Where(x => x.CategoryId == secondCategoryId)
            .Select(x => x.Id)
            .ToListAsync();

        if (!firstCategoryMemberIds.Any() || !secondCategoryMemberIds.Any())
        {
            Assert.Fail("Categories don't have enough members for this test");
        }

        var reorderDto = new ReorderTeamMembersDto
        {
            CategoryId = firstCategoryId,
            OrderedIds = [.. firstCategoryMemberIds, secondCategoryMemberIds[0]]
        };

        var serializedDto = JsonSerializer.Serialize(reorderDto);

        // Act
        var response = await Fixture.HttpClient.PutAsync("api/TeamMembers/reorder", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
