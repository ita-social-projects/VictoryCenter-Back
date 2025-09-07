using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Localization.Update;

namespace VictoryCenter.BLL.Validators.Localization;

public class UpdateLocalizationLanguageValidator : AbstractValidator<UpdateLocalizationLanguageCommand>
{
    public UpdateLocalizationLanguageValidator(BaseLocalizationLanguageValidator baseLocalizationLanguageValidator)
    {
        RuleFor(u => u.UpdateLocalizationLanguageDto).SetValidator(baseLocalizationLanguageValidator);
    }
}
