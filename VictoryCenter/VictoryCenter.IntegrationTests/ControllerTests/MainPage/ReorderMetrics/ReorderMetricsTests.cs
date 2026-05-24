using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Admin.ImpactStatistics.Metrics;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.MainPage.ReorderMetrics;

public class ReorderMetricsTests : BaseTestClass
{
    private readonly Uri _endpointUri = new("/api/MainPage/metrics/reorder", UriKind.Relative);

    public ReorderMetricsTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task ReorderMetrics_WithValidOrder_ShouldReturnOkAndChangePriority()
    {
        // Arrange
        var targetStatistic = await EnsureImpactStatisticsExistsAsync();

        var orderedIds = targetStatistic.Metrics.OrderBy(m => m.Priority).Select(m => m.Id).ToList();

        orderedIds.Reverse();

        var reorderDto = new ReorderMetricsDto
        {
            StatisticId = targetStatistic.Id,
            OrderedIds = orderedIds
        };
        var serializedDto = JsonSerializer.Serialize(reorderDto);
        var content = new StringContent(serializedDto, Encoding.UTF8, "application/json");

        // Act
        var response = await Fixture.HttpClient.PutAsync(_endpointUri, content);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        Fixture.DbContext.ChangeTracker.Clear();
        var metricsAfterReorder = await Fixture.DbContext.Metrics
            .Where(m => m.StatisticId == targetStatistic.Id)
            .OrderBy(m => m.Priority)
            .ToListAsync();

        Assert.Equal(orderedIds, metricsAfterReorder.Select(m => m.Id));
    }

    [Fact]
    public async Task ReorderMetrics_WithNonExistentStatisticId_ShouldReturnBadRequest()
    {
        // Arrange
        var targetStatistic = await EnsureImpactStatisticsExistsAsync();
        var metricIds = targetStatistic.Metrics.Select(m => m.Id).ToList();

        var reorderDto = new ReorderMetricsDto
        {
            StatisticId = long.MaxValue,
            OrderedIds = metricIds
        };
        var serializedDto = JsonSerializer.Serialize(reorderDto);
        var content = new StringContent(serializedDto, Encoding.UTF8, "application/json");

        // Act
        var response = await Fixture.HttpClient.PutAsync(_endpointUri, content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    private async Task<ImpactStatistics> EnsureImpactStatisticsExistsAsync()
    {
        var existing = await Fixture.DbContext.ImpactStatistics
            .Include(s => s.Metrics)
            .Where(s => s.Metrics.Count > 1)
            .FirstOrDefaultAsync();

        if (existing is not null)
        {
            return existing;
        }

        var image = new Image
        {
            Url = "https://example.com/test-reorder.jpg",
            BlobName = "test-blob-name",
            MimeType = "image/jpeg",
            CreatedAt = DateTimeOffset.UtcNow
        };

        await Fixture.DbContext.Images.AddAsync(image);
        await Fixture.DbContext.SaveChangesAsync();

        var mainPage = new DAL.Entities.MainPage
        {
            Title = "Test MainPage",
            Description = "Test Desc",
            ImageId = image.Id,
            ImpactStatistics = new ImpactStatistics
            {
                Title = "Test Stat",
                ImageId = image.Id,
                Metrics =
                [
                    new Metric { Value = 10, Name = "Metric 1", Type = MetricType.Raised, Priority = 1 },
                    new Metric { Value = 20, Name = "Metric 2", Type = MetricType.Partners, Priority = 2 }
                ]
            }
        };

        await Fixture.DbContext.MainPages.AddAsync(mainPage);
        await Fixture.DbContext.SaveChangesAsync();
        Fixture.DbContext.ChangeTracker.Clear();

        return mainPage.ImpactStatistics!;
    }
}