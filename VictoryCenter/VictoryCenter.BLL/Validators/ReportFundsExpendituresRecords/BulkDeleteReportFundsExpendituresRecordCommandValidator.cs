using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.ReportFundsExpendituresRecords.BulkDelete;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Helpers;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Validators.ReportFundsExpendituresRecords;

public class BulkDeleteReportFundsExpendituresRecordCommandValidator
    : AbstractValidator<BulkDeleteReportFundsExpendituresRecordCommand>
{
    public BulkDeleteReportFundsExpendituresRecordCommandValidator()
    {
        RuleFor(e => e.Ids)
            .MustBeValidBulkDeleteIds(
                nameof(BulkDeleteReportFundsExpendituresRecordCommand.Ids),
                nameof(ReportFundsExpendituresRecord.Id),
                ReportFundsExpendituresRecordConstants.MaxNumberOfRecordsPerBulkDelete);
    }
}
