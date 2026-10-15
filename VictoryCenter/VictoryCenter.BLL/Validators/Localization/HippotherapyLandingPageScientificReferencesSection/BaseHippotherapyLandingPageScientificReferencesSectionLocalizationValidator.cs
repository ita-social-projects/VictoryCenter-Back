using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Constants.Localization;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageScientificReferencesSection;

namespace VictoryCenter.BLL.Validators.Localization.HippotherapyLandingPageScientificReferencesSection;

public class BaseHippotherapyLandingPageScientificReferencesSectionLocalizationValidator
    : AbstractValidator<UpdateHippotherapyLandingPageScientificReferencesSectionLocalizationDto>
{
    public BaseHippotherapyLandingPageScientificReferencesSectionLocalizationValidator(
        HippotherapyLandingPageScientificReferenceLocalizationItemValidator referenceValidator)
    {
        RuleFor(x => x.Title)
            .MustHaveVisibleLength(
                nameof(UpdateHippotherapyLandingPageScientificReferencesSectionLocalizationDto.Title),
                HippotherapyLandingPageScientificReferencesSectionLocalizationConstants.TitleMinLength,
                HippotherapyLandingPageScientificReferencesSectionLocalizationConstants.TitleMaxLength);

        RuleFor(x => x.Description)
            .MustHaveVisibleLength(
                nameof(UpdateHippotherapyLandingPageScientificReferencesSectionLocalizationDto.Description),
                HippotherapyLandingPageScientificReferencesSectionLocalizationConstants.DescriptionMinLength,
                HippotherapyLandingPageScientificReferencesSectionLocalizationConstants.DescriptionMaxLength);

        RuleFor(x => x.References)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateHippotherapyLandingPageScientificReferencesSectionLocalizationDto.References)))
            .Must(references => references.All(r => r is not null))
            .WithMessage(ErrorMessagesConstants.CollectionCannotContainNullElements(nameof(UpdateHippotherapyLandingPageScientificReferencesSectionLocalizationDto.References)))
            .Must(references => references.Select(r => r.ReferenceId).Distinct().Count() == references.Count)
            .WithMessage(ErrorMessagesConstants.CollectionMustContainUniqueValues(nameof(UpdateHippotherapyLandingPageScientificReferencesSectionLocalizationDto.References)));

        RuleForEach(x => x.References).SetValidator(referenceValidator);
    }
}
