using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Constants.Localization;
using VictoryCenter.BLL.DTOs.Admin.Localization.Languages;

namespace VictoryCenter.BLL.Validators.Localization.Languages;

public class BaseLocalizationLanguageValidator : AbstractValidator<CreateLocalizationLanguageDto>
{
    public BaseLocalizationLanguageValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(CreateLocalizationLanguageDto.Code)))
            .Length(LocalizationLanguageConstants.CodeLength).WithMessage(ErrorMessagesConstants.PropertyMustHaveALengthOfNCharacters(nameof(CreateLocalizationLanguageDto.Code), LocalizationLanguageConstants.CodeLength));
        RuleFor(x => x.Name)
           .NotEmpty().WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(CreateLocalizationLanguageDto.Name)))
           .MaximumLength(LocalizationLanguageConstants.NameMaxLength).WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(nameof(CreateLocalizationLanguageDto.Name), LocalizationLanguageConstants.NameMaxLength));
    }
}
