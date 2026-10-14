using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageParticipantsSection.Create;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageParticipantsSection;
using VictoryCenter.BLL.Validators.Localization.Base;

namespace VictoryCenter.BLL.Validators.Localization.HippotherapyLandingPageParticipantsSection;

public class CreateHippotherapyLandingPageParticipantsSectionLocalizationValidator
    : AbstractValidator<CreateHippotherapyLandingPageParticipantsSectionLocalizationCommand>
{
    public CreateHippotherapyLandingPageParticipantsSectionLocalizationValidator(
        BaseHippotherapyLandingPageParticipantsSectionLocalizationValidator baseValidator)
    {
        RuleFor(c => c.CreateHippotherapyLandingPageParticipantsSectionLocalizationDto)
            .SetValidator(new LocalizationIdentityValidator<CreateHippotherapyLandingPageParticipantsSectionLocalizationDto>());
        RuleFor(c => c.CreateHippotherapyLandingPageParticipantsSectionLocalizationDto)
            .SetValidator(baseValidator);
    }
}
