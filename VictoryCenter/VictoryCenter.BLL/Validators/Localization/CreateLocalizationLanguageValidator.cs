using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Localization.Create;

namespace VictoryCenter.BLL.Validators.Localization;

public class CreateLocalizationLanguageValidator : AbstractValidator<CreateLocalizationLanguageCommand>
{
    public CreateLocalizationLanguageValidator(BaseLocalizationLanguageValidator baseLocalizationLanguageValidator)
    {
        RuleFor(c => c.CreateLocalizationLanguageDto).SetValidator(baseLocalizationLanguageValidator);
    }
}
