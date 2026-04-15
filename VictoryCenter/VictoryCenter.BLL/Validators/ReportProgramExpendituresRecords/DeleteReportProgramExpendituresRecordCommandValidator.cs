using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.ReportProgramExpendituresRecords.Delete;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Validators.ReportProgramExpendituresRecords;

public class DeleteReportProgramExpendituresRecordCommandValidator
    : AbstractValidator<DeleteReportProgramExpendituresRecordCommand>
{
    public DeleteReportProgramExpendituresRecordCommandValidator()
    {
        RuleFor(x => x.ReportProgramExpendituresRecordId)
            .NotNull()
            .MustBeValidId(nameof(ReportProgramExpendituresRecord.Id));
    }
}
