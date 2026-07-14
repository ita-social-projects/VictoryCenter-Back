using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Admin.ImpactStatistics;
using VictoryCenter.BLL.DTOs.Admin.ImpactStatistics.Metrics;
using VictoryCenter.BLL.DTOs.Admin.MainAboutUs;
using VictoryCenter.BLL.DTOs.Admin.MainDonations;
using VictoryCenter.BLL.DTOs.Admin.MainPages;
using VictoryCenter.BLL.DTOs.Admin.MainPartners;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

using EntityMainPage = VictoryCenter.DAL.Entities.MainPage;

namespace VictoryCenter.IntegrationTests.ControllerTests.MainPage.Update;

public class UpdateMainPageTests : BaseTestClass
{
    private readonly Uri _endpointUri = new("/api/MainPage", UriKind.Relative);

    public UpdateMainPageTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task Update_ValidData_ShouldReturnOkAndUpdateEntity()
    {
        var mainPage = await EnsureMainPageExistsAsync();
        var image1 = await EnsureImageExistsAsync();
        var image2 = await EnsureSecondImageExistsAsync(image1.Id);

        var existingStat = mainPage.ImpactStatistics;
        var existingMetric = existingStat?.Metrics.FirstOrDefault();

        var updateDto = CreateUpdateDto(
            title: "Updated MainPage title",
            mainImageId: image1.Id,
            statImageId: image2.Id,
            statId: existingStat?.Id,
            metricId: existingMetric?.Id);

        var response = await PutRaw(updateDto);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        Fixture.DbContext.ChangeTracker.Clear();
        var updated = await Fixture.DbContext.MainPages
            .Include(m => m.MainAboutUs)
            .Include(m => m.MainPartners)
            .Include(m => m.MainDonations)
            .Include(m => m.ImpactStatistics)
            .SingleAsync(m => m.Id == mainPage.Id);

        Assert.Equal(updateDto.Title, updated.Title);
        Assert.NotNull(updated.MainAboutUs);
        Assert.NotNull(updated.MainPartners);
        Assert.NotNull(updated.MainDonations);
        Assert.Equal(updateDto.MainDonations!.Title, updated.MainDonations.Title);
        Assert.NotNull(updated.ImpactStatistics);
        Assert.Equal("Updated existing stat", updated.ImpactStatistics.Title);
    }

