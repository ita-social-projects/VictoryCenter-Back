using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Entities.WhoWeAreContents;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Realizations.WhoWeAreSections;

namespace VictoryCenter.UnitTests.RepositoryTests;

public class WhoWeAreSectionPreviewsTests
{
    [Fact]
    public async Task GetPreviewRows_EmptyDatabaseReturnsNoRows()
    {
        await using var context = Context();

        Assert.Empty(await new WhoWeAreSectionsRepository(context).GetPreviewRowsAsync());
    }

    [Fact]
    public async Task GetPreviewRows_KeepsEmptyImageOnlyAndUntranslatedSections()
    {
        await using var context = Context();
        context.Set<WhoWeAreSection>().AddRange(
            Section(1),
            Section(2, new ImageContent { Localizations = [Localization(2, TranslationStatus.Outdated)] }),
            Section(3, new TitleContent { Title = "Untranslated" }));
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var rows = await new WhoWeAreSectionsRepository(context).GetPreviewRowsAsync();

        Assert.Equal(3, rows.Count);
        Assert.Equal(new long[] { 1, 2, 3 }, rows.Select(row => row.Id));
        Assert.All(rows, row =>
        {
            Assert.Equal("Section " + row.Id, row.Title);
            Assert.Null(row.LanguageId);
            Assert.Null(row.TranslationStatus);
        });
        Assert.Empty(context.ChangeTracker.Entries());
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task GetPreviewRows_AggregatesOnlyCompleteNonImageTranslations(bool outdated)
    {
        await using var context = Context();
        context.Set<WhoWeAreSection>().Add(Section(
            1,
            new TitleContent
            {
                Title = "Title",
                Localizations = [Localization(2), Localization(3, TranslationStatus.Outdated)]
            },
            new DescriptionContent
            {
                Description = new string('x', 10000),
                Localizations = [Localization(2, outdated ? TranslationStatus.Outdated : TranslationStatus.Relevant)]
            },
            new ImageContent { Localizations = [Localization(2, TranslationStatus.Outdated), Localization(4)] }));
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var rows = await new WhoWeAreSectionsRepository(context).GetPreviewRowsAsync();

        var row = Assert.Single(rows);
        Assert.Equal(2, row.LanguageId);
        Assert.Equal(outdated ? TranslationStatus.Outdated : TranslationStatus.Relevant, row.TranslationStatus);
        Assert.Empty(context.ChangeTracker.Entries());
    }

    [Fact]
    public async Task GetPreviewRows_SeparatesSectionsAndLanguages()
    {
        await using var context = Context();
        context.Set<WhoWeAreSection>().AddRange(
            Section(1, new TitleContent { Localizations = [Localization(2)] }, new DescriptionContent { Localizations = [Localization(2), Localization(3)] }),
            Section(2, new TitleContent { Localizations = [Localization(2, TranslationStatus.Outdated), Localization(3)] }),
            Section(3, new TitleContent()));
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var rows = await new WhoWeAreSectionsRepository(context).GetPreviewRowsAsync();

        Assert.Equal(4, rows.Count);
        Assert.Collection(
            rows,
            row =>
            {
                Assert.Equal(1, row.Id);
                Assert.Equal(2, row.LanguageId);
                Assert.Equal(TranslationStatus.Relevant, row.TranslationStatus);
            },
            row =>
            {
                Assert.Equal(2, row.Id);
                Assert.Equal(2, row.LanguageId);
                Assert.Equal(TranslationStatus.Outdated, row.TranslationStatus);
            },
            row =>
            {
                Assert.Equal(2, row.Id);
                Assert.Equal(3, row.LanguageId);
                Assert.Equal(TranslationStatus.Relevant, row.TranslationStatus);
            },
            row =>
            {
                Assert.Equal(3, row.Id);
                Assert.Null(row.LanguageId);
            });
    }

    [Fact]
    public async Task GetPreviewRows_CardContentParticipatesInCompleteness()
    {
        await using var context = Context();
        context.Set<WhoWeAreSection>().Add(Section(
            1,
            new TitleContent { Localizations = [Localization(2)] },
            new CardContent { Description = "Untranslated card" }));
        await context.SaveChangesAsync();

        Assert.Null(Assert.Single(await new WhoWeAreSectionsRepository(context).GetPreviewRowsAsync()).LanguageId);
    }

    [Fact]
    public async Task GetPreviewRows_ObservesCancellation()
    {
        await using var context = Context();
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => new WhoWeAreSectionsRepository(context).GetPreviewRowsAsync(cancellation.Token));
    }

    [Fact]
    public async Task GetPreviewRows_EntireAggregationTranslatesBeforeOpeningSqlServerConnection()
    {
        var options = new DbContextOptionsBuilder<VictoryCenterDbContext>()
            .UseSqlServer("Server=unused;Database=QueryTranslationOnly;Integrated Security=True")
            .AddInterceptors(new BlockConnection()).Options;
        await using var context = new VictoryCenterDbContext(options);

        await Assert.ThrowsAsync<ConnectionBlockedException>(() => new WhoWeAreSectionsRepository(context).GetPreviewRowsAsync());
    }

    private static VictoryCenterDbContext Context() => new(new DbContextOptionsBuilder<VictoryCenterDbContext>()
        .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);

    private static WhoWeAreSection Section(long id, params WhoWeAreContent[] contents) => new()
    {
        Id = id, Title = "Section " + id, SectionType = SectionType.Main, CreatedAt = DateTime.UtcNow, Contents = contents.ToList()
    };

    private static WhoWeAreContentLocalization Localization(long languageId, TranslationStatus status = TranslationStatus.Relevant) => new()
    {
        LanguageId = languageId, TranslationStatus = status, CreatedAt = DateTimeOffset.UtcNow
    };

    private sealed class BlockConnection : DbConnectionInterceptor
    {
        public override ValueTask<InterceptionResult> ConnectionOpeningAsync(
            DbConnection connection, ConnectionEventData eventData, InterceptionResult result, CancellationToken cancellationToken = default)
        {
            throw new ConnectionBlockedException();
        }
    }

    private sealed class ConnectionBlockedException : Exception
    {
    }
}
