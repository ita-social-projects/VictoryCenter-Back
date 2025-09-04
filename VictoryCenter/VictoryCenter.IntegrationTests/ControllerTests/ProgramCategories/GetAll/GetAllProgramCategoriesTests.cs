using Newtonsoft.Json;
using VictoryCenter.BLL.DTOs.Admin.ProgramCategories;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.ProgramCategories.GetAll;

public class GetAllProgramCategoriesTests : BaseTestClass
{
    public GetAllProgramCategoriesTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task ProgramCategory_ShouldReturnAllProgramCategories()
    {
        HttpResponseMessage response = await Fixture.HttpClient.GetAsync("/api/ProgramCategory/");
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        IEnumerable<ProgramCategoryDto>? responseContent = JsonConvert.DeserializeObject<IEnumerable<ProgramCategoryDto>>(responseString);
        Assert.NotNull(responseContent);
        Assert.NotEmpty(responseContent);
    }
}
