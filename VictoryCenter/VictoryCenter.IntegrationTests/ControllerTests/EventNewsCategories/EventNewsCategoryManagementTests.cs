using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Admin.EventNewsCategories;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.EventNewsCategories;

public class EventNewsCategoryManagementTests : BaseTestClass
{
    public EventNewsCategoryManagementTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task CategoryManagement_ShouldSupportAuthorizedCrud()
    {
        var suffix = Guid.NewGuid().ToString("N")[..8];
        var categoryName = $"Cat{suffix}";
        var renamedCategory = $"Ren{suffix}";

        var createResponse = await Fixture.HttpClient.PostAsJsonAsync(
            "/api/EventNewsCategories",
            new CreateEventNewsCategoryDto { Name = $"  {categoryName}  " });

        Assert.Equal(HttpStatusCode.OK, createResponse.StatusCode);
        var category = await createResponse.Content.ReadFromJsonAsync<AdminEventNewsCategoryDto>();
        Assert.NotNull(category);
        Assert.Equal(categoryName, category.Name);

        var duplicateResponse = await Fixture.HttpClient.PostAsJsonAsync(
            "/api/EventNewsCategories",
            new CreateEventNewsCategoryDto { Name = categoryName });
        Assert.Equal(HttpStatusCode.BadRequest, duplicateResponse.StatusCode);

        var getResponse = await Fixture.HttpClient.GetAsync("/api/EventNewsCategories");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        var categories = await getResponse.Content.ReadFromJsonAsync<List<AdminEventNewsCategoryDto>>();
        Assert.Contains(categories!, item => item.Id == category.Id);

        var updateResponse = await Fixture.HttpClient.PutAsJsonAsync(
            $"/api/EventNewsCategories/{category.Id}",
            new UpdateEventNewsCategoryDto { Name = $"  {renamedCategory}  " });
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
        var updatedCategory = await updateResponse.Content.ReadFromJsonAsync<AdminEventNewsCategoryDto>();
        Assert.NotNull(updatedCategory);
        Assert.Equal(renamedCategory, updatedCategory.Name);

        var deleteResponse = await Fixture.HttpClient.DeleteAsync($"/api/EventNewsCategories/{category.Id}");
        Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);
        Assert.False(await Fixture.DbContext.EventNewsCategories
            .AsNoTracking()
            .AnyAsync(item => item.Id == category.Id));
    }

    [Fact]
    public async Task CategoryManagement_ShouldReturnNotFound_ForMissingCategory()
    {
        const long missingId = long.MaxValue;

        var updateResponse = await Fixture.HttpClient.PutAsJsonAsync(
            $"/api/EventNewsCategories/{missingId}",
            new UpdateEventNewsCategoryDto { Name = "Missing" });
        var deleteResponse = await Fixture.HttpClient.DeleteAsync($"/api/EventNewsCategories/{missingId}");

        Assert.Equal(HttpStatusCode.NotFound, updateResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, deleteResponse.StatusCode);
    }

    [Fact]
    public async Task CategoryMutations_ShouldRequireAuthorization()
    {
        using var anonymousClient = Fixture.Factory.CreateClient();

        var getResponse = await anonymousClient.GetAsync("/api/EventNewsCategories");
        var createResponse = await anonymousClient.PostAsJsonAsync(
            "/api/EventNewsCategories",
            new CreateEventNewsCategoryDto { Name = "Unauthorized" });

        Assert.Equal(HttpStatusCode.Unauthorized, getResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, createResponse.StatusCode);
    }

    [Fact]
    public async Task SwaggerDocument_ShouldBeGenerated_WithCategoryLocalizationDtos()
    {
        var response = await Fixture.HttpClient.GetAsync("/swagger/v1/swagger.json");

        Assert.True(
            response.IsSuccessStatusCode,
            $"Swagger returned {response.StatusCode}: {await response.Content.ReadAsStringAsync()}");
    }

    [Fact]
    public async Task DeleteCategory_ShouldReturnBadRequest_WhenCategoryIsInUse()
    {
        var categoryId = await Fixture.DbContext.EventNews
            .AsNoTracking()
            .SelectMany(eventNews => eventNews.Categories)
            .Select(category => category.Id)
            .FirstAsync();

        var response = await Fixture.HttpClient.DeleteAsync($"/api/EventNewsCategories/{categoryId}");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
