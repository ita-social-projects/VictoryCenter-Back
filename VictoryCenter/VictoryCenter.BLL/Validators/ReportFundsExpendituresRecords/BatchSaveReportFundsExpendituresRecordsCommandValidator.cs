using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.ReportFundsExpendituresRecords.BatchSave;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ReportFundsExpendituresRecords;
using VictoryCenter.BLL.Validators.ReportExpendituresRecords;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Validators.ReportFundsExpendituresRecords;

public class BatchSaveReportFundsExpendituresRecordsCommandValidator
    : AbstractValidator<BatchSaveReportFundsExpendituresRecordCommand>
{
    public BatchSaveReportFundsExpendituresRecordsCommandValidator(
        IValidator<CreateReportFundsExpendituresRecordDto> createDtoValidator,
        IValidator<BatchUpdateReportFundsExpendituresRecordDto> updateDtoValidator)
    {
        RuleFor(command => command.BatchSaveReportFundsExpendituresRecordsDto)
            .NotNull()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(
                nameof(BatchSaveReportFundsExpendituresRecordCommand.BatchSaveReportFundsExpendituresRecordsDto)));

        When(command => command.BatchSaveReportFundsExpendituresRecordsDto != null, () =>
        {
            RuleFor(command => command.BatchSaveReportFundsExpendituresRecordsDto)
                .SetValidator(new BatchSaveReportExpendituresRecordsDtoValidator<
                    CreateReportFundsExpendituresRecordDto,
                    BatchUpdateReportFundsExpendituresRecordDto>(
                        createDtoValidator,
                        updateDtoValidator,
                        ReportFundsExpendituresRecordConstants.MaxNumberOfRecordsPerBatchOperation,
                        u => u.Id,
                        c => c.CategoryId,
                        u => u.CategoryId,
                        nameof(ReportFundsExpendituresRecord.CategoryId)));
        });
    }
}
