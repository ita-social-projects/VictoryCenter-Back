using FluentResults;
using VictoryCenter.BLL.DTOs.Admin.ReportFundsExpendituresSettings;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Interfaces.UpdateReportFundsExpendituresRecordHelper;

public interface IUpdateReportFundsExpendituresRecordHelper
{
    Task<Result<ReportFundsExpendituresSettingsDto>> GetAndValidateSettingsAsync();
    Task<Result> ValidateCategoryChangeAsync(ReportFundsExpendituresRecord entityToUpdate, long newCategoryId, long currentRecordId);
}
