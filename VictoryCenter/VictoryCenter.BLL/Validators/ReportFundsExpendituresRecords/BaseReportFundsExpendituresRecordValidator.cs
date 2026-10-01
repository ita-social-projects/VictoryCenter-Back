using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ReportFundsExpendituresRecords;
using VictoryCenter.BLL.Helpers;

namespace VictoryCenter.BLL.Validators.ReportFundsExpendituresRecords;

public class BaseReportFundsExpendituresRecordValidator : AbstractValidator<BaseReportFundsExpendituresRecordDto>
{
    public BaseReportFundsExpendituresRecordValidator()
    {
        RuleFor(dto => dto.CategoryId)
            .MustBeValidId(nameof(ReportFundsExpendituresRecordDto.CategoryId));

        RuleFor(dto => dto.Amount)
            .MustBeValidAmountOfMoney(
                nameof(BaseReportFundsExpendituresRecordDto.Amount),
                ReportFundsExpendituresRecordConstants.ZeroAmount,
                ReportFundsExpendituresRecordConstants.AmountPrecision,
                ReportFundsExpendituresRecordConstants.AmountScale,
                ReportFundsExpendituresRecordConstants.AmountFormat);

        RuleFor(dto => dto.Currency)
            .IsInEnum()
            .WithMessage(ErrorMessagesConstants.PropertyMustBeValidEnum(
                nameof(BaseReportFundsExpendituresRecordDto.Currency)));
    }
}
