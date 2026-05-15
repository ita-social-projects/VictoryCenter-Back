using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VictoryCenter.BLL.Constants;
using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;

namespace VictoryCenter.IntegrationTests.Utils.Seeders.Localizations.ReportFundsExpendituresSettingsLocalizations;

public class ReportFundsExpendituresSettingsLocalizationsSeeder : ISeeder
{
    private readonly VictoryCenterDbContext _dbContext;
    private readonly ILogger<ReportFundsExpendituresSettingsLocalizationsSeeder> _logger;
    private readonly List<ReportFundsExpendituresSettingsLocalization> _localizations = [];

    public ReportFundsExpendituresSettingsLocalizationsSeeder(
        VictoryCenterDbContext dbContext,
        ILogger<ReportFundsExpendituresSettingsLocalizationsSeeder> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public int Order => (int)SeederExecutionOrder.ReportFundsExpendituresSettingsLocalizations;
    public string Name => nameof(ReportFundsExpendituresSettingsLocalizationsSeeder);

    public async Task<SeederResult> SeedAsync()
    {
        try
        {
            await EnsureSettingsExistsAsync();

            _localizations.Add(new ReportFundsExpendituresSettingsLocalization
            {
                EntityId = ReportFundsExpendituresSettingsConstants.SingletonSettingsId,
                LanguageId = 2,
                DisclaimerTitle = "English disclaimer for testing",
                CreatedAt = DateTimeOffset.UtcNow
            });

            await _dbContext.ReportFundsExpendituresSettingsLocalizations.AddRangeAsync(_localizations);
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
                _dbContext.ReportFundsExpendituresSettingsLocalizations.RemoveRange(_localizations);
                await _dbContext.SaveChangesAsync();
                _localizations.Clear();
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
        var count = await _dbContext.ReportFundsExpendituresSettingsLocalizations.CountAsync();
        return count >= _localizations.Count;
    }

    private async Task EnsureSettingsExistsAsync()
    {
        var existing = await _dbContext.ReportFundsExpendituresSettings
            .FirstOrDefaultAsync(e => e.Id == ReportFundsExpendituresSettingsConstants.SingletonSettingsId);

        if (existing is not null)
        {
            return;
        }

        await _dbContext.ReportFundsExpendituresSettings.AddAsync(new ReportFundsExpendituresSettings
        {
            Id = ReportFundsExpendituresSettingsConstants.SingletonSettingsId,
            DisclaimerTitle = "Initial disclaimer",
            ExchangeRate = 40m,
            CreatedAt = DateTimeOffset.UtcNow
        });
        await _dbContext.SaveChangesAsync();
    }
}
