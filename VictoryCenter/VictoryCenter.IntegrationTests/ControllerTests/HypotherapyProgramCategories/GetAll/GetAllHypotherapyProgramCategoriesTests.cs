using Newtonsoft.Json;
using VictoryCenter.BLL.DTOs.Admin.HypotherapyProgramCategories;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.HypotherapyProgramCategories.GetAll;

public class GetAllHypotherapyProgramCategoriesTests : BaseTestClass
{
    public GetAllHypotherapyProgramCategoriesTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task ProgramCategory_ShouldReturnAllProgramCategories()
    {
        HttpResponseMessage response = await Fixture.HttpClient.GetAsync("/api/HypotherapyProgramCategory/");
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        IEnumerable<HypotherapyProgramCategoryDto>? responseContent = JsonConvert.DeserializeObject<IEnumerable<HypotherapyProgramCategoryDto>>(responseString);
        Assert.NotNull(responseContent);
        Assert.NotEmpty(responseContent);
    }
}
