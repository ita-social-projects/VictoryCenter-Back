using System.Data.Common;
using System.Diagnostics;
using System.Text.Json;
using AutoMapper;
using FluentResults;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using VictoryCenter.BLL.DTOs.Admin.Localization.MainPage;
using VictoryCenter.BLL.Errors;
using VictoryCenter.BLL.Mapping.Localization.Languages;
using VictoryCenter.BLL.Mapping.Localization.MainPage;
using VictoryCenter.BLL.Queries.Admin.Localization.MainPage.GetByLanguageId;
using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Options;
using VictoryCenter.DAL.Repositories.Realizations.Base;
using Xunit.Abstractions;

namespace VictoryCenter.UnitTests.DataTests;

public class MainPageLocalizationSqlTests(ITestOutputHelper output) : IAsyncLifetime
{
    private const string ConnectionVariable = "VICTORYCENTER_MAIN_LOCALIZATION_SQL_TEST_CONNECTION";
    private readonly string _database = "vc_main_localization_" + Guid.NewGuid().ToString("N");
    private readonly CommandCounter _counter = new();
    private readonly IMapper _mapper = new MapperConfiguration(config =>
    {
        config.AddProfile<MainPageLocalizationProfile>();
        config.AddProfile<LocalizationsLanguageProfile>();
    }).CreateMapper();
    private string _connection = null!;
    private long[] _pageIds = [];

    private long EnglishId { get; set; }
    private long UkrainianId { get; set; }

    public async Task InitializeAsync()
    {
        var connection = new SqlConnectionStringBuilder(Environment.GetEnvironmentVariable(ConnectionVariable));
        if ((connection.DataSource != "127.0.0.1" && !connection.DataSource.StartsWith("127.0.0.1,", StringComparison.Ordinal))
            || connection.InitialCatalog != "master" || !string.IsNullOrEmpty(connection.AttachDBFilename))
        {
            throw new InvalidOperationException("Use a disposable loopback SQL Server with Initial Catalog=master.");
        }

        connection.InitialCatalog = _database;
        _connection = connection.ConnectionString;
        await using var context = Context();
        await context.Database.EnsureCreatedAsync();
        var ukrainian = new LocalizationLanguage { Code = "uk", Name = "Ukrainian" };
        var english = new LocalizationLanguage { Code = "en", Name = "English" };
        context.AddRange(ukrainian, english);
        await context.SaveChangesAsync();
        var pages = new[] { new DAL.Entities.MainPage { Title = "Page", Description = "Source" } };
        foreach (var page in pages)
        {
            page.Localizations.Add(new MainPageLocalization { Language = ukrainian, Title = "UK title", Description = "UK description" });
            page.Localizations.Add(new MainPageLocalization { Language = english, Title = "EN title", Description = "EN description" });

            page.MainAboutUs = new MainAboutUs
            {
                Title = "About", Description = "Source", Localizations =
                [new MainAboutUsLocalization { Language = english, Title = null, Description = "About EN" }]
            };
            page.MainPartners = new MainPartners
            {
                Title = "Partners", Description = "Source", Localizations =
                [new MainPartnersLocalization { Language = english, Title = "Partners translated", Description = null }]
            };
            page.MainDonations = new MainDonations { Title = "Donations", Description = "Source" };
            page.MainDonations.Localizations.Add(new MainDonationsLocalization { Language = english, Title = "Donations EN", Description = "Description" });
        }

        context.AddRange(pages);
        await context.SaveChangesAsync();
        await context.Set<MainAboutUsLocalization>().ExecuteUpdateAsync(setters => setters.SetProperty(l => l.TranslationStatus, TranslationStatus.Outdated));
        _pageIds = pages.Select(p => p.Id).ToArray();
        EnglishId = english.Id;
        UkrainianId = ukrainian.Id;
    }

