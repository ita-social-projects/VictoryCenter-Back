using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Realizations.EventNews;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;
using EventNewsEntity = VictoryCenter.DAL.Entities.EventNews;

namespace VictoryCenter.IntegrationTests.RepositoryTests.EventNews;

public class EventNewsRepositoryTests : BaseTestClass
{
    public EventNewsRepositoryTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task GetSlugsStartingWithAsync_ShouldExcludeSpecifiedEntity()
    {
        var slugPrefix = $"excluded-id-{Guid.NewGuid():N}";
        var excludedEventNews = CreateEventNews(slugPrefix);
        var matchingEventNews = CreateEventNews($"{slugPrefix}-1");
        var nonMatchingEventNews = CreateEventNews($"another-{Guid.NewGuid():N}");
        Fixture.DbContext.EventNews.AddRange(
            excludedEventNews,
            matchingEventNews,
            nonMatchingEventNews);
        await Fixture.DbContext.SaveChangesAsync();
        Fixture.DbContext.ChangeTracker.Clear();
        var repository = new EventNewsRepository(Fixture.DbContext);

        var result = await repository.GetSlugsStartingWithAsync(
            excludedEventNews.Id,
            slugPrefix,
            CancellationToken.None);

        Assert.DoesNotContain(excludedEventNews.Slug!, result);
        Assert.Contains(matchingEventNews.Slug!, result);
        Assert.DoesNotContain(nonMatchingEventNews.Slug!, result);
    }

    [Fact]
    public async Task GetSlugsStartingWithAsync_CancelledToken_ShouldCancelQuery()
    {
        var repository = new EventNewsRepository(Fixture.DbContext);
        using var cancellationTokenSource = new CancellationTokenSource();
        await cancellationTokenSource.CancelAsync();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            repository.GetSlugsStartingWithAsync(
                0,
                "event-news",
                cancellationTokenSource.Token));
    }

    private static EventNewsEntity CreateEventNews(string slug)
    {
        return new EventNewsEntity
        {
            Slug = slug,
            Status = Status.Draft,
            CreatedAt = DateTimeOffset.UtcNow
        };
    }
}
