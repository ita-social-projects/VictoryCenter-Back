using FluentValidation;
using VictoryCenter.BLL.Constants;

namespace VictoryCenter.BLL.Helpers;

public static class ReportRecordsValidationExtensions
{
    public static IRuleBuilderOptions<T, decimal?> MustBeValidAmountOfMoney<T>(
        this IRuleBuilder<T, decimal?> ruleBuilder,
        string property,
        decimal minValue,
        int precision,
        int scale,
        string format)
    {
        return ruleBuilder
            .NotNull()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(property))
            .GreaterThanOrEqualTo(minValue)
            .WithMessage(ErrorMessagesConstants.SumMustNotBeNegative(property))
            .NotEqual(minValue)
            .WithMessage(ErrorMessagesConstants.SumNotEqualTo(
                property, minValue))
            .PrecisionScale(
                precision,
                scale,
                true)
            .WithMessage(ErrorMessagesConstants.PropertyMustBeInAValidFormat(
                property,
                format));
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
        string property,
        int reportingYearMinValue,
        int reportingYearMaxValue)
    {
        return ruleBuilder
            .GreaterThanOrEqualTo(reportingYearMinValue)
            .WithMessage(ErrorMessagesConstants.PropertyMustBeGreaterThanOrEqualToN(
                property,
                reportingYearMinValue))
            .LessThanOrEqualTo(reportingYearMaxValue)
            .WithMessage(ErrorMessagesConstants.PropertyMustBeLessThanOrEqualToN(
                property,
                reportingYearMaxValue));
    }

    public static IRuleBuilderOptions<T, IEnumerable<long>> MustHaveUniqueIds<T>(
        this IRuleBuilder<T, IEnumerable<long>> ruleBuilder,
        string collection)
    {
        return ruleBuilder.Must(e =>
        {
            var enumerable = e as long[] ?? [.. e];
            return enumerable.Length == enumerable.Distinct().Count();
        })
            .WithMessage(ErrorMessagesConstants.CollectionMustContainUniqueValues(collection));
    }

    public static IRuleBuilderOptions<T, IEnumerable<long>> MustBeValidBulkDeleteIds<T>(
        this IRuleBuilder<T, IEnumerable<long>> ruleBuilder,
        string collection,
        string property,
        int maxNumberOfRecords)
    {
        ruleBuilder
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.CollectionCannotBeEmpty(collection))
            .Must(e => e.Count() <= maxNumberOfRecords)
            .WithMessage(ErrorMessagesConstants.CollectionCannotContainMoreThan(
                collection,
                maxNumberOfRecords))
            .MustHaveUniqueIds(collection);

        return ruleBuilder.ForEach(idRule =>
            idRule.MustBeValidId(property));
    }
}
