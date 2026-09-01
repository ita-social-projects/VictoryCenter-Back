using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Admin.TeamCategories;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.TeamCategories.Update;

public class UpdateCategoryTests : BaseTestClass
{
    public UpdateCategoryTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task UpdateCategory_ShouldUpdateCategory()
    {
        var existingEntity = await Fixture.DbContext.TeamCategories.FirstOrDefaultAsync();
        var updateTeamCategoryDto = new UpdateTeamCategoryDto
        {
            Name = "Test Category",
            Description = "Test Description",
        };
        var serializedDto = JsonSerializer.Serialize(updateTeamCategoryDto);

        var response = await Fixture.HttpClient.PutAsync($"api/teamcategories/{existingEntity!.Id}", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));
        var responseString = await response.Content.ReadAsStringAsync();
        var responseContent = JsonSerializer.Deserialize<TeamCategoryDto>(responseString, JsonOptions);

        response.EnsureSuccessStatusCode();
        Assert.NotNull(responseContent);
        Assert.Equal(existingEntity.Id, responseContent.Id);
        Assert.Equal(updateTeamCategoryDto.Name, responseContent.Name);
        Assert.Equal(updateTeamCategoryDto.Description, responseContent.Description);
    }

    [Fact]
    public async Task UpdateCategory_ShouldUpdateCategory_SameInput()
    {
        var existingEntity = await Fixture.DbContext.TeamCategories.FirstOrDefaultAsync();
        var updateTeamCategoryDto = new UpdateTeamCategoryDto
        {
            Name = existingEntity!.Name,
            Description = existingEntity.Description,
        };
        var serializedDto = JsonSerializer.Serialize(updateTeamCategoryDto);

        var response = await Fixture.HttpClient.PutAsync($"api/teamcategories/{existingEntity.Id}", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));
        var responseString = await response.Content.ReadAsStringAsync();
        var responseContent = JsonSerializer.Deserialize<TeamCategoryDto>(responseString, JsonOptions);

        response.EnsureSuccessStatusCode();
        Assert.NotNull(responseContent);
        Assert.Equal(existingEntity.Id, responseContent.Id);
        Assert.Equal(updateTeamCategoryDto.Name, responseContent.Name);
        Assert.Equal(updateTeamCategoryDto.Description, responseContent.Description);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task UpdateCategory_ShouldNotUpdateCategory_InvalidName(string? testName)
    {
        var existingEntity = await Fixture.DbContext.TeamCategories.FirstOrDefaultAsync();
        var updateTeamCategoryDto = new UpdateTeamCategoryDto
        {
            Name = testName!,
            Description = "Test Description",
        };
        var serializedDto = JsonSerializer.Serialize(updateTeamCategoryDto);

        var response = await Fixture.HttpClient.PutAsync($"api/teamcategories/{existingEntity!.Id}", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public async Task UpdateCategory_ShouldNotUpdateCategory_NotFound(long testId)
    {
        var updateTeamCategoryDto = new UpdateTeamCategoryDto
        {
            Name = "Test Category",
            Description = "Test Description",
        };
        var serializedDto = JsonSerializer.Serialize(updateTeamCategoryDto);

        var response = await Fixture.HttpClient.PutAsync($"api/teamcategories/{testId}", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Theory]
    [InlineData(" Test Category")]
    [InlineData("Test Category ")]
    [InlineData(" Test Category ")]
    public async Task UpdateCategory_ShouldNotUpdateCategory_NameHasLeadingOrTrailingSpace(string paddedName)
    {
        var existingEntity = await Fixture.DbContext.TeamCategories.FirstOrDefaultAsync();
        var updateTeamCategoryDto = new UpdateTeamCategoryDto
        {
            Name = paddedName,
            Description = "Test Description",
        };
        var serializedDto = JsonSerializer.Serialize(updateTeamCategoryDto);

        var response = await Fixture.HttpClient.PutAsync($"api/teamcategories/{existingEntity!.Id}", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData("Test  Category")]
    [InlineData("Test   Category")]
    [InlineData("Test Category  Extra")]
    public async Task UpdateCategory_ShouldNotUpdateCategory_NameHasMultipleConsecutiveSpaces(string paddedName)
    {
        var existingEntity = await Fixture.DbContext.TeamCategories.FirstOrDefaultAsync();
        var updateTeamCategoryDto = new UpdateTeamCategoryDto
        {
            Name = paddedName,
            Description = "Test Description",
        };
        var serializedDto = JsonSerializer.Serialize(updateTeamCategoryDto);

        var response = await Fixture.HttpClient.PutAsync($"api/teamcategories/{existingEntity!.Id}", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
