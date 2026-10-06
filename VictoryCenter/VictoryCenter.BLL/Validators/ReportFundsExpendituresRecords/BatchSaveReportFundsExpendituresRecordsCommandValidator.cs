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
                    BatchUpdateReportFundsExpendituresRecordDto>(new()
                    {
                        CreateDtoValidator = createDtoValidator,
                        UpdateDtoValidator = updateDtoValidator,
                        MaxNumberOfRecordsPerBatchOperation = ReportFundsExpendituresRecordConstants.MaxNumberOfRecordsPerBatchOperation,
                        UpdateIdSelector = u => u.Id,
                        CreateCategoryIdSelector = c => c.CategoryId,
                        UpdateCategoryIdSelector = u => u.CategoryId,
                        CategoryIdPropertyName = nameof(ReportFundsExpendituresRecord.CategoryId),
                        EntityType = typeof(ReportFundsExpendituresRecord)
                    }));
        });
    }
}
