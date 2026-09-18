using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Admin.ImpactStatistics.Metrics;
using VictoryCenter.BLL.DTOs.Admin.Localization.MainPage.Metrics;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;
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
    public async Task UpdateMetric_ValidData_ShouldReturnOkAndUpdateEntity()
    {
        var mainPage = await EnsureMainPageExistsAsync();
        var existingMetric = mainPage.ImpactStatistics!.Metrics.First();

        var dto = new UpdateSingleMetricDto
        {
            Value = 999,
            Name = "updated-metric-name",
            ExpectedVersion = existingMetric.RowVersion,
        };

        var response = await PutRaw(existingMetric.Id, dto);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        Fixture.DbContext.ChangeTracker.Clear();
        var updatedMetric = await Fixture.DbContext.Metrics.SingleAsync(m => m.Id == existingMetric.Id);

        Assert.Equal(999, updatedMetric.Value);
        Assert.Equal("updated-metric-name", updatedMetric.Name);
    }

    [Fact]
    public async Task UpdateMetric_Conflict_ShouldReturnBadRequest()
    {
        var mainPage = await EnsureMainPageExistsAsync();
        var existingMetric = mainPage.ImpactStatistics!.Metrics.First();

        var dto = new UpdateSingleMetricDto
        {
            Value = 888,
            ExpectedVersion = [99, 99, 99, 99],
        };

        var response = await PutRaw(existingMetric.Id, dto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateMetric_InvalidData_ShouldReturnBadRequest()
    {
        var mainPage = await EnsureMainPageExistsAsync();
        var existingMetric = mainPage.ImpactStatistics!.Metrics.First();

        var dto = new UpdateSingleMetricDto
        {
            Value = -5,
        };

        var response = await PutRaw(existingMetric.Id, dto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateMetric_SequentialUpdatesWithoutReload_ShouldSucceed()
    {
        var mainPage = await EnsureMainPageExistsAsync();
        var raisedMetric = mainPage.ImpactStatistics!.Metrics.First(m => m.Type == MetricType.Raised);

        var firstDto = new UpdateSingleMetricDto
        {
            Value = 450,
            IsAutoSynced = false,
            ExpectedVersion = raisedMetric.RowVersion,
        };

        var firstResponse = await PutRaw(raisedMetric.Id, firstDto);
        Assert.Equal(HttpStatusCode.OK, firstResponse.StatusCode);

        var firstResult = await DeserializeResponse<UpdateMetricResult>(firstResponse);
        Assert.NotNull(firstResult.RowVersion);

        var secondDto = new UpdateSingleMetricDto
        {
            IsAutoSynced = true,
            ExpectedVersion = firstResult.RowVersion,
        };

        var secondResponse = await PutRaw(raisedMetric.Id, secondDto);

        Assert.Equal(HttpStatusCode.OK, secondResponse.StatusCode);

        Fixture.DbContext.ChangeTracker.Clear();
        var finalMetric = await Fixture.DbContext.Metrics.SingleAsync(m => m.Id == raisedMetric.Id);
        Assert.True(finalMetric.IsAutoSynced);
    }

    [Fact]
    public async Task UpdateMetric_ToggleAutoSync_ShouldUpdateEntityAndReturnResultWithRowVersion()
    {
        var mainPage = await EnsureMainPageExistsAsync();
        var raisedMetric = mainPage.ImpactStatistics!.Metrics.First(m => m.Type == MetricType.Raised);

        var dto = new UpdateSingleMetricDto
        {
            IsAutoSynced = true,
            ExpectedVersion = raisedMetric.RowVersion,
        };

        var response = await PutRaw(raisedMetric.Id, dto);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await DeserializeResponse<UpdateMetricResult>(response);
        Assert.NotNull(result.RowVersion);
        Assert.True(result.WasModified);

        Fixture.DbContext.ChangeTracker.Clear();
        var updatedInDb = await Fixture.DbContext.Metrics.SingleAsync(m => m.Id == raisedMetric.Id);
        Assert.True(updatedInDb.IsAutoSynced);
    }

    [Fact]
    public async Task UpdateMetric_WithLocalization_ShouldUpdateOrCreateLocalization()
    {
        var mainPage = await EnsureMainPageExistsAsync();
        var metric = mainPage.ImpactStatistics!.Metrics.First();
        var englishLang = await EnsureEnglishLanguageExistsAsync();

        var dto = new UpdateSingleMetricDto
        {
            ExpectedVersion = metric.RowVersion,
            Localization = new UpdateMetricLocalizationDto
            {
                LanguageId = englishLang.Id,
                Name = "Children supported",
                Value = "777",
            }
        };

        var response = await PutRaw(metric.Id, dto);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        Fixture.DbContext.ChangeTracker.Clear();
        var updatedMetric = await Fixture.DbContext.Metrics
            .Include(m => m.Localizations)
            .SingleAsync(m => m.Id == metric.Id);

        var loc = updatedMetric.Localizations.FirstOrDefault(l => l.LanguageId == englishLang.Id);
        Assert.NotNull(loc);
        Assert.Equal("Children supported", loc.Name);
        Assert.Equal("777", loc.Value);
        Assert.Equal(TranslationStatus.Relevant, loc.TranslationStatus);
    }

    [Fact]
    public async Task UpdateMetric_NoChanges_ShouldReturnOkWithWasModifiedFalse()
    {
        var mainPage = await EnsureMainPageExistsAsync();
        var existingMetric = mainPage.ImpactStatistics!.Metrics.First();

        var dto = new UpdateSingleMetricDto
        {
            Value = existingMetric.Value,
            Name = existingMetric.Name,
            ExpectedVersion = existingMetric.RowVersion,
        };

        var response = await PutRaw(existingMetric.Id, dto);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await DeserializeResponse<UpdateMetricResult>(response);
        Assert.False(result.WasModified);
        Assert.NotNull(result.RowVersion);
        Assert.Equal(existingMetric.Value, result.Value);
    }

    private async Task<HttpResponseMessage> PutRaw(long metricId, UpdateSingleMetricDto payload)
    {
        var serialized = JsonSerializer.Serialize(payload);
        return await Fixture.HttpClient.PutAsync(
            $"/api/MainPage/metrics/{metricId}",
            new StringContent(serialized, Encoding.UTF8, "application/json"));
    }

    private async Task<T> DeserializeResponse<T>(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(content, JsonOptions)!;
    }

    private async Task<LocalizationLanguage> EnsureEnglishLanguageExistsAsync()
    {
        var lang = await Fixture.DbContext.LocalizationLanguages.FirstOrDefaultAsync(l => l.Code == "en");
        if (lang is not null)
        {
            return lang;
        }

        lang = new LocalizationLanguage { Code = "en", Name = "English" };
        await Fixture.DbContext.LocalizationLanguages.AddAsync(lang);
        await Fixture.DbContext.SaveChangesAsync();
        return lang;
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
                    new Metric { Value = 100, Name = "children", Type = MetricType.Raised, IsAutoSynced = false, IsHidden = false, RowVersion = new byte[] { 1, 0, 0, 0 } },
                    new Metric { Value = 200, Name = "families", Type = MetricType.Partners, IsAutoSynced = false, IsHidden = false, RowVersion = new byte[] { 2, 0, 0, 0 } }
                ],
            },
        };

        await Fixture.DbContext.MainPages.AddAsync(mainPage);
        await Fixture.DbContext.SaveChangesAsync();
        Fixture.DbContext.ChangeTracker.Clear();

        return mainPage;
    }
}
