using Microsoft.EntityFrameworkCore;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.Utils;

public class MainPageDatabaseCleaner : IDatabaseCleaner
{
    private readonly IntegrationTestDbFixture _fixture;

    public MainPageDatabaseCleaner(IntegrationTestDbFixture fixture)
    {
        _fixture = fixture;
    }

    public async Task CleanupAsync()
    {
        var existing = await _fixture.DbContext.MainPages
            .Include(m => m.MainAboutUs)
            .Include(m => m.MainPartners)
            .Include(m => m.MainDonations)
            .Include(m => m.ImpactStatistics)
                .ThenInclude(s => s!.Metrics)
            .ToListAsync();

        if (existing.Count > 0)
        {
            _fixture.DbContext.MainPages.RemoveRange(existing);
            await _fixture.DbContext.SaveChangesAsync();
        }

        _fixture.DbContext.ChangeTracker.Clear();
    }
}
