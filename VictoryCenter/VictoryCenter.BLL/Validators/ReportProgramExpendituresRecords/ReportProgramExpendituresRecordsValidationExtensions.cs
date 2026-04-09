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
            .GreaterThan(ReportFundsExpendituresRecordConstants.AmountMinValue)
            .WithMessage(
                ErrorMessagesConstants.PropertyMustBePositive(property))
            .PrecisionScale(
                ReportFundsExpendituresRecordConstants.AmountPrecision,
                ReportFundsExpendituresRecordConstants.AmountScale,
                true)
            .WithMessage(ErrorMessagesConstants.PropertyMustBeInAValidFormat(
                property,
                ReportFundsExpendituresRecordConstants.AmountFormat));
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
}
