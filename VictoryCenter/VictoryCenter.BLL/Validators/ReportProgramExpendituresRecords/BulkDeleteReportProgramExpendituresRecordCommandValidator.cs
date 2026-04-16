using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.ReportProgramExpendituresRecords.BulkDelete;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Validators.ReportProgramExpendituresRecords;

public class BulkDeleteReportProgramExpendituresRecordCommandValidator
    : AbstractValidator<BulkDeleteReportProgramExpendituresRecordCommand>
{
    public BulkDeleteReportProgramExpendituresRecordCommandValidator()
    {
        RuleForEach(e => e.Ids)
            .MustBeValidId(nameof(ReportProgramExpendituresRecord.Id));

        RuleFor(e => e.Ids)
            .MustHaveUniqueIds(nameof(BulkDeleteReportProgramExpendituresRecordCommand.Ids));
    }
}
