using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.ReportProgramExpendituresRecords.BatchSave;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ReportProgramExpendituresRecords;
using VictoryCenter.BLL.Validators.ReportExpendituresRecords;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Validators.ReportProgramExpendituresRecords;

public class BatchSaveReportProgramExpendituresRecordsCommandValidator
    : AbstractValidator<BatchSaveReportProgramExpendituresRecordCommand>
{
    public BatchSaveReportProgramExpendituresRecordsCommandValidator(
        IValidator<CreateReportProgramExpendituresRecordDto> createDtoValidator,
        IValidator<BatchUpdateReportProgramExpendituresRecordDto> updateDtoValidator)
    {
        RuleFor(command => command.BatchSaveReportProgramExpendituresRecordsDto)
             .NotNull()
             .WithMessage(ErrorMessagesConstants.PropertyIsRequired(
                 nameof(BatchSaveReportProgramExpendituresRecordCommand.BatchSaveReportProgramExpendituresRecordsDto)));

        When(command => command.BatchSaveReportProgramExpendituresRecordsDto != null, () =>
        {
            RuleFor(command => command.BatchSaveReportProgramExpendituresRecordsDto)
                .SetValidator(new BatchSaveReportExpendituresRecordsDtoValidator<
                    CreateReportProgramExpendituresRecordDto,
                    BatchUpdateReportProgramExpendituresRecordDto>(
                        createDtoValidator,
                        updateDtoValidator,
                        ReportProgramExpendituresRecordConstants.MaxNumberOfRecordsPerBatchOperation,
                        u => u.Id,
                        c => c.HippotherapyProgramCategoryId,
                        u => u.HippotherapyProgramCategoryId,
                        nameof(ReportProgramExpendituresRecord.HippotherapyProgramCategoryId)));
        });
    }
}
