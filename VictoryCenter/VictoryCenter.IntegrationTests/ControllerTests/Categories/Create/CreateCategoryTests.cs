using System.Net;
using System.Text;
using System.Text.Json;
using VictoryCenter.BLL.DTOs.Admin.Categories;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.Categories.Create;

public class CreateCategoryTests : BaseTestClass
{
    public CreateCategoryTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("Test Description")]
    public async Task CreateCategory_ShouldCreateCategory(string? testDescription)
    {
        var createCategoryDto = new CreateCategoryDto
        {
            Name = "Test Category",
            Description = testDescription,
        };
        var serializedDto = JsonSerializer.Serialize(createCategoryDto);

        var response = await Fixture.HttpClient.PostAsync("api/categories", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));
        var responseString = await response.Content.ReadAsStringAsync();
        var responseContent = JsonSerializer.Deserialize<CategoryDto>(responseString, JsonOptions);

        response.EnsureSuccessStatusCode();
        Assert.NotNull(responseContent);
        Assert.Equal(createCategoryDto.Name, responseContent.Name);
        Assert.Equal(createCategoryDto.Description, responseContent.Description);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task CreateCategory_ShouldNotCreateCategory_InvalidName(string? testName)
    {
        var createCategoryDto = new CreateCategoryDto
        {
            Name = testName!,
            Description = "Test Description",
        };
        var serializedDto = JsonSerializer.Serialize(createCategoryDto);

        var response = await Fixture.HttpClient.PostAsync("api/categories", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
