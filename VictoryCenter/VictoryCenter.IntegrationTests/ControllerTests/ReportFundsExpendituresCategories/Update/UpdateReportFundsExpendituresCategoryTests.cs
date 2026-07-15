using System.Net;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using VictoryCenter.BLL.DTOs.Admin.ReportFundsExpendituresCategories;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.ReportFundsExpendituresCategories.Update;

public class UpdateReportFundsExpendituresCategoryTests : BaseTestClass
{
    public UpdateReportFundsExpendituresCategoryTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task UpdateCategory_ShouldUpdateCategory()
    {
        var category = await CreateCategoryAsync("Initial category", ReportFundsExpendituresType.Income);

        var updateDto = new UpdateReportFundsExpendituresCategoryDto
        {
            Name = "Updated category"
        };
        var serializedDto = JsonConvert.SerializeObject(updateDto);

        HttpResponseMessage response = await Fixture.HttpClient.PutAsync(
            $"/api/ReportFundsExpendituresCategories/{category.Id}",
            new StringContent(serializedDto, Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();
        ReportFundsExpendituresCategoryDto? responseContent =
            JsonConvert.DeserializeObject<ReportFundsExpendituresCategoryDto>(responseString);

        Assert.NotNull(responseContent);
        Assert.Equal(updateDto.Name, responseContent.Name);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public async Task UpdateCategory_ShouldNotUpdateCategory_NotFound(long id)
    {
        var updateDto = new UpdateReportFundsExpendituresCategoryDto
        {
            Name = "Updated category"
        };
        var serializedDto = JsonConvert.SerializeObject(updateDto);

        HttpResponseMessage response = await Fixture.HttpClient.PutAsync(
            $"/api/ReportFundsExpendituresCategories/{id}",
            new StringContent(serializedDto, Encoding.UTF8, "application/json"));

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateCategory_ShouldNotUpdateCategory_WhenDuplicateExists_IgnoringCaseAndWhitespace()
    {
        var categoryToUpdate = await CreateCategoryAsync("Initial category", ReportFundsExpendituresType.Income);
        await CreateCategoryAsync("  Updated category  ", ReportFundsExpendituresType.Income);

        var updateDto = new UpdateReportFundsExpendituresCategoryDto
        {
            Name = "updated CATEGORY"
        };
        var serializedDto = JsonConvert.SerializeObject(updateDto);

        HttpResponseMessage response = await Fixture.HttpClient.PutAsync(
            $"/api/ReportFundsExpendituresCategories/{categoryToUpdate.Id}",
            new StringContent(serializedDto, Encoding.UTF8, "application/json"));

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateCategory_ShouldNotUpdateCategory_WhenExistingCategoryIsReserved()
    {
        var category = await CreateCategoryAsync("Програмні тест 2", ReportFundsExpendituresType.Expense);

        var updateDto = new UpdateReportFundsExpendituresCategoryDto
        {
            Name = "Renamed category"
        };
        var serializedDto = JsonConvert.SerializeObject(updateDto);

        HttpResponseMessage response = await Fixture.HttpClient.PutAsync(
            $"/api/ReportFundsExpendituresCategories/{category.Id}",
            new StringContent(serializedDto, Encoding.UTF8, "application/json"));

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateCategory_ShouldNotUpdateCategory_WhenNewNameIsReserved()
    {
        var category = await CreateCategoryAsync("Initial expense category", ReportFundsExpendituresType.Expense);

        var updateDto = new UpdateReportFundsExpendituresCategoryDto
        {
            Name = "Програмні тест 2"
        };
        var serializedDto = JsonConvert.SerializeObject(updateDto);

        HttpResponseMessage response = await Fixture.HttpClient.PutAsync(
            $"/api/ReportFundsExpendituresCategories/{category.Id}",
            new StringContent(serializedDto, Encoding.UTF8, "application/json"));

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateCategory_ShouldUpdateCategory_WhenNameIsReservedButTypeIsIncome()
    {
        var category = await CreateCategoryAsync("Програмні тест 2", ReportFundsExpendituresType.Income);

        var updateDto = new UpdateReportFundsExpendituresCategoryDto
        {
            Name = "Renamed income category"
        };
        var serializedDto = JsonConvert.SerializeObject(updateDto);

        HttpResponseMessage response = await Fixture.HttpClient.PutAsync(
            $"/api/ReportFundsExpendituresCategories/{category.Id}",
            new StringContent(serializedDto, Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();
    }

    private async Task<ReportFundsExpendituresCategory> CreateCategoryAsync(
        string name,
        ReportFundsExpendituresType type)
    {
        var entity = new ReportFundsExpendituresCategory
        {
            Name = name,
            Type = type,
            CreatedAt = DateTimeOffset.UtcNow
        };

        await Fixture.DbContext.ReportFundsExpendituresCategories.AddAsync(entity);
        await Fixture.DbContext.SaveChangesAsync();

        return await Fixture.DbContext.ReportFundsExpendituresCategories
            .FirstAsync(category => category.Id == entity.Id);
    }
}
