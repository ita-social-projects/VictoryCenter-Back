using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Localization.PartnerSections.Update;

namespace VictoryCenter.BLL.Validators.Localization.PartnerSections;

public class UpdatePartnerSectionLocalizationValidator : AbstractValidator<UpdatePartnerSectionLocalizationCommand>
{
    public UpdatePartnerSectionLocalizationValidator(BasePartnerSectionLocalizationValidator baseValidator)
    {
        RuleFor(x => x.UpdatePartnerSectionLocalizationDto).SetValidator(baseValidator);
    }
}
