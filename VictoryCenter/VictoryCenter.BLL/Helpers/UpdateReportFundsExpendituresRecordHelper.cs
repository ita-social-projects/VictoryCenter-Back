using AutoMapper;
using FluentResults;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ReportFundsExpendituresSettings;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using VictoryCenter.BLL.Interfaces.UpdateReportFundsExpendituresRecordHelper;

namespace VictoryCenter.BLL.Helpers;

public class UpdateReportFundsExpendituresRecordHelper : IUpdateReportFundsExpendituresRecordHelper
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IMapper _mapper;

    public UpdateReportFundsExpendituresRecordHelper(IRepositoryWrapper repositoryWrapper, IMapper mapper)
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
}
