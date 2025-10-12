using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Localization.Languages.Update;

namespace VictoryCenter.BLL.Validators.Localization.Languages;

public class UpdateLocalizationLanguageValidator : AbstractValidator<UpdateLocalizationLanguageCommand>
{
    public UpdateLocalizationLanguageValidator(BaseLocalizationLanguageValidator baseLocalizationLanguageValidator)
    {
        RuleFor(u => u.UpdateLocalizationLanguageDto).SetValidator(baseLocalizationLanguageValidator);
    }
}
