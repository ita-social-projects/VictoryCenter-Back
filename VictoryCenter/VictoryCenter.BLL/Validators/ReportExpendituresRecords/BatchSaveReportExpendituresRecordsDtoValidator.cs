using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Helpers;
using VictoryCenter.BLL.Interfaces.ReportExpendituresRecords;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Validators.ReportExpendituresRecords;

public class BatchSaveReportExpendituresRecordsDtoValidator<TCreateDto, TUpdateDto>
    : AbstractValidator<IBatchSaveReportExpendituresRecordsDto<TCreateDto, TUpdateDto>>
    where TCreateDto : class
    where TUpdateDto : class
{
    public BatchSaveReportExpendituresRecordsDtoValidator(
        BatchSaveReportExpendituresValidatorOptions<TCreateDto, TUpdateDto> options)
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
                        dto.RecordIdsToDelete.Count <= options.MaxNumberOfRecordsPerBatchOperation)
                .WithMessage(ErrorMessagesConstants.CollectionCannotContainMoreThan(
                        ErrorMessagesConstants.BatchOperationTotalRecordsName,
                        options.MaxNumberOfRecordsPerBatchOperation))
                .Custom((dto, context) =>
                {
                    var conflictingIds = dto.RecordsToUpdate
                        .Select(options.UpdateIdSelector)
                        .Intersect(dto.RecordIdsToDelete)
                        .ToList();

                    if (conflictingIds.Count > 0)
                    {
                        var errorMessage = ErrorMessagesConstants.CannotUpdateAndDeleteSameEntity(
                            conflictingIds, options.EntityType);

                        context.AddFailure(errorMessage);
                    }
                });

            RuleForEach(dto => dto.RecordsToCreate)
                .NotNull()
                .SetValidator(options.CreateDtoValidator);

            RuleForEach(dto => dto.RecordsToUpdate)
                .NotNull()
                .SetValidator(options.UpdateDtoValidator);

            RuleFor(dto => dto.RecordsToUpdate
                .Where(u => u != null)
                .Select(options.UpdateIdSelector))
                .MustHaveUniqueIds(nameof(IBatchSaveReportExpendituresRecordsDto<TCreateDto, TUpdateDto>.RecordsToUpdate));

            RuleFor(dto => dto.RecordsToUpdate
                    .Where(u => u != null)
                    .Select(options.UpdateCategoryIdSelector)
                    .Concat(dto.RecordsToCreate
                        .Where(c => c != null)
                        .Select(options.CreateCategoryIdSelector)))
                .MustHaveUniqueIds(options.CategoryIdPropertyName);

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
