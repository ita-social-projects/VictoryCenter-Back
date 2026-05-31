using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.ReportFundsExpendituresRecords.BulkDelete;
using VictoryCenter.BLL.Constants;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Validators.ReportFundsExpendituresRecords;

public class BulkDeleteReportFundsExpendituresRecordCommandValidator
    : AbstractValidator<BulkDeleteReportFundsExpendituresRecordCommand>
{
    public BulkDeleteReportFundsExpendituresRecordCommandValidator()
    {
        RuleForEach(e => e.Ids)
            .GreaterThan(0)
            .WithMessage(ErrorMessagesConstants.PropertyMustBePositive(nameof(ReportFundsExpendituresRecord.Id)));

        RuleFor(e => e.Ids)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.CollectionCannotBeEmpty(
                nameof(BulkDeleteReportFundsExpendituresRecordCommand.Ids)))
            .Must(e => e.Count() <= ReportFundsExpendituresRecordConstants.MaxNumberOfRecordsPerBulkDelete)
            .WithMessage(ErrorMessagesConstants.CollectionCannotContainMoreThan(
                nameof(BulkDeleteReportFundsExpendituresRecordCommand.Ids),
                ReportFundsExpendituresRecordConstants.MaxNumberOfRecordsPerBulkDelete))
            .Must(e => e.Distinct().Count() == e.Count())
            .WithMessage(ErrorMessagesConstants.CollectionMustContainUniqueValues(
                nameof(BulkDeleteReportFundsExpendituresRecordCommand.Ids)));
    }
}
