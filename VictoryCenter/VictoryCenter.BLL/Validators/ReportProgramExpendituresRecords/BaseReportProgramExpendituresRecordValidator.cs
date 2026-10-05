using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ReportProgramExpendituresRecords;
using VictoryCenter.BLL.Helpers;

namespace VictoryCenter.BLL.Validators.ReportProgramExpendituresRecords;

public class BaseReportProgramExpendituresRecordValidator
    : AbstractValidator<BaseReportProgramExpendituresRecordDto>
{
    public BaseReportProgramExpendituresRecordValidator()
    {
        RuleFor(dto => dto.HippotherapyProgramCategoryId)
            .MustBeValidId(nameof(ReportProgramExpendituresRecordDto.HippotherapyProgramCategoryId));

        RuleFor(dto => dto.AmountUah)
            .MustBeValidAmountOfMoney(
                nameof(ReportProgramExpendituresRecordDto.AmountUah),
                ReportProgramExpendituresRecordConstants.ZeroAmount,
                ReportProgramExpendituresRecordConstants.AmountPrecision,
                ReportProgramExpendituresRecordConstants.AmountScale,
                ReportProgramExpendituresRecordConstants.AmountFormat);

        RuleFor(dto => dto.AmountUsd)
            .MustBeValidAmountOfMoney(
                nameof(ReportProgramExpendituresRecordDto.AmountUsd),
                ReportProgramExpendituresRecordConstants.ZeroAmount,
                ReportProgramExpendituresRecordConstants.AmountPrecision,
                ReportProgramExpendituresRecordConstants.AmountScale,
                ReportProgramExpendituresRecordConstants.AmountFormat);
    }
}
