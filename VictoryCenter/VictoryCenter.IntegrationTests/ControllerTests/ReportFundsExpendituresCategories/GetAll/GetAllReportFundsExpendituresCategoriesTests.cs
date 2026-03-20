using Newtonsoft.Json;
using VictoryCenter.BLL.DTOs.Admin.ReportFundsExpendituresCategories;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.ReportFundsExpendituresCategories.GetAll;

public class GetAllReportFundsExpendituresCategoriesTests : BaseTestClass
{
    public GetAllReportFundsExpendituresCategoriesTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task GetAllCategories_ShouldReturnAllCategories()
    {
        await Fixture.DbContext.ReportFundsExpendituresCategories.AddAsync(new ReportFundsExpendituresCategory
        {
            Name = "Income category",
            Type = ReportFundsExpendituresType.Income,
            CreatedAt = DateTimeOffset.UtcNow
        });
        await Fixture.DbContext.SaveChangesAsync();

        HttpResponseMessage response = await Fixture.HttpClient.GetAsync("/api/ReportFundsExpendituresCategories/");
        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();
        IEnumerable<ReportFundsExpendituresCategoryDto>? responseContent =
            JsonConvert.DeserializeObject<IEnumerable<ReportFundsExpendituresCategoryDto>>(responseString);

        Assert.NotNull(responseContent);
        Assert.NotEmpty(responseContent);
    }
}
