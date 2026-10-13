using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Constants.Localization;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageParticipantsSection;

namespace VictoryCenter.BLL.Validators.Localization.HippotherapyLandingPageParticipantsSection;

public class BaseHippotherapyLandingPageParticipantsSectionLocalizationValidator
    : AbstractValidator<UpdateHippotherapyLandingPageParticipantsSectionLocalizationDto>
{
    public BaseHippotherapyLandingPageParticipantsSectionLocalizationValidator(
        HippotherapyLandingPageParticipantCardLocalizationItemValidator cardValidator)
    {
        RuleFor(x => x.Title)
            .MustHaveVisibleLength(
                nameof(UpdateHippotherapyLandingPageParticipantsSectionLocalizationDto.Title),
                HippotherapyLandingPageParticipantsSectionLocalizationConstants.TitleMinLength,
                HippotherapyLandingPageParticipantsSectionLocalizationConstants.TitleMaxLength);

        RuleFor(x => x.Cards)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateHippotherapyLandingPageParticipantsSectionLocalizationDto.Cards)))
            .Must(cards => cards.All(c => c is not null))
            .WithMessage(ErrorMessagesConstants.CollectionCannotContainNullElements(nameof(UpdateHippotherapyLandingPageParticipantsSectionLocalizationDto.Cards)))
            .Must(cards => cards.Select(c => c.CardId).Distinct().Count() == cards.Count)
            .WithMessage(ErrorMessagesConstants.CollectionMustContainUniqueValues(nameof(UpdateHippotherapyLandingPageParticipantsSectionLocalizationDto.Cards)));

        RuleForEach(x => x.Cards).SetValidator(cardValidator);
    }
}
