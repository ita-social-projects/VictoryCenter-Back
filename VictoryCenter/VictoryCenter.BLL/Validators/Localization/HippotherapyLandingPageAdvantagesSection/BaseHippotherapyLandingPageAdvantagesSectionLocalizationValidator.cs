using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Constants.Localization;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageAdvantagesSection;

namespace VictoryCenter.BLL.Validators.Localization.HippotherapyLandingPageAdvantagesSection;

public class BaseHippotherapyLandingPageAdvantagesSectionLocalizationValidator
    : AbstractValidator<UpdateHippotherapyLandingPageAdvantagesSectionLocalizationDto>
{
    public BaseHippotherapyLandingPageAdvantagesSectionLocalizationValidator(
        HippotherapyLandingPageAdvantageCardLocalizationItemValidator cardValidator)
    {
        RuleFor(x => x.Title)
            .MustHaveVisibleLength(
                nameof(UpdateHippotherapyLandingPageAdvantagesSectionLocalizationDto.Title),
                HippotherapyLandingPageAdvantagesSectionLocalizationConstants.TitleMinLength,
                HippotherapyLandingPageAdvantagesSectionLocalizationConstants.TitleMaxLength);

        RuleFor(x => x.Cards)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateHippotherapyLandingPageAdvantagesSectionLocalizationDto.Cards)))
            .Must(cards => cards.All(c => c is not null)
                && cards.Select(c => c.CardId).Distinct().Count() == cards.Count)
            .WithMessage(ErrorMessagesConstants.CollectionMustContainUniqueValues(nameof(UpdateHippotherapyLandingPageAdvantagesSectionLocalizationDto.Cards)));

        RuleForEach(x => x.Cards).SetValidator(cardValidator);
    }
}