    [Fact]
    public async Task Update_EmptyTitle_ShouldReturnBadRequest()
    {
        var mainPage = await EnsureMainPageExistsAsync();
        var image1 = await EnsureImageExistsAsync();
        var existingStat = mainPage.ImpactStatistics;
        var existingMetric = existingStat?.Metrics.FirstOrDefault();

        var updateDto = CreateUpdateDto(
            title: string.Empty,
            mainImageId: image1.Id,
            statImageId: image1.Id,
            statId: existingStat?.Id,
            metricId: existingMetric?.Id);

        var response = await PutRaw(updateDto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Update_NonExistentImageId_ShouldReturnNotFound()
    {
        var mainPage = await EnsureMainPageExistsAsync();
        var existingStat = mainPage.ImpactStatistics;
        var existingMetric = existingStat?.Metrics.FirstOrDefault();

        var nonExistentId = (await Fixture.DbContext.Images.MaxAsync(i => (long?)i.Id) ?? 0) + 1000;

        var updateDto = CreateUpdateDto(
            title: "Updated title",
            mainImageId: nonExistentId,
            statImageId: nonExistentId,
            statId: existingStat?.Id,
            metricId: existingMetric?.Id);

        var response = await PutRaw(updateDto);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Update_InvalidMainDonations_ShouldReturnBadRequest()
    {
        var mainPage = await EnsureMainPageExistsAsync();
        var image = await EnsureImageExistsAsync();
        var existingStat = mainPage.ImpactStatistics;
        var existingMetric = existingStat?.Metrics.FirstOrDefault();

        var updateDto = CreateUpdateDto(
            title: "Updated title",
            mainImageId: image.Id,
            statImageId: image.Id,
            statId: existingStat?.Id,
            metricId: existingMetric?.Id) with
        {
            MainDonations = new UpdateMainDonationsDto
            {
                Title = string.Empty,
                Description = "Updated Donations description",
                ImageId = image.Id,
            },
        };

        var response = await PutRaw(updateDto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ToggleMetricVisibility_ValidData_ShouldReturnOkAndUpdateEntity()
    {
        var mainPage = await EnsureMainPageExistsAsync();
        var existingMetric = mainPage.ImpactStatistics!.Metrics.First();

        var dto = new UpdateMetricVisibilityDto { IsHidden = true };

        var response = await PutVisibilityRaw(existingMetric.Id, dto);

        if (response.StatusCode != HttpStatusCode.OK)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"Expected OK, but got {response.StatusCode}. Error details: {errorContent}");
        }

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        Fixture.DbContext.ChangeTracker.Clear();

        var updatedMainPage = await Fixture.DbContext.MainPages
            .Include(m => m.ImpactStatistics)
                .ThenInclude(s => s!.Metrics)
            .SingleAsync(m => m.Id == mainPage.Id);

        var updatedMetric = updatedMainPage.ImpactStatistics!.Metrics.First(m => m.Id == existingMetric.Id);

        Assert.True(updatedMetric.IsHidden);
    }

    [Fact]
    public async Task ToggleMetricVisibility_NonExistentMetric_ShouldReturnNotFound()
    {
        var nonExistentId = 999999;
        var dto = new UpdateMetricVisibilityDto { IsHidden = true };

        var response = await PutVisibilityRaw(nonExistentId, dto);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private async Task<HttpResponseMessage> PutRaw(UpdateMainPageDto payload)
    {
        var serialized = JsonSerializer.Serialize(payload);
        return await Fixture.HttpClient.PutAsync(
            _endpointUri,
            new StringContent(serialized, Encoding.UTF8, "application/json"));
    }

    private async Task<HttpResponseMessage> PutVisibilityRaw(long metricId, UpdateMetricVisibilityDto payload)
    {
        var serialized = JsonSerializer.Serialize(payload);
        return await Fixture.HttpClient.PutAsync(
            $"/api/MainPage/metrics/{metricId}/visibility",
            new StringContent(serialized, Encoding.UTF8, "application/json"));
    }

    private static UpdateMainPageDto CreateUpdateDto(string title, long mainImageId, long statImageId, long? statId = null, long? metricId = null)
    {
        return new UpdateMainPageDto
        {
            Title = title,
            Description = "Updated description",
            ImageId = mainImageId,
            MainAboutUs = new UpdateMainAboutUsDto { Title = "Updated About Us title", Description = "Updated About Us description" },
            MainPartners = new UpdateMainPartnersDto { Title = "Updated Partners title", Description = "Updated Partners description" },
            MainDonations = new UpdateMainDonationsDto
            {
                Title = "Updated Donations title",
                Description = "Updated Donations description",
                ImageId = mainImageId,
            },
            ImpactStatistics = new UpdateImpactStatisticDto
            {
                Id = statId,
                Title = "Updated existing stat",
                ImageId = statImageId,
                Metrics =
                [
                    new UpdateMetricDto { Id = metricId, Value = 999, Name = "updated-name", Type = MetricType.Raised },
                    new UpdateMetricDto { Value = 123, Name = "new-metric", Type = MetricType.Partners },
                ],
            },
        };
    }

    private async Task<EntityMainPage> EnsureMainPageExistsAsync()
    {
        var existing = await Fixture.DbContext.MainPages
            .Include(m => m.MainAboutUs)
            .Include(m => m.MainPartners)
            .Include(m => m.MainDonations)
            .Include(m => m.ImpactStatistics)
                .ThenInclude(s => s!.Metrics)
            .FirstOrDefaultAsync();

        if (existing is not null && existing.ImpactStatistics?.Metrics.Count > 1)
        {
            return existing;
        }

        var image = await EnsureImageExistsAsync();
        var mainPage = new EntityMainPage
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

    private async Task<Image> EnsureImageExistsAsync()
    {
        var image = await Fixture.DbContext.Images.OrderBy(i => i.Id).FirstOrDefaultAsync();
        if (image is not null)
        {
            return image;
        }

        image = new Image
        {
            Url = "https://example.com/seed-image-1.jpg",
            CreatedAt = DateTimeOffset.UtcNow,
        };

        await Fixture.DbContext.Images.AddAsync(image);
        await Fixture.DbContext.SaveChangesAsync();

        return image;
    }

    private async Task<Image> EnsureSecondImageExistsAsync(long firstImageId)
    {
        var second = await Fixture.DbContext.Images
            .Where(i => i.Id != firstImageId)
            .OrderByDescending(i => i.Id)
            .FirstOrDefaultAsync();

        if (second is not null)
        {
            return second;
        }

        second = new Image
        {
            Url = "https://example.com/seed-image-2.jpg",
            CreatedAt = DateTimeOffset.UtcNow,
        };

        await Fixture.DbContext.Images.AddAsync(second);
        await Fixture.DbContext.SaveChangesAsync();

        return second;
    }
}
