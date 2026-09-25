using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Constants.Localization;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageHippoventionSection;

namespace VictoryCenter.BLL.Validators.Localization.HippotherapyLandingPageHippoventionSection;

public class BaseHippotherapyLandingPageHippoventionSectionLocalizationValidator
    : AbstractValidator<UpdateHippotherapyLandingPageHippoventionSectionLocalizationDto>
{
    public BaseHippotherapyLandingPageHippoventionSectionLocalizationValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(
                nameof(UpdateHippotherapyLandingPageHippoventionSectionLocalizationDto.Title)))
            .MinimumLength(HippotherapyLandingPageHippoventionSectionLocalizationConstants.TitleMinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(UpdateHippotherapyLandingPageHippoventionSectionLocalizationDto.Title),
                HippotherapyLandingPageHippoventionSectionLocalizationConstants.TitleMinLength))
            .MaximumLength(HippotherapyLandingPageHippoventionSectionLocalizationConstants.TitleMaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(UpdateHippotherapyLandingPageHippoventionSectionLocalizationDto.Title),
                HippotherapyLandingPageHippoventionSectionLocalizationConstants.TitleMaxLength));

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(
                nameof(UpdateHippotherapyLandingPageHippoventionSectionLocalizationDto.Description)))
            .MinimumLength(HippotherapyLandingPageHippoventionSectionLocalizationConstants.DescriptionMinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(UpdateHippotherapyLandingPageHippoventionSectionLocalizationDto.Description),
                HippotherapyLandingPageHippoventionSectionLocalizationConstants.DescriptionMinLength))
            .MaximumLength(HippotherapyLandingPageHippoventionSectionLocalizationConstants.DescriptionMaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(UpdateHippotherapyLandingPageHippoventionSectionLocalizationDto.Description),
                HippotherapyLandingPageHippoventionSectionLocalizationConstants.DescriptionMaxLength));
    }
}
