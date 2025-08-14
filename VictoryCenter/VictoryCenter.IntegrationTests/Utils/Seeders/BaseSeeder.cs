using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VictoryCenter.DAL.Data;

namespace VictoryCenter.IntegrationTests.Utils.Seeders;

public abstract class BaseSeeder<TEntity> : ISeeder
where TEntity : class
{
    private readonly ILogger _logger;
    private readonly List<TEntity> _createdEntities = [];
    protected BaseSeeder(VictoryCenterDbContext dbContext, ILogger logger)
    {
        DbContext = dbContext;
        _logger = logger;
    }

    public abstract int Order { get; }
    public abstract string Name { get; }
    protected VictoryCenterDbContext DbContext { get; init; }

    public async Task<SeederResult> SeedAsync()
    {
        try
        {
            var entities = await GenerateEntitiesAsync();

            await DbContext.Set<TEntity>().AddRangeAsync(entities);
            await DbContext.SaveChangesAsync();

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
            if (_createdEntities.Count != 0)
            {
                DbContext.Set<TEntity>().RemoveRange(_createdEntities);
                await DbContext.SaveChangesAsync();
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
        var actual = await DbContext.Set<TEntity>().CountAsync();
        return actual >= expected;
    }

    protected abstract Task<List<TEntity>> GenerateEntitiesAsync();
}
