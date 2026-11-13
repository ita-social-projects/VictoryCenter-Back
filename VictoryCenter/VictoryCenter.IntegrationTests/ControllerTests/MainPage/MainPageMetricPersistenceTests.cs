using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VictoryCenter.BLL.DTOs.Admin.ImpactStatistics;
using VictoryCenter.BLL.DTOs.Admin.ImpactStatistics.Metrics;
using VictoryCenter.BLL.DTOs.Admin.Localization.MainPage.Metrics;
using VictoryCenter.BLL.DTOs.Admin.MainPages;
using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;
using VictoryCenter.IntegrationTests.Utils;

namespace VictoryCenter.IntegrationTests.ControllerTests.MainPage;

[Collection("SharedIntegrationTests")]
public class MainPageMetricPersistenceTests : IClassFixture<MainPageSqlServerFixture>, IAsyncLifetime
{
    private readonly MainPageSqlServerFixture _fixture;

    public MainPageMetricPersistenceTests(MainPageSqlServerFixture fixture)
    {
        _fixture = fixture;
    }

    public static TheoryData<int, string> InvalidMetricUpdates => new()
    {
        { 0, "null" },
        { 0, "nullElement" },
        { 0, "duplicateNewType" },
        { 0, "unknownNewId" },
        { 0, "invalidNewType" },
        { 0, "wrongStatisticsId" },
        { 3, "incomplete" },
        { 3, "empty" },
        { 3, "null" },
        { 3, "nullElement" },
        { 3, "duplicateType" },
        { 3, "invalidType" },
        { 3, "duplicateId" },
        { 3, "unknownId" },
        { 3, "missingId" },
        { 3, "swappedIds" },
        { 3, "replaceType" },
        { 3, "replaceWithNewType" },
        { 3, "unknownNewId" },
        { 3, "wrongStatisticsId" },
        { 4, "incomplete" },
        { 4, "empty" },
        { 4, "null" },
        { 4, "nullElement" },
        { 4, "duplicateType" },
        { 4, "invalidType" },
        { 4, "duplicateId" },
        { 4, "unknownId" },
        { 4, "missingId" },
        { 4, "swappedIds" },
        { 4, "wrongStatisticsId" },
    };

