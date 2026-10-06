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
                    BatchUpdateReportProgramExpendituresRecordDto>(new()
                    {
                        CreateDtoValidator = createDtoValidator,
                        UpdateDtoValidator = updateDtoValidator,
                        MaxNumberOfRecordsPerBatchOperation = ReportProgramExpendituresRecordConstants.MaxNumberOfRecordsPerBatchOperation,
                        UpdateIdSelector = u => u.Id,
                        CreateCategoryIdSelector = c => c.HippotherapyProgramCategoryId,
                        UpdateCategoryIdSelector = u => u.HippotherapyProgramCategoryId,
                        CategoryIdPropertyName = nameof(ReportProgramExpendituresRecord.HippotherapyProgramCategoryId),
                        EntityType = typeof(ReportProgramExpendituresRecord)
                    }));
        });
    }
}
