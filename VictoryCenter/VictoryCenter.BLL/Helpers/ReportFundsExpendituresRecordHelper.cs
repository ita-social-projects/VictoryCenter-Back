using AutoMapper;
using FluentResults;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ReportFundsExpendituresSettings;
using VictoryCenter.BLL.Interfaces.ReportFundsExpendituresRecordHelper;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Helpers;

public class ReportFundsExpendituresRecordHelper : IReportFundsExpendituresRecordHelper
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IMapper _mapper;

    public ReportFundsExpendituresRecordHelper(IRepositoryWrapper repositoryWrapper, IMapper mapper)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
    }

    public async Task<Result<ReportFundsExpendituresSettingsDto>> GetAndValidateSettingsAsync()
    {
        var settingsEntity = await _repositoryWrapper.ReportFundsExpendituresSettingsRepository
            .GetFirstOrDefaultAsync(new QueryOptions<ReportFundsExpendituresSettings>());

        if (settingsEntity is null)
        {
            return Result.Fail<ReportFundsExpendituresSettingsDto>(ReportFundsExpendituresSettingsConstants.CouldNotFindSettingsErrorMessage);
        }

        var settingsDto = _mapper.Map<ReportFundsExpendituresSettingsDto>(settingsEntity);

        if (settingsDto.ExchangeRate <= ReportFundsExpendituresSettingsConstants.ExchangeRateMinValue)
        {
            return Result.Fail<ReportFundsExpendituresSettingsDto>(ReportFundsExpendituresSettingsConstants.InvalidExchangeRateErrorMessage);
        }

        return Result.Ok(settingsDto);
    }

    public async Task<Result> ValidateCategoryChangeAsync(ReportFundsExpendituresRecord entityToUpdate, long newCategoryId, long currentRecordId)
    {
        if (entityToUpdate.CategoryId == newCategoryId)
        {
            return Result.Ok();
        }

        var category = await _repositoryWrapper.ReportFundsExpendituresCategoriesRepository
            .GetFirstOrDefaultAsync(new QueryOptions<ReportFundsExpendituresCategory>
            {
                Filter = entity => entity.Id == newCategoryId
            });

        if (category is null)
        {
            return Result.Fail(ErrorMessagesConstants.NotFound(newCategoryId, typeof(ReportFundsExpendituresCategory)));
        }

        if (category.Type != entityToUpdate.Type)
        {
            return Result.Fail(ReportFundsExpendituresRecordConstants.CategoryTypeMustMatchRecordType);
        }

        var duplicateRecordInCategory = await _repositoryWrapper.ReportFundsExpendituresRecordsRepository
            .GetFirstOrDefaultAsync(new QueryOptions<ReportFundsExpendituresRecord>
            {
                Filter = entity => entity.CategoryId == newCategoryId && entity.Id != currentRecordId
            });

        if (duplicateRecordInCategory is not null)
        {
            return Result.Fail(ReportFundsExpendituresRecordConstants.CategoryAlreadyHasRecord);
        }

        return Result.Ok();
    }

    public (decimal AmountUah, decimal AmountUsd) CalculateAmounts(decimal amount, ReportFundsExpendituresCurrency currency, decimal exchangeRate)
    {
        return currency == ReportFundsExpendituresCurrency.Usd
            ? (AmountUah: amount * exchangeRate, AmountUsd: amount)
            : (AmountUah: amount, AmountUsd: amount / exchangeRate);
    }
}
