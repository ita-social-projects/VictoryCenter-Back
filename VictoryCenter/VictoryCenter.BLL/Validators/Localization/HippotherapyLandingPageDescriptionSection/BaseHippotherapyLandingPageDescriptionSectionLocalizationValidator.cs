using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Constants.Localization;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageDescriptionSection;

namespace VictoryCenter.BLL.Validators.Localization.HippotherapyLandingPageDescriptionSection;

public class BaseHippotherapyLandingPageDescriptionSectionLocalizationValidator
    : AbstractValidator<UpdateHippotherapyLandingPageDescriptionSectionLocalizationDto>
{
    public BaseHippotherapyLandingPageDescriptionSectionLocalizationValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(
                nameof(UpdateHippotherapyLandingPageDescriptionSectionLocalizationDto.Title)))
            .MinimumLength(HippotherapyLandingPageDescriptionSectionLocalizationConstants.TitleMinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(UpdateHippotherapyLandingPageDescriptionSectionLocalizationDto.Title),
                HippotherapyLandingPageDescriptionSectionLocalizationConstants.TitleMinLength))
            .MaximumLength(HippotherapyLandingPageDescriptionSectionLocalizationConstants.TitleMaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(UpdateHippotherapyLandingPageDescriptionSectionLocalizationDto.Title),
                HippotherapyLandingPageDescriptionSectionLocalizationConstants.TitleMaxLength));

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(
                nameof(UpdateHippotherapyLandingPageDescriptionSectionLocalizationDto.Description)))
            .MinimumLength(HippotherapyLandingPageDescriptionSectionLocalizationConstants.DescriptionMinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(UpdateHippotherapyLandingPageDescriptionSectionLocalizationDto.Description),
                HippotherapyLandingPageDescriptionSectionLocalizationConstants.DescriptionMinLength))
            .MaximumLength(HippotherapyLandingPageDescriptionSectionLocalizationConstants.DescriptionMaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(UpdateHippotherapyLandingPageDescriptionSectionLocalizationDto.Description),
                HippotherapyLandingPageDescriptionSectionLocalizationConstants.DescriptionMaxLength));
    }
}
