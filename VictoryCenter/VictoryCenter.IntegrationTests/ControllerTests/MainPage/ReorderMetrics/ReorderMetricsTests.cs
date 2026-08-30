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
    public async Task ReorderMetrics_WithHiddenMetricInTheMiddle_ShouldReorderVisibleAndMoveHiddenAfterVisible()
    {
        // Arrange
        var targetStatistic = await CreateImpactStatisticsAsync([
            new Metric { Value = 10, Name = "Metric 1", Type = MetricType.Raised, Priority = 0, IsHidden = false, RowVersion = [1] },
            new Metric { Value = 20, Name = "Metric 2", Type = MetricType.Partners, Priority = 1, IsHidden = true, RowVersion = [1] },
            new Metric { Value = 30, Name = "Metric 3", Type = MetricType.Programs, Priority = 2, IsHidden = false, RowVersion = [1] },
            new Metric { Value = 40, Name = "Metric 4", Type = MetricType.TherapyHours, Priority = 3, IsHidden = false, RowVersion = [1] }
        ]);

        var reorderDto = new ReorderMetricsDto
        {
            StatisticId = targetStatistic.Id,
            OrderedIds = [targetStatistic.Metrics.Single(m => m.Name == "Metric 4").Id,
                targetStatistic.Metrics.Single(m => m.Name == "Metric 1").Id,
                targetStatistic.Metrics.Single(m => m.Name == "Metric 3").Id]
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

        Assert.Equal(
            reorderDto.OrderedIds.Concat([targetStatistic.Metrics.Single(m => m.IsHidden).Id]),
            metricsAfterReorder.Select(m => m.Id));
        Assert.Equal([0L, 1L, 2L, 3L], metricsAfterReorder.Select(m => m.Priority));
    }

    [Fact]
    public async Task ReorderMetrics_WithPriorityGaps_ShouldRenumberAndReturnOk()
    {
        // Arrange
        var targetStatistic = await CreateImpactStatisticsAsync([
            new Metric { Value = 10, Name = "Metric 1", Type = MetricType.Raised, Priority = 0, IsHidden = false, RowVersion = [1] },
            new Metric { Value = 20, Name = "Metric 2", Type = MetricType.Partners, Priority = 2, IsHidden = false, RowVersion = [1] },
            new Metric { Value = 30, Name = "Metric 3", Type = MetricType.Programs, Priority = 3, IsHidden = false, RowVersion = [1] },
            new Metric { Value = 40, Name = "Metric 4", Type = MetricType.TherapyHours, Priority = 4, IsHidden = false, RowVersion = [1] }
        ]);

        var reorderDto = new ReorderMetricsDto
        {
            StatisticId = targetStatistic.Id,
            OrderedIds = [targetStatistic.Metrics.Single(m => m.Name == "Metric 4").Id,
                targetStatistic.Metrics.Single(m => m.Name == "Metric 1").Id,
                targetStatistic.Metrics.Single(m => m.Name == "Metric 2").Id,
                targetStatistic.Metrics.Single(m => m.Name == "Metric 3").Id]
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

        Assert.Equal(reorderDto.OrderedIds, metricsAfterReorder.Select(m => m.Id));
        Assert.Equal([0L, 1L, 2L, 3L], metricsAfterReorder.Select(m => m.Priority));
    }

    [Fact]
    public async Task ReorderMetrics_WithDuplicateIds_ShouldReturnBadRequest()
    {
        // Arrange
        var targetStatistic = await EnsureImpactStatisticsExistsAsync();
        var firstMetricId = targetStatistic.Metrics.OrderBy(m => m.Priority).First().Id;

        var reorderDto = new ReorderMetricsDto
        {
            StatisticId = targetStatistic.Id,
            OrderedIds = [firstMetricId, firstMetricId]
        };
        var serializedDto = JsonSerializer.Serialize(reorderDto);
        var content = new StringContent(serializedDto, Encoding.UTF8, "application/json");

        // Act
        var response = await Fixture.HttpClient.PutAsync(_endpointUri, content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ReorderMetrics_WithMissingVisibleId_ShouldReturnBadRequest()
    {
        // Arrange
        var targetStatistic = await EnsureImpactStatisticsExistsAsync();
        var metricIds = targetStatistic.Metrics.OrderBy(m => m.Priority).Select(m => m.Id).Take(1).ToList();

        var reorderDto = new ReorderMetricsDto
        {
            StatisticId = targetStatistic.Id,
            OrderedIds = metricIds
        };
        var serializedDto = JsonSerializer.Serialize(reorderDto);
        var content = new StringContent(serializedDto, Encoding.UTF8, "application/json");

        // Act
        var response = await Fixture.HttpClient.PutAsync(_endpointUri, content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
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
                    new Metric { Value = 10, Name = "Metric 1", Type = MetricType.Raised, Priority = 1, RowVersion = new byte[] { 1 } },
                    new Metric { Value = 20, Name = "Metric 2", Type = MetricType.Partners, Priority = 2, RowVersion = new byte[] { 1 } }
                ]
            }
        };

        await Fixture.DbContext.MainPages.AddAsync(mainPage);
        await Fixture.DbContext.SaveChangesAsync();
        Fixture.DbContext.ChangeTracker.Clear();

        return mainPage.ImpactStatistics!;
    }

    private async Task<ImpactStatistics> CreateImpactStatisticsAsync(List<Metric> metrics)
    {
        var image = new Image
        {
            Url = "https://example.com/test-reorder.jpg",
            BlobName = Guid.NewGuid().ToString("N"),
            MimeType = "image/jpeg",
            CreatedAt = DateTimeOffset.UtcNow
        };

        await Fixture.DbContext.Images.AddAsync(image);
        await Fixture.DbContext.SaveChangesAsync();

        var mainPage = new DAL.Entities.MainPage
        {
            Title = $"Test MainPage {Guid.NewGuid():N}",
            Description = "Test Desc",
            ImageId = image.Id,
            ImpactStatistics = new ImpactStatistics
            {
                Title = "Test Stat",
                ImageId = image.Id,
                Metrics = metrics
            }
        };

        await Fixture.DbContext.MainPages.AddAsync(mainPage);
        await Fixture.DbContext.SaveChangesAsync();
        Fixture.DbContext.ChangeTracker.Clear();

        return await Fixture.DbContext.ImpactStatistics
            .Include(s => s.Metrics)
            .SingleAsync(s => s.Id == mainPage.ImpactStatistics!.Id);
    }
}
