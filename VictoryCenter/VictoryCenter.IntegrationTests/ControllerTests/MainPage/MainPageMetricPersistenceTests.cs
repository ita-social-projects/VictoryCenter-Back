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
    // Seeded in this order; a smaller count keeps the first entries.
    private static readonly (string Name, MetricKind Kind)[] SeedMetrics =
    [
        ("Partners", MetricKind.Standard),
        ("Programs", MetricKind.Standard),
        ("Raised", MetricKind.RaisedFunds),
        ("TherapyHours", MetricKind.Standard),
    ];

    private readonly MainPageSqlServerFixture _fixture;

    public MainPageMetricPersistenceTests(MainPageSqlServerFixture fixture)
    {
        _fixture = fixture;
    }

    public static TheoryData<int, string> InvalidMetricUpdates => new()
    {
        { 0, "null" },
        { 0, "nullElement" },
        { 0, "addMissing" },
        { 0, "unknownNewId" },
        { 0, "invalidNewKind" },
        { 0, "wrongStatisticsId" },
        { 3, "unchangedIncomplete" },
        { 3, "addMissing" },
        { 3, "empty" },
        { 3, "null" },
        { 3, "nullElement" },
        { 3, "invalidKind" },
        { 3, "duplicateId" },
        { 3, "unknownId" },
        { 3, "missingId" },
        { 3, "swappedIdsAcrossKinds" },
        { 3, "wrongStatisticsId" },
        { 4, "incomplete" },
        { 4, "empty" },
        { 4, "null" },
        { 4, "nullElement" },
        { 4, "noRaisedFunds" },
        { 4, "swappedKinds" },
        { 4, "replaceWithNewMetric" },
        { 4, "invalidKind" },
        { 4, "duplicateId" },
        { 4, "unknownId" },
        { 4, "missingId" },
        { 4, "swappedIdsAcrossKinds" },
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

    // Incomplete blocks are completed by startup seeding, so only page fields stay editable until then.
    [SqlServerTheory]
    [InlineData(0, false)]
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
            case "addMissing":
                metrics.AddRange(SeedMetrics.Skip(existingCount)
                    .Select(seed => new UpdateMetricDto { Name = seed.Name, Kind = seed.Kind, Value = 123 }));
                break;
            case "unknownNewId":
                metrics.Add(new UpdateMetricDto { Id = long.MaxValue, Name = "Hours", Kind = MetricKind.Standard });
                break;
            case "invalidNewKind":
                metrics.Add(new UpdateMetricDto { Name = "Invalid kind", Kind = (MetricKind)999 });
                break;
            case "incomplete":
                metrics.RemoveAll(m => m.Name == "Programs");
                break;
            case "empty":
                metrics.Clear();
                break;
            case "noRaisedFunds":
                metrics = [.. metrics.Select(m => m with { Kind = MetricKind.Standard })];
                break;
            case "swappedKinds":
                // Keeps a valid 3 Standard + 1 RaisedFunds composition, but kinds no longer match the stored ones.
                metrics[0] = metrics[0] with { Kind = MetricKind.RaisedFunds };
                metrics[2] = metrics[2] with { Kind = MetricKind.Standard };
                break;
            case "replaceWithNewMetric":
                metrics[0] = metrics[0] with { Id = null };
                break;
            case "invalidKind":
                metrics[0] = metrics[0] with { Kind = (MetricKind)999 };
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
            case "swappedIdsAcrossKinds":
                // Programs (Standard) and Raised (RaisedFunds) exchange IDs.
                (metrics[1], metrics[2]) = (metrics[1] with { Id = metrics[2].Id }, metrics[2] with { Id = metrics[1].Id });
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

    [SqlServerFact]
    public async Task Publish_RenamingStandardMetricsKeepsTheirIdentity()
    {
        var page = await SeedAsync();
        var dto = ToUpdate(page);
        var metrics = dto.ImpactStatistics!.Metrics.ToList();
        (metrics[0], metrics[1]) = (metrics[0] with { Name = metrics[1].Name }, metrics[1] with { Name = metrics[0].Name });
        dto = dto with { ImpactStatistics = dto.ImpactStatistics with { Metrics = metrics } };

        var response = await _fixture.Client.PutAsJsonAsync("/api/MainPage", dto);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var scope = _fixture.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<VictoryCenterDbContext>();
        var persisted = await db.Metrics.ToDictionaryAsync(m => m.Id);
        Assert.Equal(4, persisted.Count);
        Assert.Equal("Programs", persisted[metrics[0].Id!.Value].Name);
        Assert.Equal("Partners", persisted[metrics[1].Id!.Value].Name);
        Assert.All(page.ImpactStatistics!.Metrics, original => Assert.Equal(original.Kind, persisted[original.Id].Kind));
    }

    [SqlServerFact]
    public async Task Publish_NewStatisticsBlockCanBeCreatedWithoutMetricIds()
    {
        var page = await SeedAsync(0);
        var dto = ToUpdate(page);
        await RemoveStatisticsAsync();

        dto = dto with
        {
            ImpactStatistics = dto.ImpactStatistics! with
            {
                Id = null,
                Metrics = [.. SeedMetrics.Select(seed => new UpdateMetricDto { Name = seed.Name, Kind = seed.Kind, Value = 123 })],
            },
        };

        var response = await _fixture.Client.PutAsJsonAsync("/api/MainPage", dto);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var verificationScope = _fixture.Services.CreateScope();
        var persisted = verificationScope.ServiceProvider.GetRequiredService<VictoryCenterDbContext>();
        var metrics = await persisted.Metrics.ToListAsync();
        Assert.Equal(4, metrics.Count);
        Assert.Equal(3, metrics.Count(m => m.Kind == MetricKind.Standard));
        Assert.Single(metrics, m => m.Kind == MetricKind.RaisedFunds);
    }

    [SqlServerTheory]
    [InlineData(0)]
    [InlineData(3)]
    public async Task Publish_NewStatisticsBlockRejectsIncompleteMetricsWithoutDatabaseChanges(int requestedCount)
    {
        var page = await SeedAsync(0);
        var dto = ToUpdate(page);
        await RemoveStatisticsAsync();

        dto = dto with
        {
            Title = "Must not be persisted",
            ImpactStatistics = dto.ImpactStatistics! with
            {
                Id = null,
                Metrics = [.. SeedMetrics.Take(requestedCount)
                    .Select(seed => new UpdateMetricDto { Name = seed.Name, Kind = seed.Kind })],
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
        Assert.Equal(hidden.Kind, updated.Kind);
        Assert.True(updated.IsHidden);
        Assert.Equal("Translated Programs", updated.Localizations.Single().Name);
        Assert.Equal(TranslationStatus.Outdated, updated.Localizations.Single().TranslationStatus);
    }

    [SqlServerFact]
    public async Task Publish_LocalizationSaveFailureRollsBackPreviouslySavedFields()
    {
        var page = await SeedAsync();
        var before = await SnapshotAsync();
        var dto = ToUpdate(page);
        var metrics = dto.ImpactStatistics!.Metrics.ToList();
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
                Metrics = [.. SeedMetrics.Take(count).Select((seed, index) => new Metric
                {
                    Name = seed.Name,
                    Value = 123,
                    Kind = seed.Kind,
                    Prefix = MetricPrefix.Plus,
                    Priority = 10 - index,
                    IsHidden = hidden && seed.Name == "Programs",
                    IsAutoSynced = seed.Name == "Partners",
                    Localizations =
                    [
                        new MetricLocalization
                        {
                            LanguageId = english.Id,
                            Name = $"Translated {seed.Name}",
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

    private async Task RemoveStatisticsAsync()
    {
        using var scope = _fixture.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<VictoryCenterDbContext>();
        db.ImpactStatistics.Remove(await db.ImpactStatistics.SingleAsync());
        await db.SaveChangesAsync();
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
                Kind = m.Kind,
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
            $"{m.Name}|{m.Value}|{m.Kind}|{m.Prefix}|{m.Priority}|{m.IsHidden}|{m.IsAutoSynced}|" +
            string.Join(";", m.Localizations.OrderBy(l => l.LanguageId)
                .Select(l => $"{l.EntityId}|{l.LanguageId}|{l.Name}|{l.Value}|{l.TranslationStatus}")))];
    }
}
