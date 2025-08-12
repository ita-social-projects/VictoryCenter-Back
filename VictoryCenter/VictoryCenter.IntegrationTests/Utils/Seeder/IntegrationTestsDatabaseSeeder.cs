using Microsoft.Extensions.Logging;
using VictoryCenter.BLL.Interfaces.BlobStorage;
using VictoryCenter.DAL.Data;

namespace VictoryCenter.IntegrationTests.Utils.Seeder;

public class SeederManager
{
    private readonly VictoryCenterDbContext _dbContext;
    private readonly ILoggerFactory _loggerFactory;
    private readonly ILogger<SeederManager> _logger;
    private readonly IBlobService _blobService;
    private List<ISeeder> _seeders;

    public SeederManager(VictoryCenterDbContext dbContext, ILoggerFactory loggerFactory, IBlobService blobService,  IEnumerable<ISeeder>? seeders = null)
    {
        _dbContext = dbContext;
        _loggerFactory = loggerFactory;
        _logger = loggerFactory.CreateLogger<SeederManager>();
        _blobService = blobService;

        _seeders = (seeders ?? CreateDefaultSeeders())
            .OrderBy(s => s.Order)
            .ToList();
    }

    public void ClearSeeders()
        => _seeders.Clear();

    public void ConfigureSeeders(params ISeeder[] seeders)
        => _seeders = seeders.OrderBy(s => s.Order).ToList();

    public void AddSeeder(ISeeder seeder)
    {
        _seeders.Add(seeder);
        _seeders = _seeders.OrderBy(s => s.Order).ToList();
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

    public IEnumerable<ISeeder> CreateDefaultSeeders()
    {
        yield return new CategoriesSeeder.CategoriesSeeder(_dbContext, _loggerFactory.CreateLogger<CategoriesSeeder.CategoriesSeeder>(), _blobService);
        yield return new TeamMembersSeeder.TeamMembersSeeder(_dbContext, _loggerFactory.CreateLogger<TeamMembersSeeder.TeamMembersSeeder>(), _blobService);
        yield return new ImageSeeder.ImagesDataSeeder(_dbContext, _loggerFactory.CreateLogger<ImageSeeder.ImagesDataSeeder>(), _blobService);
    }
}
