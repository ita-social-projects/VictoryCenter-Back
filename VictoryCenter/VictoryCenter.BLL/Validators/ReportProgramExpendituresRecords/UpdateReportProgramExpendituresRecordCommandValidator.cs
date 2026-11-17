using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.ReportProgramExpendituresRecords.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ReportProgramExpendituresRecords;
using VictoryCenter.BLL.Helpers;

namespace VictoryCenter.BLL.Validators.ReportProgramExpendituresRecords;

public class UpdateReportProgramExpendituresRecordCommandValidator
    : AbstractValidator<UpdateReportProgramExpendituresRecordCommand>
{
    public UpdateReportProgramExpendituresRecordCommandValidator()
    {
        RuleFor(x => x.ReportProgramExpendituresRecordId)
            .MustBeValidId(
                nameof(UpdateReportProgramExpendituresRecordCommand.ReportProgramExpendituresRecordId));

        RuleFor(x => x.Dto)
            .NotNull()
            .DependentRules(() =>
            {
                RuleFor(x => x.Dto.HippotherapyProgramCategoryId)
                    .MustBeValidId(
                        nameof(UpdateReportProgramExpendituresRecordDto.HippotherapyProgramCategoryId));
                RuleFor(x => x.Dto.AmountUah)
                    .MustBeValidAmountOfMoney(
                        nameof(UpdateReportProgramExpendituresRecordDto.AmountUah),
                        ReportProgramExpendituresRecordConstants.ZeroAmount,
                        ReportProgramExpendituresRecordConstants.AmountPrecision,
                        ReportProgramExpendituresRecordConstants.AmountScale,
                        ReportProgramExpendituresRecordConstants.AmountFormat);
                RuleFor(x => x.Dto.AmountUsd)
                    .MustBeValidAmountOfMoney(
                        nameof(UpdateReportProgramExpendituresRecordDto.AmountUsd),
                        ReportProgramExpendituresRecordConstants.ZeroAmount,
                        ReportProgramExpendituresRecordConstants.AmountPrecision,
                        ReportProgramExpendituresRecordConstants.AmountScale,
                        ReportProgramExpendituresRecordConstants.AmountFormat);
            });
    }
}
