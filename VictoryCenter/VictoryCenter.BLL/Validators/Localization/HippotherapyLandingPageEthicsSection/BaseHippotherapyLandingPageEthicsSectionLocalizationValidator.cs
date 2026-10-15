using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Constants.Localization;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageEthicsSection;

namespace VictoryCenter.BLL.Validators.Localization.HippotherapyLandingPageEthicsSection;

public class BaseHippotherapyLandingPageEthicsSectionLocalizationValidator
    : AbstractValidator<UpdateHippotherapyLandingPageEthicsSectionLocalizationDto>
{
    public BaseHippotherapyLandingPageEthicsSectionLocalizationValidator(
        HippotherapyLandingPageEthicsPrincipleLocalizationItemValidator principleValidator)
    {
        RuleFor(x => x.Title)
            .MustHaveVisibleLength(
                nameof(UpdateHippotherapyLandingPageEthicsSectionLocalizationDto.Title),
                HippotherapyLandingPageEthicsSectionLocalizationConstants.TitleMinLength,
                HippotherapyLandingPageEthicsSectionLocalizationConstants.TitleMaxLength);

        RuleFor(x => x.Description)
            .MustHaveVisibleLength(
                nameof(UpdateHippotherapyLandingPageEthicsSectionLocalizationDto.Description),
                HippotherapyLandingPageEthicsSectionLocalizationConstants.DescriptionMinLength,
                HippotherapyLandingPageEthicsSectionLocalizationConstants.DescriptionMaxLength);

        RuleFor(x => x.Principles)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateHippotherapyLandingPageEthicsSectionLocalizationDto.Principles)))
            .Must(principles => principles.All(r => r is not null))
            .WithMessage(ErrorMessagesConstants.CollectionCannotContainNullElements(nameof(UpdateHippotherapyLandingPageEthicsSectionLocalizationDto.Principles)))
            .Must(principles => principles.Select(r => r.PrincipleId).Distinct().Count() == principles.Count)
            .WithMessage(ErrorMessagesConstants.CollectionMustContainUniqueValues(nameof(UpdateHippotherapyLandingPageEthicsSectionLocalizationDto.Principles)));

        RuleForEach(x => x.Principles).SetValidator(principleValidator);
    }
}
