using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Admin.ImpactStatistics.Metrics;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.MainPage.Update;

public class UpdateSingleMetricTests : BaseTestClass
{
    public UpdateSingleMetricTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task PatchMetric_ValidData_ShouldReturnOkAndUpdateEntity()
    {
        var mainPage = await EnsureMainPageExistsAsync();
        var existingMetric = mainPage.ImpactStatistics!.Metrics.First();

        var dto = new UpdateSingleMetricDto
        {
            Value = 999,
            Name = "updated-patch-name",
            ExpectedVersion = existingMetric.RowVersion,
        };

        var response = await PatchRaw(existingMetric.Id, dto);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        Fixture.DbContext.ChangeTracker.Clear();
        var updatedMetric = await Fixture.DbContext.Metrics.SingleAsync(m => m.Id == existingMetric.Id);

        Assert.Equal(999, updatedMetric.Value);
        Assert.Equal("updated-patch-name", updatedMetric.Name);
    }

    [Fact]
    public async Task PatchMetric_Conflict_ShouldReturnBadRequest()
    {
        var mainPage = await EnsureMainPageExistsAsync();
        var existingMetric = mainPage.ImpactStatistics!.Metrics.First();

        var dto = new UpdateSingleMetricDto
        {
            Value = 888,
            ExpectedVersion = [99, 99, 99, 99],
        };

        var response = await PatchRaw(existingMetric.Id, dto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PatchMetric_InvalidData_ShouldReturnBadRequest()
    {
        var mainPage = await EnsureMainPageExistsAsync();
        var existingMetric = mainPage.ImpactStatistics!.Metrics.First();

        var dto = new UpdateSingleMetricDto
        {
            Value = -5,
        };

        var response = await PatchRaw(existingMetric.Id, dto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    private async Task<HttpResponseMessage> PatchRaw(long metricId, UpdateSingleMetricDto payload)
    {
        var serialized = JsonSerializer.Serialize(payload);
        return await Fixture.HttpClient.PatchAsync(
            $"/api/MainPage/metrics/{metricId}",
            new StringContent(serialized, Encoding.UTF8, "application/json"));
    }

    private async Task<DAL.Entities.MainPage> EnsureMainPageExistsAsync()
    {
        var existing = await Fixture.DbContext.MainPages
            .Include(m => m.ImpactStatistics)
                .ThenInclude(s => s!.Metrics)
            .FirstOrDefaultAsync();

        if (existing is not null && existing.ImpactStatistics?.Metrics.Count > 1)
        {
            return existing;
        }

        var image = new Image
        {
            Url = "https://example.com/seed-image-1.jpg",
            BlobName = "seed-image-1",
            MimeType = "image/jpeg",
            CreatedAt = DateTimeOffset.UtcNow,
        };
        await Fixture.DbContext.Images.AddAsync(image);
        await Fixture.DbContext.SaveChangesAsync();

        var mainPage = new DAL.Entities.MainPage
        {
            Title = "Seed Title",
            Description = "Seed Desc",
            ImageId = image.Id,
            MainAboutUs = new MainAboutUs { Title = "Seed About Us", Description = "Seed Desc" },
            MainPartners = new MainPartners { Title = "Seed Partners", Description = "Seed Desc" },
            MainDonations = new MainDonations
            {
                Title = "Seed Donations",
                Description = "Seed Desc",
                ImageId = image.Id,
            },
            ImpactStatistics = new ImpactStatistics
            {
                Title = "Seed Stat",
                ImageId = image.Id,
                Metrics =
                [
                    new Metric { Value = 100, Name = "children", Type = MetricType.Raised, IsHidden = false, RowVersion = new byte[] { 1 } },
                    new Metric { Value = 200, Name = "families", Type = MetricType.Partners, IsHidden = false, RowVersion = new byte[] { 1 } }
                ],
            },
        };

        await Fixture.DbContext.MainPages.AddAsync(mainPage);
        await Fixture.DbContext.SaveChangesAsync();
        Fixture.DbContext.ChangeTracker.Clear();

        return mainPage;
    }
}
