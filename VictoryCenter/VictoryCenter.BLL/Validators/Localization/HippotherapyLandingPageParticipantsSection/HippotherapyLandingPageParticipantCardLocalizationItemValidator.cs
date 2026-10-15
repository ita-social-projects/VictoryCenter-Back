using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Constants.Localization;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageParticipantsSection;

namespace VictoryCenter.BLL.Validators.Localization.HippotherapyLandingPageParticipantsSection;

public class HippotherapyLandingPageParticipantCardLocalizationItemValidator
    : AbstractValidator<UpdateHippotherapyLandingPageParticipantCardLocalizationItemDto>
{
    public HippotherapyLandingPageParticipantCardLocalizationItemValidator()
    {
        RuleFor(x => x.CardId)
            .GreaterThan(0)
            .WithMessage(ErrorMessagesConstants.PropertyMustBePositive(nameof(UpdateHippotherapyLandingPageParticipantCardLocalizationItemDto.CardId)));

        RuleFor(x => x.Description)
            .MustHaveVisibleLength(
                nameof(UpdateHippotherapyLandingPageParticipantCardLocalizationItemDto.Description),
                HippotherapyLandingPageParticipantsSectionLocalizationConstants.CardDescriptionMinLength,
                HippotherapyLandingPageParticipantsSectionLocalizationConstants.CardDescriptionMaxLength);
    }
}
