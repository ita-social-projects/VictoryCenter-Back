using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.ReportFundsExpendituresRecords.BatchSave;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ReportFundsExpendituresRecords;
using VictoryCenter.BLL.Helpers;
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
            RuleFor(command => command.BatchSaveReportFundsExpendituresRecordsDto.RecordsToCreate)
                .NotNull()
                .WithMessage(ErrorMessagesConstants.PropertyIsRequired(
                    nameof(BatchSaveReportFundsExpendituresRecordsDto.RecordsToCreate)));

            RuleFor(command => command.BatchSaveReportFundsExpendituresRecordsDto.RecordsToUpdate)
                .NotNull()
                .WithMessage(ErrorMessagesConstants.PropertyIsRequired(
                    nameof(BatchSaveReportFundsExpendituresRecordsDto.RecordsToUpdate)));

            RuleFor(command => command.BatchSaveReportFundsExpendituresRecordsDto.RecordIdsToDelete)
                .NotNull()
                .WithMessage(ErrorMessagesConstants.PropertyIsRequired(
                    nameof(BatchSaveReportFundsExpendituresRecordsDto.RecordIdsToDelete)));

            When(
                command => command.BatchSaveReportFundsExpendituresRecordsDto.RecordsToCreate != null &&
                            command.BatchSaveReportFundsExpendituresRecordsDto.RecordsToUpdate != null &&
                            command.BatchSaveReportFundsExpendituresRecordsDto.RecordIdsToDelete != null, () =>
            {
                RuleFor(command => command.BatchSaveReportFundsExpendituresRecordsDto)
                    .Must(dto => dto.RecordsToCreate.Count > 0
                        || dto.RecordsToUpdate.Count > 0
                        || dto.RecordIdsToDelete.Count > 0)
                    .WithMessage(ErrorMessagesConstants.BatchOperationMustContainAtLeastOneRecord)
                    .Must(dto => !dto.RecordsToUpdate
                        .Select(r => r.Id)
                        .Intersect(dto.RecordIdsToDelete)
                        .Any())
                    .WithMessage(ErrorMessagesConstants.CollectionsCannotContainIntersectingIds(
                        nameof(BatchSaveReportFundsExpendituresRecordsDto.RecordsToUpdate),
                        nameof(BatchSaveReportFundsExpendituresRecordsDto.RecordIdsToDelete)));

                RuleForEach(command => command.BatchSaveReportFundsExpendituresRecordsDto.RecordsToCreate)
                    .SetValidator(createDtoValidator);

                RuleForEach(command => command.BatchSaveReportFundsExpendituresRecordsDto.RecordsToUpdate)
                    .SetValidator(updateDtoValidator);

                RuleFor(command => command.BatchSaveReportFundsExpendituresRecordsDto.RecordsToUpdate.Select(u => u.Id))
                    .MustHaveUniqueIds(nameof(BatchSaveReportFundsExpendituresRecordsDto.RecordsToUpdate));

                RuleFor(command => command.BatchSaveReportFundsExpendituresRecordsDto.RecordsToUpdate
                        .Select(u => u.CategoryId)
                        .Concat(command.BatchSaveReportFundsExpendituresRecordsDto.RecordsToCreate.Select(c => c.CategoryId)))
                    .MustHaveUniqueIds(nameof(ReportFundsExpendituresRecord.CategoryId));

                When(command => command.BatchSaveReportFundsExpendituresRecordsDto.RecordIdsToDelete.Count > 0, () =>
                {
                    RuleFor(command => command.BatchSaveReportFundsExpendituresRecordsDto.RecordIdsToDelete)
                        .MustBeValidBulkDeleteIds(
                            nameof(BatchSaveReportFundsExpendituresRecordsDto.RecordIdsToDelete),
                            nameof(ReportFundsExpendituresRecord.Id),
                            ReportFundsExpendituresRecordConstants.MaxNumberOfRecordsPerBulkDelete);
                });
            });
        });
    }
}
