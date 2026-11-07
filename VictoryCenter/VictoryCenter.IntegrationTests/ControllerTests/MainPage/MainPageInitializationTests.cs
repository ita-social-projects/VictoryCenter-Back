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
        Assert.Equal(Enum.GetValues<MetricType>(), metrics.Select(m => m.Type));
        Assert.Equal(["Партнерів", "Програм", "Зібрано", "Годин терапії"], metrics.Select(m => m.Name));
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
        Assert.Equal(Enum.GetValues<MetricType>(), page.ImpactStatistics!.Metrics.Select(m => m.Type));
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

    [SqlServerTheory]
    [InlineData(4)]
    [InlineData(3)]
    [InlineData(0)]
    public async Task Initialize_ExistingCompleteOrIncompletePage_PreservesAllData(int metricCount)
    {
        await InitializeMainPageAsync();
        using (var scope = _fixture.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<VictoryCenterDbContext>();
            var page = await db.MainPages.Include(p => p.ImpactStatistics)!.ThenInclude(s => s!.Metrics).SingleAsync();
            page.Title = "Existing main page";
            page.ImpactStatistics!.Title = "Existing statistics";
            var metrics = page.ImpactStatistics.Metrics.OrderBy(m => m.Priority).ToList();
            var english = await db.LocalizationLanguages.SingleAsync(l => l.Code == "en");
            foreach (var metric in metrics.Take(metricCount))
            {
                metric.Name = $"Existing {metric.Type}";
                metric.Value = 123;
                metric.Priority = 10 - metric.Priority;
                metric.Prefix = MetricPrefix.Plus;
                metric.IsHidden = true;
                metric.IsAutoSynced = true;
                metric.Localizations.Add(new MetricLocalization
                {
                    LanguageId = english.Id,
                    Name = $"Translated {metric.Type}",
                    Value = "Translated value",
                    TranslationStatus = TranslationStatus.Relevant,
                });
            }

            db.Metrics.RemoveRange(metrics.Skip(metricCount));
            await db.SaveChangesAsync();
        }

        var before = await SnapshotAsync();

        await InitializeMainPageAsync();
        await InitializeMainPageAsync();
        await _fixture.Client.GetAsync("/api/MainPage");
        await _fixture.Client.GetAsync("/api/MainPage/admin");

        Assert.Equal(before, await SnapshotAsync());
        using var verificationScope = _fixture.Services.CreateScope();
        var verificationDb = verificationScope.ServiceProvider.GetRequiredService<VictoryCenterDbContext>();
        Assert.Equal(metricCount, await verificationDb.Metrics.CountAsync());
        Assert.Equal("Existing main page", (await verificationDb.MainPages.SingleAsync()).Title);
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
                    Type = m.Type,
                    Value = m.Type == MetricType.Programs ? 42 : m.Value,
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
        Assert.Equal(42, persisted.Single(m => m.Type == MetricType.Programs).Value);
    }

    [SqlServerFact]
    public async Task Initialize_ConcurrentStartups_PersistOnlyOneCompleteAggregate()
    {
        var barrier = new ConcurrentSaveInterceptor();

        await Task.WhenAll(InitializeMainPageAsync(barrier), InitializeMainPageAsync(barrier));

        await AssertSingleAggregateAsync();
    }

    private async Task InitializeMainPageAsync(SaveChangesInterceptor? interceptor = null)
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
        Assert.Equal(Enum.GetValues<MetricType>(), await db.Metrics.OrderBy(m => m.Priority).Select(m => m.Type).ToArrayAsync());
    }

    private async Task<string> SnapshotAsync()
    {
        var page = await _fixture.Client.GetFromJsonAsync<MainPageDto>("/api/MainPage/admin");
        return JsonSerializer.Serialize(page);
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
