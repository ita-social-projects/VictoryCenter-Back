using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.IntegrationTests.Utils.Seeders.Localizations.ReportFundsExpendituresCategoryLocalizations;

public class ReportFundsExpendituresCategoryLocalizationsSeeder : ISeeder
{
    private readonly VictoryCenterDbContext _dbContext;
    private readonly ILogger<ReportFundsExpendituresCategoryLocalizationsSeeder> _logger;
    private readonly List<ReportFundsExpendituresCategory> _categories = [];
    private readonly List<ReportFundsExpendituresCategoryLocalization> _localizations = [];

    public ReportFundsExpendituresCategoryLocalizationsSeeder(
        VictoryCenterDbContext dbContext,
        ILogger<ReportFundsExpendituresCategoryLocalizationsSeeder> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public int Order => (int)SeederExecutionOrder.ReportFundsExpendituresCategoryLocalizations;
    public string Name => nameof(ReportFundsExpendituresCategoryLocalizationsSeeder);

    public async Task<SeederResult> SeedAsync()
    {
        try
        {
            _categories.AddRange(new List<ReportFundsExpendituresCategory>
            {
                new() { Name = "Localization Test Category 1", Type = ReportFundsExpendituresType.Income, CreatedAt = DateTimeOffset.UtcNow },
                new() { Name = "Localization Test Category 2", Type = ReportFundsExpendituresType.Expense, CreatedAt = DateTimeOffset.UtcNow },
            });

            await _dbContext.ReportFundsExpendituresCategories.AddRangeAsync(_categories);
            await _dbContext.SaveChangesAsync();

            _localizations.AddRange(new List<ReportFundsExpendituresCategoryLocalization>
            {
                new() { EntityId = _categories[0].Id, LanguageId = 2, Name = "English Name 1", CreatedAt = DateTimeOffset.UtcNow },
                new() { EntityId = _categories[1].Id, LanguageId = 2, Name = "English Name 2", CreatedAt = DateTimeOffset.UtcNow },
            });

            await _dbContext.ReportFundsExpendituresCategoryLocalizations.AddRangeAsync(_localizations);
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
                _dbContext.ReportFundsExpendituresCategoryLocalizations.RemoveRange(_localizations);
                await _dbContext.SaveChangesAsync();
                _localizations.Clear();
            }

            if (_categories.Count != 0)
            {
                _dbContext.ReportFundsExpendituresCategories.RemoveRange(_categories);
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
        var count = await _dbContext.ReportFundsExpendituresCategoryLocalizations.CountAsync();
        return count >= _localizations.Count;
    }
}
