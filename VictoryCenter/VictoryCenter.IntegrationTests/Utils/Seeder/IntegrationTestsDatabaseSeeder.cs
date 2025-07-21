using Microsoft.Extensions.Logging;
using VictoryCenter.DAL.Data;
using VictoryCenter.IntegrationTests.Utils.Seeder.Seeders;

namespace VictoryCenter.IntegrationTests.Utils.Seeder;

public class SeederManager
{
    private readonly VictoryCenterDbContext _dbContext;
    private readonly ILoggerFactory _loggerFactory;
    private readonly List<ISeeder> _seeders;

    public SeederManager(VictoryCenterDbContext dbContext, ILoggerFactory loggerFactory)
    {
        _dbContext = dbContext;
        _loggerFactory = loggerFactory;

        _seeders = new List<ISeeder>
        {
            new CategoriesSeeder(_dbContext, _loggerFactory.CreateLogger<CategoriesSeeder>()),
            new TeamMembersSeeder(_dbContext, _loggerFactory.CreateLogger<TeamMembersSeeder>())
        }
        .OrderBy(s => s.Order)
        .ToList();
    }

    public async Task<bool> SeedAllAsync()
    {
        foreach (var seeder in _seeders)
        {
            var result = await seeder.SeedAsync();
            if (!result.Success)
            {
                await DisposeAllAsync();
                return false;
            }
        }

        return true;
    }

    public async Task DisposeAllAsync()
    {
        foreach (var seeder in _seeders.OrderByDescending(s => s.Order))
        {
            await seeder.DisposeAsync();
        }
    }
}
