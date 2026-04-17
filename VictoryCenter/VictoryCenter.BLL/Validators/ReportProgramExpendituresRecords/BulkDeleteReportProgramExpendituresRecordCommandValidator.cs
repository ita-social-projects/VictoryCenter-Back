using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.ReportProgramExpendituresRecords.BulkDelete;
using VictoryCenter.BLL.Constants;
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
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.CollectionCannotBeEmpty(
                nameof(BulkDeleteReportProgramExpendituresRecordCommand.Ids)))
            .WithMessage(
                ErrorMessagesConstants.CollectionCannotBeEmpty(
                    nameof(BulkDeleteReportProgramExpendituresRecordCommand.Ids)))
            .Must(e =>
                e.Count() <= ReportProgramExpendituresRecordConstants.MaxNumberOfRecordsPerBulkDelete)
            .WithMessage(ErrorMessagesConstants.CollectionCannotContainMoreThan(
                nameof(BulkDeleteReportProgramExpendituresRecordCommand.Ids),
                ReportProgramExpendituresRecordConstants.MaxNumberOfRecordsPerBulkDelete))
            .MustHaveUniqueIds(nameof(BulkDeleteReportProgramExpendituresRecordCommand.Ids));
    }
}
