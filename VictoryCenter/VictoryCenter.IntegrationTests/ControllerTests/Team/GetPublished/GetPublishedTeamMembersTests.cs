using System.Net;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Public.TeamPage;
using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.IntegrationTests.ControllerTests.Base;

namespace VictoryCenter.IntegrationTests.ControllerTests.Team.GetPublished;

[Collection("SharedIntegrationTests")]
public class GetPublishedTeamMembersTests
{
    private const string _requestUri = "/api/Team/published";

    private readonly HttpClient _httpClient;
    private readonly VictoryCenterDbContext _dbContext;
    private readonly JsonSerializerOptions _jsonOptions;

    public GetPublishedTeamMembersTests(IntegrationTestDbFixture fixture)
    {
        _httpClient = fixture.HttpClient;
        _dbContext = fixture.DbContext;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    [Fact]
    public async Task GetPublicTeamMembers_ShouldReturnOnlyPublishedMembersGroupedByCategory()
    {
        // Arrange
        var categoriesWithPublishedMembers = await GetCategoriesWithPublishedMembersAsync();

        if (!categoriesWithPublishedMembers.Any())
        {
            Assert.Fail("No categories found in the database for GetPublicTeamMembers test.");
        }

        // Act
        var response = await _httpClient.GetAsync(_requestUri);
        var responseString = await response.Content.ReadAsStringAsync();
        var actualCategoryDtos = JsonSerializer.Deserialize<List<CategoryWithPublishedTeamMembersDto>>(responseString, _jsonOptions);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(actualCategoryDtos);
        Assert.NotEmpty(actualCategoryDtos);
        Assert.Equal(categoriesWithPublishedMembers.Count, actualCategoryDtos.Count);

        foreach (var actualCategoryDto in actualCategoryDtos)
        {
            Assert.NotNull(actualCategoryDto.CategoryName);
            Assert.NotNull(actualCategoryDto.TeamMembers);

            // At least one member should be present in each category
            Assert.NotEmpty(actualCategoryDto.TeamMembers);

            var expectedCategory = categoriesWithPublishedMembers.First(c => c.Id == actualCategoryDto.Id);
            var expectedMembers = expectedCategory.TeamMembers.ToList();

            Assert.Equal(expectedMembers.Count, actualCategoryDto.TeamMembers.Count);

            // Validate each member in the category
            for (var i = 0; i < expectedMembers.Count; i++)
            {
                var actualMemberDto = actualCategoryDto.TeamMembers[i];
                var expectedMember = expectedMembers[i];

                Assert.Equal(Status.Published, expectedMember.Status);
                Assert.Equal(expectedMember.Id, actualMemberDto.Id);
                Assert.Equal(expectedMember.FullName, actualMemberDto.FullName);
                Assert.Equal(expectedMember.Description, actualMemberDto.Description);
            }
        }
    }

    private async Task<List<Category>> GetCategoriesWithPublishedMembersAsync()
    {
        return await _dbContext.Categories
            .AsNoTracking()
            .Include(category => category.TeamMembers
                .Where(teamMember => teamMember.Status == Status.Published)
                .OrderBy(teamMember => teamMember.Priority))
            .Where(category => category.TeamMembers.Any(teamMember => teamMember.Status == Status.Published))
            .ToListAsync();
    }
}
