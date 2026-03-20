using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ReportFundsExpendituresCategories;

namespace VictoryCenter.BLL.Validators.ReportFundsExpendituresCategories;

public class BaseReportFundsExpendituresCategoryValidator : AbstractValidator<BaseReportFundsExpendituresCategoryDto>
{
    public BaseReportFundsExpendituresCategoryValidator()
    {
        RuleFor(dto => dto.Name)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(ReportFundsExpendituresCategoryDto.Name)))
            .MaximumLength(ReportFundsExpendituresCategoryConstants.NameMaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(ReportFundsExpendituresCategoryDto.Name),
                ReportFundsExpendituresCategoryConstants.NameMaxLength));

        RuleFor(dto => dto.Type)
            .IsInEnum()
            .WithMessage(ErrorMessagesConstants.PropertyMustBeValidEnum(nameof(ReportFundsExpendituresCategoryDto.Type)));
    }
}
