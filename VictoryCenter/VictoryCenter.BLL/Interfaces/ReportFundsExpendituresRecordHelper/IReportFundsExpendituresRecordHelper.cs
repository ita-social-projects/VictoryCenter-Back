using FluentResults;
using VictoryCenter.BLL.DTOs.Admin.ReportFundsExpendituresSettings;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.Interfaces.ReportFundsExpendituresRecordHelper;

public interface IReportFundsExpendituresRecordHelper
{
    Task<Result<ReportFundsExpendituresSettingsDto>> GetAndValidateSettingsAsync();
    Task<Result> ValidateCategoryChangeAsync(ReportFundsExpendituresRecord entityToUpdate, long newCategoryId, long currentRecordId);
    (decimal AmountUah, decimal AmountUsd) CalculateAmounts(decimal amount, ReportFundsExpendituresCurrency currency, decimal exchangeRate);
}