    [LocalSqlFact]
    public async Task Projection_PreservesAllResponsesAndErrors_WithOneCommandAndNoTracking()
    {
        var requests = _pageIds.SelectMany(id => new[] { (id, EnglishId), (id, UkrainianId), (id, long.MaxValue), (id, 0L), (id, -1L) })
            .Concat(new[] { (long.MaxValue, EnglishId), (0L, EnglishId), (-1L, EnglishId) });
        foreach (var (id, language) in requests)
        {
            await using var context = Context();
            var expected = await LegacyAsync(context, id, language);
            Assert.Empty(context.ChangeTracker.Entries());
            _counter.Count = 0;
            var actual = await CurrentAsync(context, id, language);
            Equal(expected, actual);
            Assert.Equal(1, _counter.Count);
            Assert.Empty(context.ChangeTracker.Entries());
        }

        await using var check = Context();
        var full = (await CurrentAsync(check, _pageIds[0], EnglishId)).Value;
        Assert.Equal(TranslationStatus.Outdated, full.MainAboutUs!.TranslationStatus);
        Assert.Null(full.MainAboutUs.Title);
        Assert.Null(full.MainPartners!.Description);
        Assert.Equal("en", full.MainDonations!.LocalizationInfoDto.Code);
        await check.Set<MainPartnersLocalization>().ExecuteDeleteAsync();
        await check.Set<MainDonationsLocalization>().ExecuteDeleteAsync();
        Equal(await LegacyAsync(check, _pageIds[0], EnglishId), await CurrentAsync(check, _pageIds[0], EnglishId));
        var partial = (await CurrentAsync(check, _pageIds[0], EnglishId)).Value;
        Assert.NotNull(partial.MainAboutUs);
        Assert.Null(partial.MainPartners);
        Assert.Null(partial.MainDonations);

        await check.Set<MainAboutUsLocalization>().ExecuteDeleteAsync();
        await check.Set<MainAboutUs>().ExecuteDeleteAsync();
        await check.Set<MainPartners>().ExecuteDeleteAsync();
        await check.Set<MainDonations>().ExecuteDeleteAsync();
        Equal(await LegacyAsync(check, _pageIds[0], EnglishId), await CurrentAsync(check, _pageIds[0], EnglishId));
        var empty = (await CurrentAsync(check, _pageIds[0], EnglishId)).Value;
        Assert.Null(empty.MainAboutUs);
        Assert.Null(empty.MainPartners);
        Assert.Null(empty.MainDonations);

        await check.Set<MainPageLocalization>().Where(l => l.LanguageId == EnglishId).ExecuteDeleteAsync();
        Equal(await LegacyAsync(check, _pageIds[0], EnglishId), await CurrentAsync(check, _pageIds[0], EnglishId));
    }

    [LocalSqlFact]
    public async Task Projection_PreservesAll54OptionalBlockAndRootTranslationCombinations()
    {
        await using var context = Context();
        await context.Set<MainPageLocalization>().Where(l => l.LanguageId == EnglishId)
            .ExecuteUpdateAsync(setters => setters.SetProperty(l => l.Title, (string?)null).SetProperty(l => l.Description, string.Empty));
        foreach (var rootTranslated in new[] { true, false })
        {
            if (!rootTranslated)
            {
                await context.Set<MainPageLocalization>().Where(l => l.LanguageId == EnglishId).ExecuteDeleteAsync();
            }

            // Each block can be absent, untranslated or translated: 3^3 combinations per root state.
            for (var combination = 0; combination < 27; combination++)
            {
                await context.Set<MainAboutUsLocalization>().ExecuteDeleteAsync();
                await context.Set<MainPartnersLocalization>().ExecuteDeleteAsync();
                await context.Set<MainDonationsLocalization>().ExecuteDeleteAsync();
                await context.Set<MainAboutUs>().ExecuteDeleteAsync();
                await context.Set<MainPartners>().ExecuteDeleteAsync();
                await context.Set<MainDonations>().ExecuteDeleteAsync();
                var aboutState = combination % 3;
                var partnerState = (combination / 3) % 3;
                var donationState = combination / 9;
                if (aboutState > 0)
                {
                    var block = new MainAboutUs { MainPageId = _pageIds[0], Title = "About", Description = "Source" };
                    if (aboutState == 2)
                    {
                        block.Localizations.Add(new MainAboutUsLocalization { LanguageId = EnglishId, Title = null, Description = "<p>Translation & details</p>" });
                    }

                    context.Add(block);
                }

                if (partnerState > 0)
                {
                    var block = new MainPartners { MainPageId = _pageIds[0], Title = "Partners", Description = "Source" };
                    if (partnerState == 2)
                    {
                        block.Localizations.Add(new MainPartnersLocalization { LanguageId = EnglishId, Title = "Partners EN", Description = null });
                    }

                    context.Add(block);
                }

                if (donationState > 0)
                {
                    var block = new MainDonations { MainPageId = _pageIds[0], Title = "Donations", Description = "Source" };
                    if (donationState == 2)
                    {
                        block.Localizations.Add(new MainDonationsLocalization { LanguageId = EnglishId, Title = "Donations EN", Description = string.Empty });
                    }

                    context.Add(block);
                }

                await context.SaveChangesAsync();
                await context.Set<MainAboutUsLocalization>().ExecuteUpdateAsync(setters => setters.SetProperty(l => l.TranslationStatus, TranslationStatus.Outdated));
                context.ChangeTracker.Clear();
                var expected = await LegacyAsync(context, _pageIds[0], EnglishId);
                _counter.Count = 0;
                var actual = await CurrentAsync(context, _pageIds[0], EnglishId);
                Equal(expected, actual);
                Assert.Equal(rootTranslated, actual.IsSuccess);
                Assert.Equal(1, _counter.Count);
                Assert.Empty(context.ChangeTracker.Entries());
                if (rootTranslated)
                {
                    Assert.Equal(aboutState == 2, actual.Value.MainAboutUs is not null);
                    Assert.Equal(partnerState == 2, actual.Value.MainPartners is not null);
                    Assert.Equal(donationState == 2, actual.Value.MainDonations is not null);
                }
            }
        }

        output.WriteLine("Verified all 54 optional-block/root-translation combinations: identical DTOs or typed errors, one SQL command, no tracking.");
    }