    public async Task InitializeAsync()
    {
        using var scope = _fixture.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<VictoryCenterDbContext>();
        db.MainPages.RemoveRange(await db.MainPages.ToListAsync());
        await db.SaveChangesAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [SqlServerFact]
    public async Task Get_PublicFiltersHidden_AdminReturnsCompleteOrderedAggregate()
    {
        var page = await SeedAsync();
        var authorization = _fixture.Client.DefaultRequestHeaders.Authorization;
        _fixture.Client.DefaultRequestHeaders.Authorization = null;
        try
        {
            var publicPage = await _fixture.Client.GetFromJsonAsync<MainPageDto>("/api/MainPage");
            Assert.Equal(3, publicPage!.ImpactStatistics!.Metrics.Count);
            Assert.DoesNotContain(publicPage.ImpactStatistics.Metrics, m => m.IsHidden);
            var unauthorized = await _fixture.Client.GetAsync("/api/MainPage/admin");
            Assert.Equal(HttpStatusCode.Unauthorized, unauthorized.StatusCode);
        }
        finally
        {
            _fixture.Client.DefaultRequestHeaders.Authorization = authorization;
        }

        var adminPage = await _fixture.Client.GetFromJsonAsync<MainPageDto>("/api/MainPage/admin");

        Assert.NotNull(adminPage);
        Assert.Equal(page.Id, adminPage.Id);
        Assert.Equal(
            page.ImpactStatistics!.Metrics.OrderBy(m => m.Priority).Select(m => m.Id),
            adminPage.ImpactStatistics!.Metrics.Select(m => m.Id));
        var hidden = Assert.Single(adminPage.ImpactStatistics.Metrics, m => m.IsHidden);
        Assert.Equal("Translated Programs", hidden.Localizations.Single().Name);
        Assert.Equal("en", hidden.Localizations.Single().LocalizationInfoDto.Code);
        Assert.Equal("123 translated", hidden.Localizations.Single().Value);
    }

    [SqlServerTheory]
    [InlineData(0, true)]
    [InlineData(0, false)]
    [InlineData(3, true)]
    [InlineData(3, false)]
    [InlineData(4, true)]
    [InlineData(4, false)]
    public async Task Publish_TitleChangePreservesMetricsAndTranslations(int existingCount, bool includeStatistics)
    {
        var page = await SeedAsync(existingCount);
        var before = await SnapshotAsync();
        var dto = ToUpdate(page);
        dto = includeStatistics
            ? dto with { ImpactStatistics = dto.ImpactStatistics! with { Title = "Updated statistics title" } }
            : dto with { Title = "Updated main page title", ImpactStatistics = null };

        var response = await _fixture.Client.PutAsJsonAsync("/api/MainPage", dto);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(before, await SnapshotAsync());
        using var scope = _fixture.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<VictoryCenterDbContext>();
        var persisted = await db.MainPages.Include(p => p.ImpactStatistics).SingleAsync();
        Assert.Equal(dto.Title, persisted.Title);
        Assert.Equal(includeStatistics ? dto.ImpactStatistics!.Title : page.ImpactStatistics!.Title, persisted.ImpactStatistics!.Title);
    }

    [SqlServerTheory]
    [MemberData(nameof(InvalidMetricUpdates))]
    public async Task Publish_InvalidMetricsRejectedWithoutDatabaseChanges(int existingCount, string invalidCase)
    {
        var page = await SeedAsync(existingCount);
        var before = await SnapshotAsync();
        var dto = ToUpdate(page);
        var metrics = dto.ImpactStatistics!.Metrics.ToList();
        switch (invalidCase)
        {
            case "nullElement":
                metrics.Add(null!);
                break;
            case "duplicateNewType":
                metrics.Add(new UpdateMetricDto { Name = "Partners", Type = MetricType.Partners });
                metrics.Add(new UpdateMetricDto { Name = "Partners again", Type = MetricType.Partners });
                break;
            case "unknownNewId":
                metrics.Add(new UpdateMetricDto { Id = long.MaxValue, Name = "Hours", Type = MetricType.TherapyHours });
                break;
            case "invalidNewType":
                metrics.Add(new UpdateMetricDto { Name = "Invalid type", Type = (MetricType)999 });
                break;
            case "replaceType":
                metrics[0] = metrics[0] with { Type = MetricType.TherapyHours };
                break;
            case "replaceWithNewType":
                metrics[0] = metrics[0] with { Id = null, Type = MetricType.TherapyHours };
                break;
            case "incomplete":
                metrics.RemoveAll(m => m.Type == MetricType.Programs);
                break;
            case "empty":
                metrics.Clear();
                break;
            case "duplicateType":
                metrics[0] = metrics[0] with { Type = metrics[1].Type };
                break;
            case "invalidType":
                metrics[0] = metrics[0] with { Type = (MetricType)999 };
                break;
            case "duplicateId":
                metrics[0] = metrics[0] with { Id = metrics[1].Id };
                break;
            case "unknownId":
                metrics[0] = metrics[0] with { Id = long.MaxValue };
                break;
            case "missingId":
                metrics[0] = metrics[0] with { Id = null };
                break;
            case "swappedIds":
                (metrics[0], metrics[1]) = (metrics[0] with { Id = metrics[1].Id }, metrics[1] with { Id = metrics[0].Id });
                break;
        }

        dto = dto with
        {
            Title = "Must not be persisted",
            ImpactStatistics = dto.ImpactStatistics with
            {
                Id = invalidCase == "wrongStatisticsId" ? long.MaxValue : dto.ImpactStatistics.Id,
                Title = "Must not be persisted",
                Metrics = invalidCase == "null" ? null! : metrics,
            },
        };

        var response = await _fixture.Client.PutAsJsonAsync("/api/MainPage", dto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(before, await SnapshotAsync());
        using var scope = _fixture.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<VictoryCenterDbContext>();
        var persisted = await db.MainPages.Include(p => p.ImpactStatistics).SingleAsync();
        Assert.Equal(page.Title, persisted.Title);
        Assert.Equal(page.ImpactStatistics!.Title, persisted.ImpactStatistics!.Title);
    }

    [SqlServerTheory]
    [InlineData(0, 1)]
    [InlineData(0, 4)]
    [InlineData(3, 4)]
    public async Task Publish_IncompleteStatisticsCanBeInitializedExplicitly(int existingCount, int requestedCount)
    {
        var page = await SeedAsync(existingCount);
        var ids = page.ImpactStatistics!.Metrics.Select(m => m.Id).ToArray();
        var before = await SnapshotAsync();
        var dto = ToUpdate(page);
        dto = dto with
        {
            ImpactStatistics = dto.ImpactStatistics! with
            {
                Metrics = [.. Enum.GetValues<MetricType>().Take(requestedCount).Select(type =>
                    dto.ImpactStatistics.Metrics.SingleOrDefault(m => m.Type == type) ??
                    new UpdateMetricDto { Name = type.ToString(), Type = type, Value = 123 })],
            },
        };

        var response = await _fixture.Client.PutAsJsonAsync("/api/MainPage", dto);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var scope = _fixture.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<VictoryCenterDbContext>();
        var metrics = await db.Metrics.ToListAsync();
        Assert.Equal(requestedCount, metrics.Count);
        Assert.All(ids, id => Assert.Contains(metrics, m => m.Id == id));
        Assert.Equal(requestedCount, metrics.Select(m => m.Type).Distinct().Count());
        Assert.Equal(before, (await SnapshotAsync()).Where(row => ids.Any(id => row.StartsWith($"{id}|"))).ToArray());
        Assert.All(metrics.Where(m => !ids.Contains(m.Id)), metric =>
        {
            Assert.True(metric.Id > 0);
            Assert.Equal(page.ImpactStatistics.Id, metric.StatisticId);
            Assert.Equal(123, metric.Value);
            Assert.Equal(metric.Type.ToString(), metric.Name);
            Assert.False(metric.IsHidden);
        });
    }

    [SqlServerFact]
    public async Task Publish_NewStatisticsBlockCanBeCreatedWithoutMetricIds()
    {
        var page = await SeedAsync(0);
        var dto = ToUpdate(page);
        using (var scope = _fixture.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<VictoryCenterDbContext>();
            db.ImpactStatistics.Remove(await db.ImpactStatistics.SingleAsync());
            await db.SaveChangesAsync();
        }

        dto = dto with
        {
            ImpactStatistics = dto.ImpactStatistics! with
            {
                Id = null,
                Metrics = [.. Enum.GetValues<MetricType>()
                    .Select(type => new UpdateMetricDto { Name = type.ToString(), Type = type, Value = 123 })],
            },
        };

        var response = await _fixture.Client.PutAsJsonAsync("/api/MainPage", dto);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var verificationScope = _fixture.Services.CreateScope();
        var persisted = verificationScope.ServiceProvider.GetRequiredService<VictoryCenterDbContext>();
        Assert.Equal(4, await persisted.Metrics.CountAsync());
    }

    [SqlServerTheory]
    [InlineData(0)]
    [InlineData(3)]
    public async Task Publish_NewStatisticsBlockRejectsIncompleteMetricsWithoutDatabaseChanges(int requestedCount)
    {
        var page = await SeedAsync(0);
        var dto = ToUpdate(page);
        using (var scope = _fixture.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<VictoryCenterDbContext>();
            db.ImpactStatistics.Remove(await db.ImpactStatistics.SingleAsync());
            await db.SaveChangesAsync();
        }

        dto = dto with
        {
            Title = "Must not be persisted",
            ImpactStatistics = dto.ImpactStatistics! with
            {
                Id = null,
                Metrics = [.. Enum.GetValues<MetricType>().Take(requestedCount)
                    .Select(type => new UpdateMetricDto { Name = type.ToString(), Type = type })],
            },
        };

        var response = await _fixture.Client.PutAsJsonAsync("/api/MainPage", dto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        using var verificationScope = _fixture.Services.CreateScope();
        var persisted = verificationScope.ServiceProvider.GetRequiredService<VictoryCenterDbContext>();
        Assert.Equal(page.Title, (await persisted.MainPages.SingleAsync()).Title);
        Assert.Empty(await persisted.ImpactStatistics.ToListAsync());
        Assert.Empty(await persisted.Metrics.ToListAsync());
    }

    [SqlServerFact]
    public async Task Publish_ExplicitMetricEditPreservesIdentityVisibilityAndPriority()
    {
        var page = await SeedAsync();
        var dto = ToUpdate(page);
        var hidden = page.ImpactStatistics!.Metrics.Single(m => m.IsHidden);
        var metrics = dto.ImpactStatistics!.Metrics.Select(m => m.Id == hidden.Id
            ? m with { Value = 456, Name = "Updated programs" }
            : m).ToList();
        dto = dto with { ImpactStatistics = dto.ImpactStatistics with { Metrics = metrics } };

        var response = await _fixture.Client.PutAsJsonAsync("/api/MainPage", dto);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var scope = _fixture.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<VictoryCenterDbContext>();
        var updated = await db.Metrics.Include(m => m.Localizations).SingleAsync(m => m.Id == hidden.Id);
        Assert.Equal(4, await db.Metrics.CountAsync());
        Assert.Equal(456, updated.Value);
        Assert.Equal("Updated programs", updated.Name);
        Assert.Equal(hidden.Priority, updated.Priority);
        Assert.True(updated.IsHidden);
        Assert.Equal("Translated Programs", updated.Localizations.Single().Name);
        Assert.Equal(TranslationStatus.Outdated, updated.Localizations.Single().TranslationStatus);
    }

    [SqlServerTheory]
    [InlineData(0)]
    [InlineData(3)]
    [InlineData(4)]
    public async Task Publish_LocalizationSaveFailureRollsBackPreviouslySavedFields(int existingCount)
    {
        var page = await SeedAsync(existingCount);
        var before = await SnapshotAsync();
        var dto = ToUpdate(page);
        var metrics = dto.ImpactStatistics!.Metrics.ToList();
        var missingType = Enum.GetValues<MetricType>().FirstOrDefault(type => metrics.All(m => m.Type != type));
        if (existingCount < 4)
        {
            metrics.Add(new UpdateMetricDto { Name = missingType.ToString(), Type = missingType });
        }

        metrics[^1] = metrics[^1] with
        {
            Value = 999,
            Localization = new UpdateMetricLocalizationDto { LanguageId = long.MaxValue, Name = "Invalid language" },
        };
        dto = dto with
        {
            Title = "Must be rolled back",
            ImpactStatistics = dto.ImpactStatistics with { Metrics = metrics },
        };

        var response = await _fixture.Client.PutAsJsonAsync("/api/MainPage", dto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(before, await SnapshotAsync());
        using var scope = _fixture.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<VictoryCenterDbContext>();
        Assert.Equal(page.Title, (await db.MainPages.SingleAsync()).Title);
    }

    [SqlServerTheory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Reorder_CompleteIdsPersistNormalizedPrioritiesIncludingHiddenMetrics(bool hidden)
    {
        await SeedAsync(hidden: hidden);
        var admin = await _fixture.Client.GetFromJsonAsync<MainPageDto>("/api/MainPage/admin");
        var statistics = admin!.ImpactStatistics!;
        var ids = statistics.Metrics.Select(m => m.Id).Reverse().ToList();

        var response = await _fixture.Client.PutAsJsonAsync(
            "/api/MainPage/metrics/reorder",
            new ReorderMetricsDto { StatisticId = statistics.Id, OrderedIds = ids });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var scope = _fixture.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<VictoryCenterDbContext>();
        var persisted = await db.Metrics.OrderBy(m => m.Priority).ToListAsync();
        Assert.Equal(ids, persisted.Select(m => m.Id));
        Assert.Equal([0, 1, 2, 3], persisted.Select(m => m.Priority));
        Assert.Equal(hidden ? 1 : 0, persisted.Count(m => m.IsHidden));
    }

    [SqlServerFact]
    public async Task Reorder_OmittingHiddenMetricIsRejectedWithoutPriorityChanges()
    {
        var page = await SeedAsync();
        var before = await SnapshotAsync();
        var response = await _fixture.Client.PutAsJsonAsync("/api/MainPage/metrics/reorder", new ReorderMetricsDto
        {
            StatisticId = page.ImpactStatistics!.Id,
            OrderedIds = [.. page.ImpactStatistics.Metrics.Where(m => !m.IsHidden).Select(m => m.Id).Reverse()],
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(before, await SnapshotAsync());
    }

    private async Task<DAL.Entities.MainPage> SeedAsync(int count = 4, bool hidden = true)
    {
        using var scope = _fixture.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<VictoryCenterDbContext>();
        var english = await db.LocalizationLanguages.SingleAsync(l => l.Code == "en");
        var page = new DAL.Entities.MainPage
        {
            Title = "Main page title",
            Description = "Main page description",
            ImpactStatistics = new ImpactStatistics
            {
                Title = "Statistics title",
                Metrics = [.. Enum.GetValues<MetricType>().Take(count).Select((type, index) => new Metric
                {
                    Name = type.ToString(),
                    Value = 123,
                    Type = type,
                    Prefix = MetricPrefix.Plus,
                    Priority = 10 - index,
                    IsHidden = hidden && type == MetricType.Programs,
                    IsAutoSynced = type == MetricType.Partners,
                    Localizations =
                    [
                        new MetricLocalization
                        {
                            LanguageId = english.Id,
                            Name = $"Translated {type}",
                            Value = "123 translated",
                            TranslationStatus = TranslationStatus.Relevant,
                        },
                    ],
                })],
            },
        };
        db.MainPages.Add(page);
        await db.SaveChangesAsync();
        return page;
    }

    private static UpdateMainPageDto ToUpdate(DAL.Entities.MainPage page) => new()
    {
        Title = page.Title,
        Description = page.Description,
        ImpactStatistics = new UpdateImpactStatisticDto
        {
            Id = page.ImpactStatistics!.Id,
            Title = page.ImpactStatistics.Title,
            Metrics = [.. page.ImpactStatistics.Metrics.Select(m => new UpdateMetricDto
            {
                Id = m.Id,
                Name = m.Name,
                Value = m.Value,
                Type = m.Type,
                Prefix = m.Prefix,
                IsAutoSynced = m.IsAutoSynced,
            })],
        },
    };

    private async Task<string[]> SnapshotAsync()
    {
        using var scope = _fixture.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<VictoryCenterDbContext>();
        var metrics = await db.Metrics.Include(m => m.Localizations).OrderBy(m => m.Id).ToListAsync();
        return [.. metrics.Select(m => $"{m.Id}|{m.StatisticId}|{m.CreatedAt:O}|{Convert.ToHexString(m.RowVersion)}|" +
            $"{m.Name}|{m.Value}|{m.Type}|{m.Prefix}|{m.Priority}|{m.IsHidden}|{m.IsAutoSynced}|" +
            string.Join(";", m.Localizations.OrderBy(l => l.LanguageId)
                .Select(l => $"{l.EntityId}|{l.LanguageId}|{l.Name}|{l.Value}|{l.TranslationStatus}")))];
    }
}
