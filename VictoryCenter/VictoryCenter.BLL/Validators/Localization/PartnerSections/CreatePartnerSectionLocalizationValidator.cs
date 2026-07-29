using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Localization.PartnerSections.Create;
using VictoryCenter.BLL.DTOs.Admin.Localization.PartnerSections;
using VictoryCenter.BLL.Validators.Localization.Base;

namespace VictoryCenter.BLL.Validators.Localization.PartnerSections;

public class CreatePartnerSectionLocalizationValidator : AbstractValidator<CreatePartnerSectionLocalizationCommand>
{
    public CreatePartnerSectionLocalizationValidator(BasePartnerSectionLocalizationValidator baseValidator)
    {
        RuleFor(x => x.CreatePartnerSectionLocalizationDto)
            .SetValidator(new LocalizationIdentityValidator<CreatePartnerSectionLocalizationDto>());

        RuleFor(x => x.CreatePartnerSectionLocalizationDto).SetValidator(baseValidator);
    }
}
