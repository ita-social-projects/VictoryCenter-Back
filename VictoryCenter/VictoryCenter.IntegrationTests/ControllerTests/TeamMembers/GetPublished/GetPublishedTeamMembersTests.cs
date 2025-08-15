using System.Net;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Public.TeamPage;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.TeamMembers.GetPublished;

public class GetPublishedTeamMembersTests : BaseTestClass
{
    private const string _requestUri = "/api/Team/published";

    public GetPublishedTeamMembersTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
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
        var response = await Fixture.HttpClient.GetAsync(_requestUri);
        var responseString = await response.Content.ReadAsStringAsync();
        var actualCategoryDtos = JsonSerializer.Deserialize<List<CategoryWithPublishedTeamMembersDto>>(responseString, JsonOptions);

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
        return await Fixture.DbContext.Categories
            .AsNoTracking()
            .Include(category => category.TeamMembers
                .Where(teamMember => teamMember.Status == Status.Published)
                .OrderBy(teamMember => teamMember.Priority))
            .Where(category => category.TeamMembers.Any(teamMember => teamMember.Status == Status.Published))
            .ToListAsync();
    }
}
