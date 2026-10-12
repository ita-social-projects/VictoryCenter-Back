using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageParticipantsSection.Update;

namespace VictoryCenter.BLL.Validators.Localization.HippotherapyLandingPageParticipantsSection;

public class UpdateHippotherapyLandingPageParticipantsSectionLocalizationValidator
    : AbstractValidator<UpdateHippotherapyLandingPageParticipantsSectionLocalizationCommand>
{
    public UpdateHippotherapyLandingPageParticipantsSectionLocalizationValidator(
        BaseHippotherapyLandingPageParticipantsSectionLocalizationValidator baseValidator)
    {
        RuleFor(x => x.UpdateHippotherapyLandingPageParticipantsSectionLocalizationDto)
            .SetValidator(baseValidator);
    }
}
