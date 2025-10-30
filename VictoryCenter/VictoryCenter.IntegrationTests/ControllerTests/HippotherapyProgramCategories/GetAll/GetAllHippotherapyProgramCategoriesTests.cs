using Newtonsoft.Json;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyProgramCategories;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.HippotherapyProgramCategories.GetAll;

public class GetAllHippotherapyProgramCategoriesTests : BaseTestClass
{
    public GetAllHippotherapyProgramCategoriesTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task ProgramCategory_ShouldReturnAllProgramCategories()
    {
        HttpResponseMessage response = await Fixture.HttpClient.GetAsync("/api/HippotherapyProgramCategories/");
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        IEnumerable<HippotherapyProgramCategoryDto>? responseContent = JsonConvert.DeserializeObject<IEnumerable<HippotherapyProgramCategoryDto>>(responseString);
        Assert.NotNull(responseContent);
        Assert.NotEmpty(responseContent);
    }
}
