using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Constants.Localization;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageHippoventionCenterSection;
using VictoryCenter.BLL.Helpers;

namespace VictoryCenter.BLL.Validators.Localization.HippotherapyLandingPageHippoventionCenterSection;

public class BaseHippotherapyLandingPageHippoventionCenterSectionLocalizationValidator
    : AbstractValidator<UpdateHippotherapyLandingPageHippoventionCenterSectionLocalizationDto>
{
    public BaseHippotherapyLandingPageHippoventionCenterSectionLocalizationValidator()
    {
        RuleFor(x => x.Title)
            .MustHaveVisibleLength(
                nameof(UpdateHippotherapyLandingPageHippoventionCenterSectionLocalizationDto.Title),
                HippotherapyLandingPageHippoventionCenterSectionLocalizationConstants.TitleMinLength,
                HippotherapyLandingPageHippoventionCenterSectionLocalizationConstants.TitleMaxLength);

        RuleFor(x => x.Description)
            .NotNull()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(
                nameof(UpdateHippotherapyLandingPageHippoventionCenterSectionLocalizationDto.Description)));

        RuleFor(x => x.Description)
            .MustHaveVisibleLength(
                nameof(UpdateHippotherapyLandingPageHippoventionCenterSectionLocalizationDto.Description),
                HippotherapyLandingPageHippoventionCenterSectionLocalizationConstants.DescriptionMinLength,
                HippotherapyLandingPageHippoventionCenterSectionLocalizationConstants.DescriptionMaxLength)
            .When(x => !string.IsNullOrWhiteSpace(HtmlContentHelper.StripHtmlTags(x.Description ?? string.Empty)));

        RuleFor(x => x.Pros)
            .MustHaveVisibleLength(
                nameof(UpdateHippotherapyLandingPageHippoventionCenterSectionLocalizationDto.Pros),
                HippotherapyLandingPageHippoventionCenterSectionLocalizationConstants.ProsMinLength,
                HippotherapyLandingPageHippoventionCenterSectionLocalizationConstants.ProsMaxLength);
    }
}
