using System.Data.Common;
using System.Net;
using System.Net.Http.Json;
using System.Reflection;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using VictoryCenter.BLL.DTOs.Admin.ImpactStatistics;
using VictoryCenter.BLL.DTOs.Admin.ImpactStatistics.Metrics;
using VictoryCenter.BLL.DTOs.Admin.MainPages;
using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.WebAPI.Extensions;

namespace VictoryCenter.IntegrationTests.ControllerTests.MainPage;

[Collection("SharedIntegrationTests")]
public class MainPageInitializationTests : IClassFixture<MainPageSqlServerFixture>, IAsyncLifetime
{
    // Seeded defaults in priority order.
    private const string PartnersName = "Партнерів";
    private const string ProgramsName = "Програм";
    private const string TherapyHoursName = "Годин терапії";
    private static readonly MetricKind[] DefaultKinds =
        [MetricKind.Standard, MetricKind.Standard, MetricKind.RaisedFunds, MetricKind.Standard];

    private readonly MainPageSqlServerFixture _fixture;

    public MainPageInitializationTests(MainPageSqlServerFixture fixture)
    {
        _fixture = fixture;
    }

    public async Task InitializeAsync()
    {
        using var scope = _fixture.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<VictoryCenterDbContext>();
        db.MainPages.RemoveRange(await db.MainPages.ToListAsync());
        await db.SaveChangesAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [SqlServerFact]
    public async Task Initialize_EmptyDatabase_PersistsFourDefaultMetricsWithTheirAggregate()
    {
        await InitializeMainPageAsync();

        using var scope = _fixture.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<VictoryCenterDbContext>();
        var page = await db.MainPages.Include(p => p.ImpactStatistics)!.ThenInclude(s => s!.Metrics)
            .ThenInclude(m => m.Localizations).SingleAsync();
        var statistics = page.ImpactStatistics!;
        var metrics = statistics.Metrics.OrderBy(m => m.Priority).ToList();

        Assert.Equal(4, await db.Metrics.CountAsync());
        Assert.Equal(DefaultKinds, metrics.Select(m => m.Kind));
        Assert.Equal([PartnersName, ProgramsName, "Зібрано", TherapyHoursName], metrics.Select(m => m.Name));
        Assert.Equal([0L, 1L, 2L, 3L], metrics.Select(m => m.Priority));
        Assert.Equal(4, metrics.Select(m => m.Id).Distinct().Count());
        Assert.All(metrics, metric =>
        {
            Assert.True(metric.Id > 0);
            Assert.Equal(statistics.Id, metric.StatisticId);
            Assert.Equal(0, metric.Value);
            Assert.False(metric.IsHidden);
            Assert.False(metric.IsAutoSynced);
            Assert.Null(metric.Prefix);
            Assert.Empty(metric.Localizations);
            Assert.NotEqual(default, metric.CreatedAt);
            Assert.NotEmpty(metric.RowVersion);
        });
    }

    [SqlServerTheory]
    [InlineData("/api/MainPage")]
    [InlineData("/api/MainPage/admin")]
    public async Task Initialize_DefaultMetricsAreReturnedByGet(string endpoint)
    {
        await InitializeMainPageAsync();

        var page = await _fixture.Client.GetFromJsonAsync<MainPageDto>(endpoint);

        Assert.NotNull(page);
        Assert.Equal(DefaultKinds, page.ImpactStatistics!.Metrics.Select(m => m.Kind));
        Assert.All(page.ImpactStatistics.Metrics, metric =>
        {
            Assert.True(metric.Id > 0);
            Assert.Equal(0, metric.Value);
            Assert.False(metric.IsHidden);
            Assert.False(metric.IsAutoSynced);
        });
    }

    [SqlServerFact]
    public async Task Initialize_RepeatedStartup_DoesNotDuplicateOrChangeData()
    {
        await InitializeMainPageAsync();
        var before = await SnapshotAsync();

        await InitializeMainPageAsync();
        await InitializeMainPageAsync();

        Assert.Equal(before, await SnapshotAsync());
        await AssertSingleAggregateAsync();
    }

    [SqlServerFact]
    public async Task Initialize_IncompletePage_AddsOnlyMissingMetricAndPreservesExistingMetrics()
    {
        await InitializeMainPageAsync();
        using (var scope = _fixture.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<VictoryCenterDbContext>();
            var metrics = await db.Metrics.ToListAsync();
            var english = await db.LocalizationLanguages.SingleAsync(l => l.Code == "en");
            foreach (var metric in metrics.Where(m => m.Name != PartnersName))
            {
                metric.Name = $"Existing {metric.Name}";
                metric.Value = 123;
                metric.Priority += 10;
                metric.Prefix = MetricPrefix.Plus;
                metric.IsHidden = true;
                metric.IsAutoSynced = true;
                metric.Localizations.Add(new MetricLocalization
                {
                    LanguageId = english.Id,
                    Name = $"Translated {metric.Name}",
                    Value = "Translated value",
                    TranslationStatus = TranslationStatus.Relevant,
                });
            }

            db.Metrics.Remove(metrics.Single(m => m.Name == PartnersName));
            await db.SaveChangesAsync();
        }

        var before = await MetricsSnapshotAsync(excludePartners: true);
        await InitializeMainPageAsync();
        var completePageBefore = await MetricsSnapshotAsync();
        await InitializeMainPageAsync();

        Assert.Equal(before, await MetricsSnapshotAsync(excludePartners: true));
        Assert.Equal(completePageBefore, await MetricsSnapshotAsync());
        using var verificationScope = _fixture.Services.CreateScope();
        var dbAfter = verificationScope.ServiceProvider.GetRequiredService<VictoryCenterDbContext>();
        var metricsAfter = await dbAfter.Metrics.Include(m => m.Localizations).ToListAsync();
        Assert.Equal(4, metricsAfter.Count);
        var partner = Assert.Single(metricsAfter, m => m.Name == PartnersName);
        Assert.Equal(MetricKind.Standard, partner.Kind);
        Assert.Equal(14, partner.Priority);
        Assert.Equal(0, partner.Value);
        Assert.False(partner.IsHidden);
        Assert.False(partner.IsAutoSynced);
        Assert.Null(partner.Prefix);
        Assert.Empty(partner.Localizations);
    }

    [SqlServerFact]
    public async Task Initialize_IncompletePage_PrefersDefaultNamesNotUsedInTheBlock()
    {
        await InitializeMainPageAsync();
        using (var scope = _fixture.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<VictoryCenterDbContext>();
            db.Metrics.RemoveRange(await db.Metrics
                .Where(m => m.Name == ProgramsName || m.Name == TherapyHoursName)
                .ToListAsync());
            await db.SaveChangesAsync();
        }

        await InitializeMainPageAsync();

        using var verificationScope = _fixture.Services.CreateScope();
        var dbAfter = verificationScope.ServiceProvider.GetRequiredService<VictoryCenterDbContext>();
        var metrics = await dbAfter.Metrics.ToListAsync();
        Assert.Equal(4, metrics.Count);
        Assert.Equal(
            new[] { PartnersName, ProgramsName, "Зібрано", TherapyHoursName }.Order(),
            metrics.Select(m => m.Name).Order());
        Assert.Equal(3, metrics.Count(m => m.Kind == MetricKind.Standard));
        Assert.Single(metrics, m => m.Kind == MetricKind.RaisedFunds);
    }

    [SqlServerTheory]
    [InlineData(MetricKind.Standard, MetricKind.Standard)]
    [InlineData(MetricKind.RaisedFunds, MetricKind.RaisedFunds)]
    public async Task Initialize_BlockWithExcessMetricsOfOneKind_IsLeftUnchanged(MetricKind firstKind, MetricKind thirdKind)
    {
        // Legacy data: 4 Standard metrics, or 2 Standard + 2 RaisedFunds. Seeding must not add a fifth metric.
        await InitializeMainPageAsync();
        using (var scope = _fixture.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<VictoryCenterDbContext>();
            var metrics = await db.Metrics.OrderBy(m => m.Priority).ToListAsync();
            metrics[0].Kind = firstKind;
            metrics[2].Kind = thirdKind;
            await db.SaveChangesAsync();
        }

        var before = await MetricsSnapshotAsync();

        await InitializeMainPageAsync();

        Assert.Equal(before, await MetricsSnapshotAsync());
    }

    [SqlServerFact]
    public async Task Initialize_ConcurrentIncompletePage_DoesNotDuplicateMetrics()
    {
        await InitializeMainPageAsync();
        using (var scope = _fixture.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<VictoryCenterDbContext>();
            db.Metrics.Remove(await db.Metrics.SingleAsync(m => m.Name == PartnersName));
            await db.SaveChangesAsync();
        }

        var barrier = new ConcurrentReadInterceptor();
        await Task.WhenAll(InitializeMainPageAsync(barrier), InitializeMainPageAsync(barrier));

        using var verificationScope = _fixture.Services.CreateScope();
        var dbAfter = verificationScope.ServiceProvider.GetRequiredService<VictoryCenterDbContext>();
        var kinds = await dbAfter.Metrics.Select(m => m.Kind).ToListAsync();
        Assert.Equal(4, kinds.Count);
        Assert.Equal(3, kinds.Count(kind => kind == MetricKind.Standard));
        Assert.Single(kinds, kind => kind == MetricKind.RaisedFunds);
    }

    [SqlServerFact]
    public async Task Initialize_ExistingPageWithoutStatistics_DoesNotCreateStatistics()
    {
        using (var scope = _fixture.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<VictoryCenterDbContext>();
            db.MainPages.Add(new DAL.Entities.MainPage { Title = "Existing page", Description = "Existing description" });
            await db.SaveChangesAsync();
        }

        await InitializeMainPageAsync();

        using var verificationScope = _fixture.Services.CreateScope();
        var verificationDb = verificationScope.ServiceProvider.GetRequiredService<VictoryCenterDbContext>();
        Assert.Single(await verificationDb.MainPages.ToListAsync());
        Assert.Empty(await verificationDb.ImpactStatistics.ToListAsync());
        Assert.Empty(await verificationDb.Metrics.ToListAsync());
    }

    [SqlServerFact]
    public async Task Initialize_SeededMetrics_CanBePublishedWithExistingUpdateValidation()
    {
        await InitializeMainPageAsync();
        var page = (await _fixture.Client.GetFromJsonAsync<MainPageDto>("/api/MainPage/admin"))!;
        var metrics = page.ImpactStatistics!.Metrics.ToList();
        var update = new UpdateMainPageDto
        {
            Title = page.Title,
            Description = page.Description,
            ImpactStatistics = new UpdateImpactStatisticDto
            {
                Id = page.ImpactStatistics.Id,
                Title = "Зміни, які можна виміряти",
                Metrics = [.. metrics.Select(m => new UpdateMetricDto
                {
                    Id = m.Id,
                    Name = m.Name,
                    Kind = m.Kind,
                    Value = m.Name == ProgramsName ? 42 : m.Value,
                    Prefix = m.Prefix,
                    IsAutoSynced = m.IsAutoSynced,
                })],
            },
        };

        var response = await _fixture.Client.PutAsJsonAsync("/api/MainPage", update);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var scope = _fixture.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<VictoryCenterDbContext>();
        var persisted = await db.Metrics.OrderBy(m => m.Priority).ToListAsync();
        Assert.Equal(metrics.Select(m => m.Id), persisted.Select(m => m.Id));
        Assert.Equal(42, persisted.Single(m => m.Name == ProgramsName).Value);
    }

    [SqlServerFact]
    public async Task Initialize_ConcurrentStartups_PersistOnlyOneCompleteAggregate()
    {
        var barrier = new ConcurrentSaveInterceptor();

        await Task.WhenAll(InitializeMainPageAsync(barrier), InitializeMainPageAsync(barrier));

        await AssertSingleAggregateAsync();
    }

    private async Task InitializeMainPageAsync(IInterceptor? interceptor = null)
    {
        using var scope = _fixture.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<VictoryCenterDbContext>();
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddSingleton(TimeProvider.System);
        builder.Services.AddDbContext<VictoryCenterDbContext>(options =>
        {
            options.UseSqlServer(db.Database.GetConnectionString());
            if (interceptor is not null)
            {
                options.AddInterceptors(interceptor);
            }
        });
        await using var app = builder.Build();
        var initialize = typeof(ServicesConfiguration).GetMethod("CreateInitialMainPageAsync", BindingFlags.NonPublic | BindingFlags.Static)!;
        await (Task)initialize.Invoke(null, [app])!;
    }

    private async Task AssertSingleAggregateAsync()
    {
        using var scope = _fixture.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<VictoryCenterDbContext>();
        Assert.Equal(1, await db.MainPages.CountAsync());
        Assert.Equal(1, await db.ImpactStatistics.CountAsync());
        Assert.Equal(1, await db.MainAboutUs.CountAsync());
        Assert.Equal(1, await db.MainPartners.CountAsync());
        Assert.Equal(1, await db.MainDonations.CountAsync());
        Assert.Equal(4, await db.Metrics.CountAsync());
        Assert.Equal(DefaultKinds, await db.Metrics.OrderBy(m => m.Priority).Select(m => m.Kind).ToArrayAsync());
    }

    private async Task<string> SnapshotAsync()
    {
        var page = await _fixture.Client.GetFromJsonAsync<MainPageDto>("/api/MainPage/admin");
        return JsonSerializer.Serialize(page);
    }

    private async Task<string> MetricsSnapshotAsync(bool excludePartners = false)
    {
        using var scope = _fixture.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<VictoryCenterDbContext>();
        var metrics = await db.Metrics
            .Where(m => !excludePartners || m.Name != PartnersName).OrderBy(m => m.Id)
            .Select(m => new
            {
                m.Id,
                m.Kind,
                m.Name,
                m.Value,
                m.Priority,
                m.IsHidden,
                m.IsAutoSynced,
                m.Prefix,
                m.CreatedAt,
                m.RowVersion,
                Localizations = m.Localizations.OrderBy(l => l.LanguageId).Select(l => new
                {
                    l.LanguageId,
                    l.Name,
                    l.Value,
                    l.TranslationStatus,
                }).ToList(),
            })
            .ToListAsync();
        return JsonSerializer.Serialize(metrics);
    }

    private sealed class ConcurrentReadInterceptor : DbCommandInterceptor
    {
        private readonly TaskCompletionSource _ready = new(TaskCreationOptions.RunContinuationsAsynchronously);
        private int _arrivals;

        public override async ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(
            DbCommand command,
            CommandEventData eventData,
            InterceptionResult<DbDataReader> result,
            CancellationToken cancellationToken = default)
        {
            if (command.CommandText.Contains("SELECT * FROM MainPages", StringComparison.Ordinal))
            {
                if (Interlocked.Increment(ref _arrivals) == 2)
                {
                    _ready.SetResult();
                }

                await _ready.Task.WaitAsync(TimeSpan.FromSeconds(30), cancellationToken);
            }

            return result;
        }
    }

    private sealed class ConcurrentSaveInterceptor : SaveChangesInterceptor
    {
        private readonly TaskCompletionSource _ready = new(TaskCreationOptions.RunContinuationsAsynchronously);
        private int _arrivals;

        public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            if (Interlocked.Increment(ref _arrivals) == 2)
            {
                _ready.SetResult();
            }

            await _ready.Task.WaitAsync(TimeSpan.FromSeconds(30), cancellationToken);
            return result;
        }
    }
}
