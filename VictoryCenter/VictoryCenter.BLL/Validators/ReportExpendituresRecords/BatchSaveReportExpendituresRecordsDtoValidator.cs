using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Helpers;
using VictoryCenter.BLL.Interfaces.ReportExpendituresRecords;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Validators.ReportExpendituresRecords;

public class BatchSaveReportExpendituresRecordsDtoValidator<TCreateDto, TUpdateDto>
    : AbstractValidator<IBatchSaveReportExpendituresRecordsDto<TCreateDto, TUpdateDto>>
{
    public BatchSaveReportExpendituresRecordsDtoValidator(
        IValidator<TCreateDto> createDtoValidator,
        IValidator<TUpdateDto> updateDtoValidator,
        int maxNumberOfRecordsPerBatchOperation,
        Func<TUpdateDto, long> updateIdSelector,
        Func<TCreateDto, long> createCategoryIdSelector,
        Func<TUpdateDto, long> updateCategoryIdSelector,
        string categoryIdPropertyName)
    {
        RuleFor(dto => dto.RecordsToCreate)
            .NotNull()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(
                nameof(IBatchSaveReportExpendituresRecordsDto<TCreateDto, TUpdateDto>.RecordsToCreate)));

        RuleFor(dto => dto.RecordsToUpdate)
            .NotNull()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(
                nameof(IBatchSaveReportExpendituresRecordsDto<TCreateDto, TUpdateDto>.RecordsToUpdate)));

        RuleFor(dto => dto.RecordIdsToDelete)
            .NotNull()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(
                nameof(IBatchSaveReportExpendituresRecordsDto<TCreateDto, TUpdateDto>.RecordIdsToDelete)));

        When(
            dto => dto.RecordsToCreate != null &&
                    dto.RecordsToUpdate != null &&
                    dto.RecordIdsToDelete != null, () =>
        {
            RuleFor(dto => dto)
                .Must(dto => dto.RecordsToCreate.Count > 0
                        || dto.RecordsToUpdate.Count > 0
                        || dto.RecordIdsToDelete.Count > 0)
                .WithMessage(ErrorMessagesConstants.BatchOperationMustContainAtLeastOneRecord)
                .Must(dto =>
                        dto.RecordsToCreate.Count +
                        dto.RecordsToUpdate.Count +
                        dto.RecordIdsToDelete.Count <= maxNumberOfRecordsPerBatchOperation)
                .WithMessage(ErrorMessagesConstants.CollectionCannotContainMoreThan(
                        ErrorMessagesConstants.BatchOperationTotalRecordsName,
                        maxNumberOfRecordsPerBatchOperation))
                .Custom((dto, context) =>
                {
                    var conflictingIds = dto.RecordsToUpdate
                        .Select(updateIdSelector)
                        .Intersect(dto.RecordIdsToDelete)
                        .ToList();

                    if (conflictingIds.Count > 0)
                    {
                        var errorMessage = ErrorMessagesConstants.CannotUpdateAndDeleteSameEntity(
                            conflictingIds, typeof(ReportFundsExpendituresRecord));

                        context.AddFailure(errorMessage);
                    }
                });

            RuleForEach(dto => dto.RecordsToCreate)
                .SetValidator(createDtoValidator);

            RuleForEach(dto => dto.RecordsToUpdate)
                .SetValidator(updateDtoValidator);

            RuleFor(dto => dto.RecordsToUpdate.Select(updateIdSelector))
                .MustHaveUniqueIds(nameof(IBatchSaveReportExpendituresRecordsDto<TCreateDto, TUpdateDto>.RecordsToUpdate));

            RuleFor(dto => dto.RecordsToUpdate
                    .Select(updateCategoryIdSelector)
                    .Concat(dto.RecordsToCreate.Select(createCategoryIdSelector)))
                .MustHaveUniqueIds(categoryIdPropertyName);

            When(dto => dto.RecordIdsToDelete.Count > 0, () =>
            {
                RuleFor(dto => dto.RecordIdsToDelete)
                    .MustContainValidIds(
                        nameof(IBatchSaveReportExpendituresRecordsDto<TCreateDto, TUpdateDto>.RecordIdsToDelete),
                        nameof(ReportFundsExpendituresRecord.Id));
            });
        });
    }
}
