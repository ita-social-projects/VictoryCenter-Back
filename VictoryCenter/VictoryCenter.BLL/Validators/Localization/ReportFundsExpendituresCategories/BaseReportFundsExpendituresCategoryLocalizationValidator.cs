using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Constants.Localization;
using VictoryCenter.BLL.DTOs.Admin.Localization.ReportFundsExpendituresCategories;

namespace VictoryCenter.BLL.Validators.Localization.ReportFundsExpendituresCategories;

public class BaseReportFundsExpendituresCategoryLocalizationValidator
    : AbstractValidator<UpdateReportFundsExpendituresCategoryLocalizationDto>
{
    public BaseReportFundsExpendituresCategoryLocalizationValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateReportFundsExpendituresCategoryLocalizationDto.Name)))
            .MinimumLength(ReportFundsExpendituresCategoryLocalizationConstants.NameMinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(UpdateReportFundsExpendituresCategoryLocalizationDto.Name),
                ReportFundsExpendituresCategoryLocalizationConstants.NameMinLength))
            .MaximumLength(ReportFundsExpendituresCategoryLocalizationConstants.NameMaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(UpdateReportFundsExpendituresCategoryLocalizationDto.Name),
                ReportFundsExpendituresCategoryLocalizationConstants.NameMaxLength));
    }
}
