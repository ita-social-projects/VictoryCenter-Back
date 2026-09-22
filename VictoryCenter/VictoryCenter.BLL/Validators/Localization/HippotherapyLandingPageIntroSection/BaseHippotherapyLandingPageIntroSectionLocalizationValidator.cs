using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Constants.Localization;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageIntroSection;

namespace VictoryCenter.BLL.Validators.Localization.HippotherapyLandingPageIntroSection;

public class BaseHippotherapyLandingPageIntroSectionLocalizationValidator
    : AbstractValidator<UpdateHippotherapyLandingPageIntroSectionLocalizationDto>
{
    public BaseHippotherapyLandingPageIntroSectionLocalizationValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(
                nameof(UpdateHippotherapyLandingPageIntroSectionLocalizationDto.Title)))
            .MinimumLength(HippotherapyLandingPageIntroSectionLocalizationConstants.TitleMinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(UpdateHippotherapyLandingPageIntroSectionLocalizationDto.Title),
                HippotherapyLandingPageIntroSectionLocalizationConstants.TitleMinLength))
            .MaximumLength(HippotherapyLandingPageIntroSectionLocalizationConstants.TitleMaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(UpdateHippotherapyLandingPageIntroSectionLocalizationDto.Title),
                HippotherapyLandingPageIntroSectionLocalizationConstants.TitleMaxLength));

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(
                nameof(UpdateHippotherapyLandingPageIntroSectionLocalizationDto.Description)))
            .MinimumLength(HippotherapyLandingPageIntroSectionLocalizationConstants.DescriptionMinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(UpdateHippotherapyLandingPageIntroSectionLocalizationDto.Description),
                HippotherapyLandingPageIntroSectionLocalizationConstants.DescriptionMinLength))
            .MaximumLength(HippotherapyLandingPageIntroSectionLocalizationConstants.DescriptionMaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(UpdateHippotherapyLandingPageIntroSectionLocalizationDto.Description),
                HippotherapyLandingPageIntroSectionLocalizationConstants.DescriptionMaxLength));
    }
}