    [LocalSqlFact]
    public async Task Projection_PropagatesCancellation()
    {
        await using var context = Context();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            new RepositoryWrapper(context).MainPageRepository.GetLocalizationAsync(_pageIds[0], EnglishId, new CancellationToken(true)));
    }

    [LocalSqlFact]
    public async Task CompareSequentialReadsWithProjection()
    {
        var before = new List<double>();
        var after = new List<double>();
        for (var iteration = -2; iteration < 9; iteration++)
        {
            foreach (var legacy in iteration % 2 == 0 ? new[] { true, false } : new[] { false, true })
            {
                await using var context = Context();
                await context.Database.OpenConnectionAsync();
                _counter.Count = 0;
                var timer = Stopwatch.StartNew();
                var result = legacy ? await LegacyAsync(context, _pageIds[0], EnglishId) : await CurrentAsync(context, _pageIds[0], EnglishId);
                timer.Stop();
                Assert.True(result.IsSuccess);
                Assert.Equal(legacy ? 5 : 1, _counter.Count);
                if (iteration >= 0)
                {
                    (legacy ? before : after).Add(timer.Elapsed.TotalMilliseconds);
                }
            }
        }

        output.WriteLine("SQL version: " + await ServerVersionAsync());
        output.WriteLine("Runtime: " + System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription);
        output.WriteLine("EF Core: " + typeof(DbContext).Assembly.GetName().Version);
        output.WriteLine("Before ms: " + JsonSerializer.Serialize(before));
        output.WriteLine("After ms: " + JsonSerializer.Serialize(after));
        output.WriteLine($"Median ms: {before.Order().ElementAt(4):F3} -> {after.Order().ElementAt(4):F3}; commands: 5 -> 1");
    }

    public async Task DisposeAsync()
    {
        if (_connection is null)
        {
            return;
        }

        SqlConnection.ClearAllPools();
        await using var context = Context();
        await context.Database.EnsureDeletedAsync();
    }

    private VictoryCenterDbContext Context() => new(new DbContextOptionsBuilder<VictoryCenterDbContext>()
        .UseSqlServer(_connection).AddInterceptors(_counter).Options);

    private Task<Result<MainPageLocalizationDto>> CurrentAsync(VictoryCenterDbContext context, long id, long language) =>
        new GetMainPageLocalizationByLanguageIdHandler(new RepositoryWrapper(context)).Handle(new GetMainPageLocalizationByLanguageIdQuery(id, language), CancellationToken.None);

    // The pre-optimization read path from release c68d2fcf, retained only for equivalence and measurement.
    private async Task<Result<MainPageLocalizationDto>> LegacyAsync(VictoryCenterDbContext context, long id, long language)
    {
        var wrapper = new RepositoryWrapper(context);
        var page = await wrapper.MainPageRepository.GetFirstOrDefaultAsync(new QueryOptions<DAL.Entities.MainPage>
        {
            Filter = p => p.Id == id, AsNoTracking = true,
            Include = q => q.Include(p => p.MainAboutUs).Include(p => p.MainPartners).Include(p => p.MainDonations)
        });
        if (page is null)
        {
            return Result.Fail(NotFoundErrors.NotFound(id, typeof(DAL.Entities.MainPage)));
        }

        var translation = await wrapper.MainPageLocalizationsRepository.GetFirstOrDefaultAsync(new QueryOptions<MainPageLocalization>
        {
            Filter = l => l.EntityId == id && l.LanguageId == language, AsNoTracking = true, Include = q => q.Include(l => l.Language)
        });
        if (translation is null)
        {
            return Result.Fail(NotFoundErrors.NotFound((id, language), typeof(MainPageLocalization)));
        }

        var about = page.MainAboutUs is null ? null : await wrapper.MainAboutUsLocalizationsRepository.GetFirstOrDefaultAsync(new QueryOptions<MainAboutUsLocalization>
        {
            Filter = l => l.EntityId == page.MainAboutUs.Id && l.LanguageId == language, AsNoTracking = true, Include = q => q.Include(l => l.Language)
        });
        var partners = page.MainPartners is null ? null : await wrapper.MainPartnersLocalizationsRepository.GetFirstOrDefaultAsync(new QueryOptions<MainPartnersLocalization>
        {
            Filter = l => l.EntityId == page.MainPartners.Id && l.LanguageId == language, AsNoTracking = true, Include = q => q.Include(l => l.Language)
        });
        var donations = page.MainDonations is null ? null : await wrapper.MainDonationsLocalizationsRepository.GetFirstOrDefaultAsync(new QueryOptions<MainDonationsLocalization>
        {
            Filter = l => l.EntityId == page.MainDonations.Id && l.LanguageId == language, AsNoTracking = true, Include = q => q.Include(l => l.Language)
        });
        return Result.Ok(_mapper.Map<MainPageLocalizationDto>(translation) with
        {
            MainAboutUs = _mapper.Map<MainAboutUsLocalizationDto?>(about),
            MainPartners = _mapper.Map<MainPartnersLocalizationDto?>(partners),
            MainDonations = _mapper.Map<MainDonationsLocalizationDto?>(donations)
        });
    }

    private static void Equal(Result<MainPageLocalizationDto> expected, Result<MainPageLocalizationDto> actual)
    {
        Assert.Equal(expected.IsSuccess, actual.IsSuccess);
        Assert.Equal(expected.Errors.Select(e => e.Message), actual.Errors.Select(e => e.Message));
        Assert.Equal(expected.Errors.Select(e => e.GetType()), actual.Errors.Select(e => e.GetType()));
        if (expected.IsSuccess)
        {
            Assert.Equal(JsonSerializer.Serialize(expected.Value), JsonSerializer.Serialize(actual.Value));
        }
    }

    private async Task<string> ServerVersionAsync()
    {
        await using var connection = new SqlConnection(_connection);
        await connection.OpenAsync();
        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT @@VERSION";
        return (string)(await command.ExecuteScalarAsync())!;
    }

    private sealed class LocalSqlFactAttribute : FactAttribute
    {
        public LocalSqlFactAttribute()
        {
            if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(ConnectionVariable)))
            {
                Skip = $"Set {ConnectionVariable} to a disposable loopback SQL Server master connection.";
            }
        }
    }

    private sealed class CommandCounter : DbCommandInterceptor
    {
        public int Count { get; set; }

        public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(
            DbCommand command,
            CommandEventData eventData,
            InterceptionResult<DbDataReader> result,
            CancellationToken cancellationToken = default)
        {
            Count++;
            return ValueTask.FromResult(result);
        }
    }
}
