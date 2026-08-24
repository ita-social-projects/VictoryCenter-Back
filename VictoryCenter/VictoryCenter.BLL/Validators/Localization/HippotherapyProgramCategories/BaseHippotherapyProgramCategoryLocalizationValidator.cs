using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Constants.Localization;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgramCategories;

namespace VictoryCenter.BLL.Validators.Localization.HippotherapyProgramCategories;

public class BaseHippotherapyProgramCategoryLocalizationValidator
    : AbstractValidator<UpdateHippotherapyProgramCategoryLocalizationDto>
{
    public BaseHippotherapyProgramCategoryLocalizationValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateHippotherapyProgramCategoryLocalizationDto.Name)))
            .MinimumLength(HippotherapyProgramCategoryLocalizationConstants.NameMinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(UpdateHippotherapyProgramCategoryLocalizationDto.Name),
                HippotherapyProgramCategoryLocalizationConstants.NameMinLength))
            .MaximumLength(HippotherapyProgramCategoryLocalizationConstants.NameMaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(UpdateHippotherapyProgramCategoryLocalizationDto.Name),
                HippotherapyProgramCategoryLocalizationConstants.NameMaxLength));
    }
}
