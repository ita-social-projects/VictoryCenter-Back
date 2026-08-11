using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;

namespace VictoryCenter.IntegrationTests.Utils.Seeders.Localizations.HippotherapyProgramCategoryLocalizations;

public class HippotherapyProgramCategoryLocalizationsSeeder : ISeeder
{
    private readonly VictoryCenterDbContext _dbContext;
    private readonly ILogger<HippotherapyProgramCategoryLocalizationsSeeder> _logger;
    private readonly List<HippotherapyProgramCategory> _categories = [];
    private readonly List<HippotherapyProgramCategoryLocalization> _localizations = [];

    public HippotherapyProgramCategoryLocalizationsSeeder(
        VictoryCenterDbContext dbContext,
        ILogger<HippotherapyProgramCategoryLocalizationsSeeder> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public int Order => (int)SeederExecutionOrder.HippotherapyProgramCategoryLocalizations;
    public string Name => nameof(HippotherapyProgramCategoryLocalizationsSeeder);

    public async Task<SeederResult> SeedAsync()
    {
        try
        {
            _categories.AddRange(new List<HippotherapyProgramCategory>
            {
                new() { Name = "Localization Test Category 1", CreatedAt = DateTimeOffset.UtcNow },
                new() { Name = "Localization Test Category 2", CreatedAt = DateTimeOffset.UtcNow },
            });

            await _dbContext.HippotherapyProgramCategories.AddRangeAsync(_categories);
            await _dbContext.SaveChangesAsync();

            _localizations.AddRange(new List<HippotherapyProgramCategoryLocalization>
            {
                new() { EntityId = _categories[0].Id, LanguageId = 2, Name = "English Name 1", CreatedAt = DateTimeOffset.UtcNow },
                new() { EntityId = _categories[1].Id, LanguageId = 2, Name = "English Name 2", CreatedAt = DateTimeOffset.UtcNow },
            });

            await _dbContext.HippotherapyProgramCategoryLocalizations.AddRangeAsync(_localizations);
            await _dbContext.SaveChangesAsync();

            if (!await VerifyAsync())
            {
                throw new InvalidOperationException($"Verification failed for seeder {Name}");
            }

            return new SeederResult { Success = true, CreatedCount = _localizations.Count };
        }
        catch (Exception ex)
        {
            await DisposeAsync();
            _logger.LogError(ex, "Error in seeder {Name}", Name);
            return new SeederResult { Success = false, ErrorMessage = ex.Message };
        }
    }

    public async Task<bool> DisposeAsync()
    {
        try
        {
            if (_localizations.Count != 0)
            {
                _dbContext.HippotherapyProgramCategoryLocalizations.RemoveRange(_localizations);
                await _dbContext.SaveChangesAsync();
                _localizations.Clear();
            }

            if (_categories.Count != 0)
            {
                _dbContext.HippotherapyProgramCategories.RemoveRange(_categories);
                await _dbContext.SaveChangesAsync();
                _categories.Clear();
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error disposing seeder {Name}", Name);
            return false;
        }
    }

    public async Task<bool> VerifyAsync()
    {
        var count = await _dbContext.HippotherapyProgramCategoryLocalizations.CountAsync();
        return count >= _localizations.Count;
    }
}
