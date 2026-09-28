using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Constants.Localization;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageHippoventionSection;
using VictoryCenter.BLL.Helpers;

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
            .Must(v => HtmlContentHelper.StripHtmlTags(v).Length >= HippotherapyLandingPageHippoventionSectionLocalizationConstants.TitleMinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumVisibleLengthOfNCharacters(
                nameof(UpdateHippotherapyLandingPageHippoventionSectionLocalizationDto.Title),
                HippotherapyLandingPageHippoventionSectionLocalizationConstants.TitleMinLength))
            .Must(v => HtmlContentHelper.StripHtmlTags(v).Length <= HippotherapyLandingPageHippoventionSectionLocalizationConstants.TitleMaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumVisibleLengthOfNCharacters(
                nameof(UpdateHippotherapyLandingPageHippoventionSectionLocalizationDto.Title),
                HippotherapyLandingPageHippoventionSectionLocalizationConstants.TitleMaxLength));

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(
                nameof(UpdateHippotherapyLandingPageHippoventionSectionLocalizationDto.Description)))
            .Must(v => HtmlContentHelper.StripHtmlTags(v).Length >= HippotherapyLandingPageHippoventionSectionLocalizationConstants.DescriptionMinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumVisibleLengthOfNCharacters(
                nameof(UpdateHippotherapyLandingPageHippoventionSectionLocalizationDto.Description),
                HippotherapyLandingPageHippoventionSectionLocalizationConstants.DescriptionMinLength))
            .Must(v => HtmlContentHelper.StripHtmlTags(v).Length <= HippotherapyLandingPageHippoventionSectionLocalizationConstants.DescriptionMaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumVisibleLengthOfNCharacters(
                nameof(UpdateHippotherapyLandingPageHippoventionSectionLocalizationDto.Description),
                HippotherapyLandingPageHippoventionSectionLocalizationConstants.DescriptionMaxLength));
    }
}
