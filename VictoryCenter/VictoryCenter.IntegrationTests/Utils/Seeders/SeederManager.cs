using Microsoft.Extensions.DependencyInjection;

namespace VictoryCenter.IntegrationTests.Utils.Seeders;

public sealed class SeederManager
{
    private readonly IServiceProvider _serviceProvider;
    private List<ISeeder> _seeders;

    public SeederManager(IServiceProvider serviceProvider, IEnumerable<ISeeder>? seeders = null)
    {
        _serviceProvider = serviceProvider;

        _seeders = (seeders ?? CreateDefaultSeeders())
            .OrderBy(s => s.Order)
            .ToList();
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
        var seederTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => typeof(ISeeder).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

        foreach (var type in seederTypes)
        {
            yield return (ISeeder)ActivatorUtilities.CreateInstance(_serviceProvider, type);
        }
    }
}
