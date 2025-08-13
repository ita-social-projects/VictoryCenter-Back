using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VictoryCenter.DAL.Data;

namespace VictoryCenter.IntegrationTests.Utils.Seeders;

public abstract class BaseSeeder<TEntity> : ISeeder
where TEntity : class
{
    protected readonly VictoryCenterDbContext _dbContext;
    private readonly ILogger _logger;
    private readonly List<TEntity> _createdEntities = new();

    protected BaseSeeder(VictoryCenterDbContext dbContext, ILogger logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public abstract int Order { get; }
    public abstract string Name { get; }

    public async Task<SeederResult> SeedAsync()
    {
        try
        {
            if (await ShouldSkipAsync())
            {
                return new SeederResult { Success = true, CreatedCount = 0 };
            }

            var entities = await GenerateEntitiesAsync();

            await _dbContext.Set<TEntity>().AddRangeAsync(entities);
            await _dbContext.SaveChangesAsync();

            _createdEntities.AddRange(entities);

            if (!await VerifyAsync())
            {
                throw new InvalidOperationException($"Verification failed for seeder {Name}");
            }

            return new SeederResult { Success = true, CreatedCount = entities.Count };
        }
        catch (Exception ex)
        {
            await DisposeAsync();
            _logger.LogError(ex, $"Error in seeder {Name}");
            return new SeederResult { Success = false, ErrorMessage = ex.Message };
        }
    }

    public async Task<bool> DisposeAsync()
    {
        try
        {
            if (_createdEntities.Any())
            {
                _dbContext.Set<TEntity>().RemoveRange(_createdEntities);
                await _dbContext.SaveChangesAsync();
                _createdEntities.Clear();
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error disposing seeder {Name}");
            return false;
        }
    }

    public virtual async Task<bool> VerifyAsync()
    {
        var expected = _createdEntities.Count;
        var actual = await _dbContext.Set<TEntity>().CountAsync();
        return actual >= expected;
    }

    protected abstract Task<List<TEntity>> GenerateEntitiesAsync();
    protected abstract Task<bool> ShouldSkipAsync();
}
