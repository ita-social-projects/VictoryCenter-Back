using System.Net;
using System.Text;
using Newtonsoft.Json;
using VictoryCenter.BLL.DTOs.Admin.ReportFundsExpendituresCategories;
using VictoryCenter.DAL.Enums;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.ReportFundsExpendituresCategories.Create;

public class CreateReportFundsExpendituresCategoryTests : BaseTestClass
{
    public CreateReportFundsExpendituresCategoryTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task CreateCategory_ShouldCreateCategory()
    {
        var createDto = new CreateReportFundsExpendituresCategoryDto
        {
            Name = "Created category",
            Type = ReportFundsExpendituresType.Expense
        };
        var serializedDto = JsonConvert.SerializeObject(createDto);

        HttpResponseMessage response = await Fixture.HttpClient.PostAsync(
            "/api/ReportFundsExpendituresCategories/",
            new StringContent(serializedDto, Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();
        ReportFundsExpendituresCategoryDto? responseContent =
            JsonConvert.DeserializeObject<ReportFundsExpendituresCategoryDto>(responseString);

        Assert.NotNull(responseContent);
        Assert.Equal(createDto.Name, responseContent.Name);
        Assert.Equal(createDto.Type, responseContent.Type);
    }

    [Theory]
    [InlineData("ПРОГРАМНІ")]
    [InlineData("програмні тест")]
    [InlineData("Програмні тест 2")]
    public async Task CreateCategory_ShouldNotCreateCategory_WhenNameIsReservedAndTypeIsExpense(string name)
    {
        var createDto = new CreateReportFundsExpendituresCategoryDto
        {
            Name = name,
            Type = ReportFundsExpendituresType.Expense
        };
        var serializedDto = JsonConvert.SerializeObject(createDto);

        HttpResponseMessage response = await Fixture.HttpClient.PostAsync(
            "/api/ReportFundsExpendituresCategories/",
            new StringContent(serializedDto, Encoding.UTF8, "application/json"));

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateCategory_ShouldCreateCategory_WhenNameIsReservedButTypeIsIncome()
    {
        var createDto = new CreateReportFundsExpendituresCategoryDto
        {
            Name = "Програмні тест 2",
            Type = ReportFundsExpendituresType.Income
        };
        var serializedDto = JsonConvert.SerializeObject(createDto);

        HttpResponseMessage response = await Fixture.HttpClient.PostAsync(
            "/api/ReportFundsExpendituresCategories/",
            new StringContent(serializedDto, Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task CreateCategory_ShouldNotCreateCategory_InvalidName(string? name)
    {
        var createDto = new CreateReportFundsExpendituresCategoryDto
        {
            Name = name!,
            Type = ReportFundsExpendituresType.Income
        };
        var serializedDto = JsonConvert.SerializeObject(createDto);

        HttpResponseMessage response = await Fixture.HttpClient.PostAsync(
            "/api/ReportFundsExpendituresCategories/",
            new StringContent(serializedDto, Encoding.UTF8, "application/json"));

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
