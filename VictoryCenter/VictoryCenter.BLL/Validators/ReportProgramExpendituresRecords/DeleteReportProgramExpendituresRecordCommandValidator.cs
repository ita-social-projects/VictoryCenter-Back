using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.ReportProgramExpendituresRecords.Delete;
using VictoryCenter.DAL.Entities;
using VictoryCenter.BLL.Helpers;

namespace VictoryCenter.BLL.Validators.ReportProgramExpendituresRecords;

public class DeleteReportProgramExpendituresRecordCommandValidator
    : AbstractValidator<DeleteReportProgramExpendituresRecordCommand>
{
    public DeleteReportProgramExpendituresRecordCommandValidator()
    {
        RuleFor(x => x.ReportProgramExpendituresRecordId)
            .MustBeValidId(nameof(ReportProgramExpendituresRecord.Id));
    }
}
