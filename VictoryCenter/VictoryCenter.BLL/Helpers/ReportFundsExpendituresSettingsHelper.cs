using FluentResults;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using ReportFundsExpendituresSettingsEntity = VictoryCenter.DAL.Entities.ReportFundsExpendituresSettings;

namespace VictoryCenter.BLL.Helpers;

public static class ReportFundsExpendituresSettingsHelper
{
    public static async Task<Result<ReportFundsExpendituresSettingsEntity>> GetOrCreateSettingsAsync(
        IRepositoryWrapper repositoryWrapper,
        TimeProvider timeProvider)
    {
        var settings = await repositoryWrapper.ReportFundsExpendituresSettingsRepository
            .GetFirstOrDefaultAsync(new QueryOptions<ReportFundsExpendituresSettingsEntity>
            {
                Filter = entity => entity.Id == ReportFundsExpendituresSettingsConstants.SingletonSettingsId
            });

        if (settings is not null)
        {
            return Result.Ok(settings);
        }

        settings = CreateDefaultSettings(timeProvider);
        await repositoryWrapper.ReportFundsExpendituresSettingsRepository.CreateAsync(settings);

        try
        {
            if (await repositoryWrapper.SaveChangesAsync() <= 0)
            {
                return Result.Fail<ReportFundsExpendituresSettingsEntity>(
                    ErrorMessagesConstants.FailedToCreateEntity(typeof(ReportFundsExpendituresSettingsEntity)));
            }
        }
        catch (DbUpdateException)
        {
            return Result.Fail<ReportFundsExpendituresSettingsEntity>(
                ErrorMessagesConstants.FailedToCreateEntityInDatabase(typeof(ReportFundsExpendituresSettingsEntity)));
        }

        return Result.Ok(settings);
    }

    private static ReportFundsExpendituresSettingsEntity CreateDefaultSettings(TimeProvider timeProvider)
    {
        var now = timeProvider.GetUtcNow();

        return new ReportFundsExpendituresSettingsEntity
        {
            Id = ReportFundsExpendituresSettingsConstants.SingletonSettingsId,
            DisclaimerTitle = string.Empty,
            ExchangeRate = 1m,
            ProgramExpendituresReportingYear = Math.Clamp(now.Year, 2010, 2050),
            CreatedAt = now
        };
    }
}
