using FluentValidation;
using VictoryCenter.BLL.Constants;

namespace VictoryCenter.BLL.Validators.ReportProgramExpendituresRecords;

public static class ReportProgramExpendituresRecordsValidationExtensions
{
    public static IRuleBuilderOptions<T, decimal> MustBeValidAmountOfMoney<T>(
        this IRuleBuilder<T, decimal> ruleBuilder,
        string property)
    {
        return ruleBuilder
            .GreaterThan(ReportProgramExpendituresRecordConstants.AmountMinValue)
            .WithMessage(
                ErrorMessagesConstants.PropertyMustBePositive(property))
            .PrecisionScale(
                ReportProgramExpendituresRecordConstants.AmountPrecision,
                ReportProgramExpendituresRecordConstants.AmountScale,
                true)
            .WithMessage(ErrorMessagesConstants.PropertyMustBeInAValidFormat(
                property,
                ReportProgramExpendituresRecordConstants.AmountFormat));
    }

    public static IRuleBuilderOptions<T, long> MustBeValidId<T>(
        this IRuleBuilder<T, long> ruleBuilder,
        string property)
    {
        return ruleBuilder
            .GreaterThan(0)
            .WithMessage(
                ErrorMessagesConstants.PropertyMustBePositive(property));
    }

    public static IRuleBuilderOptions<T, int> MustBeValidReportingYear<T>(
        this IRuleBuilder<T, int> ruleBuilder,
        string property)
    {
        return ruleBuilder
            .GreaterThanOrEqualTo(ReportProgramExpendituresRecordConstants.ReportingYearMinValue)
            .WithMessage(ErrorMessagesConstants.PropertyMustBeGreaterThanOrEqualToN(
                property,
                ReportProgramExpendituresRecordConstants.ReportingYearMinValue))
            .LessThanOrEqualTo(ReportProgramExpendituresRecordConstants.ReportingYearMaxValue)
            .WithMessage(ErrorMessagesConstants.PropertyMustBeLessThanOrEqualToN(
                property,
                ReportProgramExpendituresRecordConstants.ReportingYearMaxValue));
    }

    public static IRuleBuilderOptions<T, IEnumerable<long>> MustHaveUniqueIds<T>(
        this IRuleBuilder<T, IEnumerable<long>> ruleBuilder,
        string collection)
    {
        return ruleBuilder.Must(e =>
            {
                var enumerable = e as long[] ?? e.ToArray();
                return enumerable.Count() == enumerable.Distinct().Count();
            })
            .WithMessage(ErrorMessagesConstants.CollectionMustContainUniqueValues(collection));
    }
}
