using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using VictoryCenter.BLL.Interfaces.BlobStorage;
using VictoryCenter.DAL.Data;

namespace VictoryCenter.IntegrationTests.Utils.Seeders;

public class SeederManager
{
    private readonly VictoryCenterDbContext _dbContext;
    private readonly ILoggerFactory _loggerFactory;
    private readonly ILogger<SeederManager> _logger;
    private readonly IBlobService _blobService;
    private readonly IServiceProvider _serviceProvider;
    private List<ISeeder> _seeders;

    public SeederManager(VictoryCenterDbContext dbContext, ILoggerFactory loggerFactory, IBlobService blobService, IServiceProvider serviceProvider, IEnumerable<ISeeder>? seeders = null)
    {
        _dbContext = dbContext;
        _loggerFactory = loggerFactory;
        _logger = loggerFactory.CreateLogger<SeederManager>();
        _blobService = blobService;
        _serviceProvider = serviceProvider;

        _seeders = [.. (seeders ?? CreateDefaultSeeders()).OrderBy(s => s.Order)];
    }

    public void ClearSeeders()
        => _seeders.Clear();

    public void ConfigureSeeders(params ISeeder[] seeders)
        => _seeders = [.. seeders.OrderBy(s => s.Order)];

    public void AddSeeder(ISeeder seeder)
    {
        _seeders.Add(seeder);
        _seeders = [.. _seeders.OrderBy(s => s.Order)];
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

    private IEnumerable<ISeeder> CreateDefaultSeeders()
    {
        var seederType = typeof(ISeeder);

        var seederTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => seederType.IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

        foreach (var type in seederTypes)
        {
            yield return (ISeeder)ActivatorUtilities.CreateInstance(_serviceProvider, type);
        }
    }
}
