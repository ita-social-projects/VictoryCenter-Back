using System.Net;
using System.Text;
using Newtonsoft.Json;
using VictoryCenter.BLL.DTOs.Admin.HypotherapyProgramCategories;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.HypotherapyProgramCategories.Create;

public class CreateHypotherapyProgramCategoryTests : BaseTestClass
{
    public CreateHypotherapyProgramCategoryTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task ProgramCategory_ShouldCreateProgramCategory()
    {
        var createProgramCategoryDto = new CreateHypotherapyProgramCategoryDto
        {
            Name = "NewName1"
        };
        var serializedDto = JsonConvert.SerializeObject(createProgramCategoryDto);

        HttpResponseMessage response = await Fixture.HttpClient.PostAsync("/api/HypotherapyProgramCategory/", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();

        HypotherapyProgramCategoryDto? responseContent = JsonConvert.DeserializeObject<HypotherapyProgramCategoryDto>(responseString);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(responseContent);
        Assert.Equal(createProgramCategoryDto.Name, responseContent.Name);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task ProgramCategory_ShouldNotCreateProgramCategory_InvalidName(string? name)
    {
        var createProgramCategoryDto = new CreateHypotherapyProgramCategoryDto
        {
            Name = name!
        };
        var serializedDto = JsonConvert.SerializeObject(createProgramCategoryDto);

        HttpResponseMessage response = await Fixture.HttpClient.PostAsync("/api/HypotherapyProgramCategory/", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
