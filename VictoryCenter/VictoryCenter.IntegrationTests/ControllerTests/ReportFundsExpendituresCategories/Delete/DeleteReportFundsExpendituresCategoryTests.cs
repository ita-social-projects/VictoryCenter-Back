using System.Net;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.ReportFundsExpendituresCategories.Delete;

public class DeleteReportFundsExpendituresCategoryTests : BaseTestClass
{
    public DeleteReportFundsExpendituresCategoryTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task DeleteCategory_ShouldDeleteCategory()
    {
        var category = await CreateCategoryAsync("Delete category", ReportFundsExpendituresType.Expense);

        HttpResponseMessage response = await Fixture.HttpClient.DeleteAsync(
            $"/api/ReportFundsExpendituresCategories/{category.Id}");
        response.EnsureSuccessStatusCode();

        Assert.Null(await Fixture.DbContext.ReportFundsExpendituresCategories
            .FirstOrDefaultAsync(entity => entity.Id == category.Id));
    }

    [Fact]
    public async Task DeleteCategory_ShouldNotDeleteCategory_WhenAssociatedWithRecord()
    {
        var category = await CreateCategoryAsync("Category with record", ReportFundsExpendituresType.Income);
        await Fixture.DbContext.ReportFundsExpendituresRecords.AddAsync(new ReportFundsExpendituresRecord
        {
            CategoryId = category.Id,
            Type = ReportFundsExpendituresType.Income,
            ReportingYear = 2025,
            AmountUah = 200m,
            AmountUsd = 10m,
            CreatedAt = DateTimeOffset.UtcNow
        });
        await Fixture.DbContext.SaveChangesAsync();

        HttpResponseMessage response = await Fixture.HttpClient.DeleteAsync(
            $"/api/ReportFundsExpendituresCategories/{category.Id}");

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public async Task DeleteCategory_ShouldNotDeleteCategory_NotFound(long id)
    {
        HttpResponseMessage response = await Fixture.HttpClient.DeleteAsync(
            $"/api/ReportFundsExpendituresCategories/{id}");

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
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

        return entity;
    }
}
