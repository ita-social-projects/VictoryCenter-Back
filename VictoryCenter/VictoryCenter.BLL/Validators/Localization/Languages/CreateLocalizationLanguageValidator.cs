using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Localization.Languages.Create;

namespace VictoryCenter.BLL.Validators.Localization.Languages;

public class CreateLocalizationLanguageValidator : AbstractValidator<CreateLocalizationLanguageCommand>
{
    public CreateLocalizationLanguageValidator(BaseLocalizationLanguageValidator baseLocalizationLanguageValidator)
    {
        RuleFor(c => c.CreateLocalizationLanguageDto).SetValidator(baseLocalizationLanguageValidator);
    }
}
