using FluentValidation;

namespace VictoryCenter.BLL.Validators.ReportExpendituresRecords;

public record BatchSaveReportExpendituresValidatorOptions<TCreateDto, TUpdateDto>
    where TCreateDto : class
    where TUpdateDto : class
{
    public required IValidator<TCreateDto> CreateDtoValidator { get; init; }
    public required IValidator<TUpdateDto> UpdateDtoValidator { get; init; }
    public required int MaxNumberOfRecordsPerBatchOperation { get; init; }
    public required Func<TUpdateDto, long> UpdateIdSelector { get; init; }
    public required Func<TCreateDto, long> CreateCategoryIdSelector { get; init; }
    public required Func<TUpdateDto, long> UpdateCategoryIdSelector { get; init; }
    public required string CategoryIdPropertyName { get; init; }
    public required Type EntityType { get; init; }
}
