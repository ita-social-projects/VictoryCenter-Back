using Microsoft.EntityFrameworkCore;
using VictoryCenter.DAL.Data;

namespace VictoryCenter.UnitTests.DataTests;

public class SqlServerCompatibilityTests
{
    [Fact]
    public void Model_ShouldMatchExistingMigrationSnapshot()
    {
        using var context = CreateContext();

        Assert.False(context.Database.HasPendingModelChanges());
    }

    [Fact]
    public void CategoryQuery_ShouldTranslateCollectionFilterAndOrdering()
    {
        using var context = CreateContext();
        long[] categoryIds = [1, 2, 3];

        var sql = context.EventNews
            .Where(eventNews => categoryIds.Contains(eventNews.CategoryId))
            .OrderBy(eventNews => eventNews.Priority)
            .ThenBy(eventNews => eventNews.Id)
            .Select(eventNews => eventNews.Id)
            .ToQueryString();

        Assert.Contains("ORDER BY", sql);
        Assert.Contains(" IN (", sql);
    }

    [Fact]
    public void CategorySummary_ShouldTranslateGroupingAndAggregation()
    {
        using var context = CreateContext();

        var sql = context.EventNews
            .GroupBy(eventNews => eventNews.CategoryId)
            .Select(group => new { CategoryId = group.Key, Count = group.Count() })
            .OrderBy(summary => summary.CategoryId)
            .ToQueryString();

        Assert.Contains("GROUP BY", sql);
        Assert.Contains("COUNT(*)", sql);
        Assert.Contains("ORDER BY", sql);
    }

    private static VictoryCenterDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<VictoryCenterDbContext>()
            .UseSqlServer("Server=localhost;Database=TranslationOnly;Integrated Security=true;TrustServerCertificate=true")
            .Options;

        return new VictoryCenterDbContext(options);
    }
}
